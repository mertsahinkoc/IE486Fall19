using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using FLOW.NET.Layout;

namespace FLOW.NET.Operational.Events
{
    public class SeizeNodeEvent : Event
    {
        private Transporter transporter;
        public SeizeNodeEvent()
        {
        }

        public SeizeNodeEvent(double timeIn, SimulationManager managerIn, Transporter transporterIn) : base(timeIn,managerIn)
        {
            this.transporter = transporterIn;
        }

        public Transporter Transporter
        {
            get { return this.transporter; }
            set { this.transporter = value; }
        }
        public override EventState GetEventState()
        {
            SeizeNodeEventState eventState = new SeizeNodeEventState();
            eventState.Transporter = this.transporter.Name;
            eventState.Time = this.Time;
            return eventState;
        }
        protected override void Operation()
        {
            this.Transporter.onRoad = false;
            if (this.transporter.Route[0].Content.Count < this.transporter.Route[0].Capacity)
            {
                transporter.Route[0].Receive(this.Time, transporter);
                transporter.BecomeFree(this.Time);
            }
            else
            {
                transporter.Route[0].TransporterQueue.add(transporter)
                transporter.BecomeBlocked(this.Time);
                return;
            }
            if (transporter.Location = transporter.ParkNode)
            {
                if (transporter.assignedStorage != null)
                {
                    this.Manager.EventCalendar.ScheduleSeizeNodeEvent(this.Time + transporter.TravelTime.GenerateValue(), transporter);
                    transporter.Route[0].Release(this.Time,transporter);
                    transporter.onRoad = true;
                    return;
                }
            }
            if (transporter.Route.Count == 0)
            {
                if (transporter.Content.Count > 0)
                {
                    this.Manager.EventCalendar.ScheduleEndLoadUnloadEvent(this.Time + transporter.TransferTime.GenerateValue(), transporter);
                    transporter.InTransfer = true;
                    foreach (Bin bin in transporter.Content)
                    {
                        bin.InTransfer = true;
                    }
                    return;
                }
                else
                {
                    this.Manager.TriggerSupermarketControllerAlgorithm(this.transporter.assignedStorage);
                    return;

                }
            }

            if (transporter.Route.Count > 0)
            {
                BinList binsToUnload = transporter.Route[0].GetBinsToUnload((Storage)transporter.Location);
                BinList binsToCollect = this.Manager.BinCollectorAlgorithm.Execute(binsToUnload, (Storage)transporter.Location);
                
                //ScheduleEndLoadUnloadEvent icine binsToCollect almiyor ona gore duzenlememiz gerekiyor. Bunun disinda icine neden binmagazine aldigina bakip guncellememiz gerekiyr.
                this.Manager.EventCalendar.ScheduleEndLoadUnloadEvent(this.Time + transporter.TravelTime.GenerateValue(), binsToUnload, transporter, binsToCollect);

                foreach (Bin bin in binsToUnload)
                {
                    bin.InTransfer = true;
                }
                foreach (Bin bin in binsToCollect)
                {
                    bin.InTransfer = true;
                }
                return;
            }
        }
        protected override void TraceEvent()
        {
        }

    }

    [XmlType("SeizeNodeEventState")]
    public class SeizeNodeEventState : EventState
    {
        private string transporter;

        [XmlElement("Transporter")]
        public string Transporter
        {
            get { return this.transporter; }
            set { this.transporter = value; }
        }

        public override Event GetEvent(SimulationManager managerIn)
        {
            //nasil bir implementation gerektigini anlamadim.
            return null;
        }


    }
}

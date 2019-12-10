using System;
using System.Diagnostics;
using System.Xml.Serialization;
using FLOW.NET.Layout;
using System.IO;

namespace FLOW.NET.Operational.Events
{
    public class StartProcessEvent : Event
    {
        private Processor processor;

        private Unitload unitload;

        private double transferTime;

        public StartProcessEvent()
        {
        }

        public StartProcessEvent(double timeIn, SimulationManager managerIn, Processor processorIn, Unitload unitloadIn, double transferTimeIn)
            : base(timeIn, managerIn)
        {
            this.processor = processorIn;
            this.unitload = unitloadIn;
            this.transferTime = transferTimeIn;
        }

        public Processor Processor
        {
            get { return this.processor; }
            set { this.processor = value; }
        }

        public Unitload Unitload
        {
            get { return this.unitload; }
            set { this.unitload = value; }
        }

        public override EventState GetEventState()
        {
            StartProcessEventState eventState = new StartProcessEventState();
            eventState.Cell = this.processor.Parent.Name;
            eventState.Processor = this.processor.Name;
            eventState.Time = this.Time;
            eventState.Unitload = this.unitload.Name;
            return eventState;
        }

        protected override void Operation()
        {

            Station station = (Station)this.processor.Parent;
            this.Manager.TriggerStationControllerAlgorithm(station);
            if (this.unitload.Location == null)
            {
                this.processor.Receive(this.Time, this.unitload);
            }
            else
            {
                Queue inQueue = (Queue)this.unitload.Location;
                station = (Station)inQueue.Parent;
                inQueue.Release(this.Time, this.unitload);
                this.processor.Receive(this.Time, this.unitload);
                unitload.InTransfer = false;
                //if the queue was full, now there is an empty spot and blocked units can come. Link between pull and push algorithms.
                if (inQueue.Capacity - (inQueue.Content.Count + inQueue.Reserved) == 1)
                {
                    this.Manager.TriggerPull(inQueue); //will be checked whether there is a candidate blocked unitload(s) for this queue.
                }
            }

            // Assumption: If there is a unitload on processor and if the processor is down, the unitload will stay until the repair event.
            if (this.processor.IsBroken == false) //No Calendar Breakdown
            {
                double processTime = station.CalculateProcessTime(unitload);

                if (this.processor.NextBreakdown > processTime)
                {
                    this.processor.NextBreakdown -= processTime;
                    this.Manager.EventCalendar.ScheduleEndProcessEvent(this.Manager.Time + processTime, processor);
                }

                else
                {
                    this.Manager.EventCalendar.ScheduleProcessorBreakdownEvent(this.Manager.Time + processor.NextBreakdown, processor);
                }
            }
        }

        protected override void TraceEvent()
        {
            Debug.WriteLine(String.Format("STARTPROCESS [{0}, {1}, {2}]", this.Time, this.processor.Name, this.unitload.Name));
        }
    }

    [XmlType("SeizeProcessorEventState")]
    public class StartProcessEventState : EventState
    {
        private string cell;

        private string processor;

        private string unitload;

        private double transferTime;

        [XmlElement("Cell")]
        public string Cell
        {
            get { return this.cell; }
            set { this.cell = value; }
        }

        [XmlElement("Processor")]
        public string Processor
        {
            get { return this.processor; }
            set { this.processor = value; }
        }

        [XmlElement("Unitload")]
        public string Unitload
        {
            get { return this.unitload; }
            set { this.unitload = value; }
        }

        public override Event GetEvent(SimulationManager managerIn)
        {
            StationList processCells = managerIn.LayoutManager.Layout.Stations;
            UnitloadList unitloads = managerIn.JobManager.Unitloads;
            return new StartProcessEvent(this.Time, managerIn, processCells[this.cell].Processors[this.processor], unitloads[this.unitload], this.transferTime);
        }
    }
}
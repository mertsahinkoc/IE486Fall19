using System;
using System.Diagnostics;
using System.Xml.Serialization;
using FLOW.NET.Layout;

namespace FLOW.NET.Operational.Events
{
    public class EndProcessEvent : Event
    {
        private Processor processor;

        public EndProcessEvent()
        {
        }

        public EndProcessEvent(double timeIn, SimulationManager managerIn, Processor processorIn)
            : base(timeIn, managerIn)
        {
            this.processor = processorIn;
        }

        public Processor Processor
        {
            get { return this.processor; }
            set { this.processor = value; }
        }

        public override EventState GetEventState()
        {
            EndProcessEventState eventState = new EndProcessEventState();
            eventState.Cell = this.processor.Parent.Name;
            eventState.Processor = this.processor.Name;
            eventState.Time = this.Time;
            return eventState;
        }

        protected override void Operation()
        {
            Station station = (Station)this.processor.Parent;
            Unitload unitload = (Unitload)this.processor.Content[0];
            unitload.CompleteOperation();
            unitload.EndProcessTime = this.Time;
            Job job = (Job)unitload.Parent;
            if (job.JobType.OutputStation.Name == station.Name) //Job departure
            {
                job.Unitloads.Remove(unitload);
                job.Completed.Add(unitload);
                unitload.Alternates[0].updateUsage(this.Time);
                this.processor.Release(this.Manager.Time, unitload);
                this.Manager.TriggerStationControllerAlgorithm(station);
                if (job.IsCompleted == true)
                {

                    this.Manager.EventCalendar.ScheduleDepartureEvent(this.Time, job);
                }
            }
            else
            {
                if (job.JobType.InputStation.Name == station.Name) //releasing job
                {
                    if (job.StartTime == Double.PositiveInfinity)
                    {
                        job.StartTime = this.Manager.Time;
                    }
                }

                this.Manager.Algorithms.StationSelection.Execute(unitload); // first station selections, after arrival at station processor will be seleceted.
                this.Manager.TriggerPush(unitload); //Push to selected station's inqueue.
            }

        }

        protected override void TraceEvent()
        {
            Debug.WriteLine(String.Format("ENDPROCESS [{0}, {1}]", this.Time, this.processor.Name));
        }
    }

    [XmlType("EndProcessEventState")]
    public class EndProcessEventState : EventState
    {
        private string cell;

        private string processor;

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

        public override Event GetEvent(SimulationManager managerIn)
        {
            StationList processCells = managerIn.LayoutManager.Layout.Stations;
            return new EndProcessEvent(this.Time, managerIn, processCells[this.cell].Processors[this.processor]);
        }
    }
}
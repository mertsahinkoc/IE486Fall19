using System;
using System.Diagnostics;
using System.Xml.Serialization;
using FLOW.NET.Layout;

namespace FLOW.NET.Operational.Events
{
    public class ProcessorBreakdownEvent : Event
    {
        private Processor processor;

        public ProcessorBreakdownEvent()
        {
        }

        public ProcessorBreakdownEvent(double timeIn, SimulationManager managerIn, Processor processorIn)
            : base(timeIn, managerIn)
        {
            this.processor = processorIn;
        }

        public override bool IsPermanent
        {
            get { return true; }
        }

        public Processor Processor
        {
            get { return this.processor; }
            set { this.processor = value; }
        }

        public override EventState GetEventState()
        {
            ProcessorBreakdownEventState eventState = new ProcessorBreakdownEventState();
            eventState.Cell = this.processor.Parent.Name;
            eventState.Processor = this.processor.Name;
            eventState.Time = this.Time;
            return eventState;
        }

        protected override void Operation() //processor will preserve its status such as busy, block or available
        {
            //unitload will stay till repair, we do not scrap it or look for other stations or processors to process it. 
            Station station = (Station)this.processor.Parent;

            this.processor.IsBroken = true;
            this.processor.RepairTime = this.Time + this.processor.Repair.GenerateValue();
            this.processor.EstimatedRepairTime = this.Time + this.processor.Repair.ExpectedValue();
            this.Manager.EventCalendar.ScheduleProcessorRepairEvent(this.processor.RepairTime, this.processor);

        }

        protected override void TraceEvent()
        {
            Debug.WriteLine(String.Format("PROCESSORBREAKDOWN [{0}, {1}]", this.Time, this.processor.Name));
        }
    }

    [XmlType("ProcessorBreakdownEventState")]
    public class ProcessorBreakdownEventState : EventState
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
            return new ProcessorBreakdownEvent(this.Time, managerIn, processCells[this.cell].Processors[this.processor]);
        }
    }
}
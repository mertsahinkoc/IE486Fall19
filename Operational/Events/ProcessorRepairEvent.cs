using System;
using System.Diagnostics;
using System.Xml.Serialization;
using FLOW.NET.Layout;
using FLOW.NET.Random;

namespace FLOW.NET.Operational.Events
{
    public class ProcessorRepairEvent : Event
    {
        private Processor processor;

        public ProcessorRepairEvent()
        {
        }

        public ProcessorRepairEvent(double timeIn, SimulationManager managerIn, Processor processorIn)
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
            ProcessorRepairEventState eventState = new ProcessorRepairEventState();
            eventState.Cell = this.processor.Parent.Name;
            eventState.Processor = this.processor.Name;
            eventState.Time = this.Time;
            return eventState;
        }

        protected override void Operation()
        {
            Station station = (Station)this.processor.Parent;
            this.processor.IsBroken = false;
            if (this.processor.BreakdownType == RVGeneratorType.BusyTime)
            {
                this.processor.NextBreakdown = this.processor.Breakdown.GenerateValue();
            }
            else
            {
                this.Manager.EventCalendar.ScheduleProcessorBreakdownEvent(this.Time + this.processor.Breakdown.GenerateValue(), this.processor);
            }
            this.processor.RepairTime = Double.PositiveInfinity;
            this.processor.EstimatedRepairTime = Double.PositiveInfinity;
            Unitload unitload = (Unitload)this.processor.Content[0];
            if (processor.IsBusy == true && unitload.Operation != null) //if there was a unitload on the processor when it was brokendown. 
            {
                //forget previous process on the unitload, generate new process time. 
                double processTime = station.CalculateProcessTime(unitload);
                this.Manager.EventCalendar.ScheduleEndProcessEvent(this.Manager.Time + processTime, processor);
            }
            else
            {
                this.Manager.TriggerStationControllerAlgorithm(station);
            }
        }

        protected override void TraceEvent()
        {
            Debug.WriteLine(String.Format("PROCESSORREPAIR [{0}, {1}]", this.Time, this.processor.Name));
        }
    }

    [XmlType("ProcessorRepairEventState")]
    public class ProcessorRepairEventState : EventState
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
            return new ProcessorRepairEvent(this.Time, managerIn, processCells[this.cell].Processors[this.processor]);
        }
    }
}
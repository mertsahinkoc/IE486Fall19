using System;
using System.Xml.Serialization;

namespace FLOW.NET.Operational.Events
{
    public delegate void EventPostOperation();
    public delegate void EventPreOperation();

    public abstract class Event
    {
        private SimulationManager manager;

        private double time;

        public Event()
        {
        }

        public Event(double timeIn, SimulationManager managerIn)
        {
            this.time = timeIn;
            this.manager = managerIn;
        }

        public virtual bool IsPermanent
        {
            get { return false; }
        }

        public SimulationManager Manager
        {
            get { return this.manager; }
            set { this.manager = value; }
        }

        public double Time
        {
            get { return this.time; }
            set { this.time = value; }
        }

        public void Execute()
        {
            this.TraceEvent();
            this.Operation();
        }

        public abstract EventState GetEventState();

        protected abstract void Operation();

        protected abstract void TraceEvent();
    }

    [XmlType("EventState")]
    [XmlInclude(typeof(ArrivalEventState))]
    [XmlInclude(typeof(DepartureEventState))]
    [XmlInclude(typeof(EndPlanningPeriodEventState))]
    [XmlInclude(typeof(EndProcessEventState))]
    [XmlInclude(typeof(EndSimulationEventState))]
    [XmlInclude(typeof(EndWarmupEventState))]
    [XmlInclude(typeof(ProcessorBreakdownEventState))]
    [XmlInclude(typeof(ProcessorRepairEventState))]
    [XmlInclude(typeof(StartPlanningPeriodEventState))]
    public abstract class EventState
    {
        private double time;

        [XmlElement("Time")]
        public double Time
        {
            get { return this.time; }
            set { this.time = value; }
        }

        public abstract Event GetEvent(SimulationManager managerIn);
    }
}
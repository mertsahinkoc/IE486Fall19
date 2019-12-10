using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace FLOW.NET.Operational.Events
{
    public class EndWarmupEvent : Event
    {
        public EndWarmupEvent()
        {
        }

        public EndWarmupEvent(double timeIn, SimulationManager managerIn)
            : base(timeIn, managerIn)
        {
        }

        public override EventState GetEventState()
        {
            EndWarmupEventState eventState = new EndWarmupEventState();
            eventState.Time = this.Time;
            return eventState;
        }

        protected override void Operation()
        {
            this.Manager.ClearStatistics(this.Time);
        }

        protected override void TraceEvent()
        {
            Debug.WriteLine(String.Format("ENDWARMUP [{0}]", this.Time));
        }
    }

    [XmlType("EndWarmupEventState")]
    public class EndWarmupEventState : EventState
    {
        public override Event GetEvent(SimulationManager managerIn)
        {
            return new EndWarmupEvent(this.Time, managerIn);
        }
    }
}
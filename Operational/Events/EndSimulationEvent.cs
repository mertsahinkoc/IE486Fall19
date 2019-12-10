using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace FLOW.NET.Operational.Events
{
    public class EndSimulationEvent : Event
    {
        public EndSimulationEvent()
        {
        }

        public EndSimulationEvent(double timeIn, SimulationManager managerIn)
            : base(timeIn, managerIn)
        {
        }

        public override bool IsPermanent
        {
            get { return true; }
        }

        public override EventState GetEventState()
        {
            EndSimulationEventState eventState = new EndSimulationEventState();
            eventState.Time = this.Time;
            return eventState;
        }

        protected override void Operation()
        {
            this.Manager.EventCalendar.Reset();
            this.Manager.EventCalendar.IsPassive = true;
        }

        protected override void TraceEvent()
        {
            Debug.WriteLine(String.Format("ENDSIMULATION [{0}]", this.Time));
        }
    }

    [XmlType("EndSimulationEventState")]
    public class EndSimulationEventState : EventState
    {
        public override Event GetEvent(SimulationManager managerIn)
        {
            return new EndSimulationEvent(this.Time, managerIn);
        }
    }
}
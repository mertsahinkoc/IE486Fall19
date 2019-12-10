using System;
using System.IO;
using System.Diagnostics;
using System.Xml.Serialization;

namespace FLOW.NET.Operational.Events
{
    public class EndPlanningPeriodEvent : Event
    {
        public EndPlanningPeriodEvent()
        {
        }

        public EndPlanningPeriodEvent(double timeIn, SimulationManager managerIn)
            : base(timeIn, managerIn)
        {
        }

        public override bool IsPermanent
        {
            get { return true; }
        }

        public override EventState GetEventState()
        {
            EndPlanningPeriodEventState eventState = new EndPlanningPeriodEventState();
            eventState.Time = this.Time;
            return eventState;
        }

        protected override void Operation()
        {
            JobManager jobManager = this.Manager.JobManager;
            LayoutManager layoutManager = this.Manager.LayoutManager;
            this.Manager.EventCalendar.Reset();
            jobManager.Reset(this.Time);
            layoutManager.Reset(this.Time);
            ConfigurationParameter configuration = this.Manager.Parameter.Configuration;
            configuration.PeriodState.PreOperation();
            configuration.PeriodState.PostOperation();
            this.Manager.EventCalendar.ScheduleStartPlanningPeriodEvent(this.Time);
        }

        protected override void TraceEvent()
        {
            Debug.WriteLine(String.Format("ENDPLANNINGPERIOD [{0}]", this.Time));
        }
    }

    [XmlType("EndPlanningPeriodEventState")]
    public class EndPlanningPeriodEventState : EventState
    {
        public override Event GetEvent(SimulationManager managerIn)
        {
            return new EndPlanningPeriodEvent(this.Time, managerIn);
        }
    }
}
using System;
using System.IO;
using System.Diagnostics;
using System.Xml.Serialization;

namespace FLOW.NET.Operational.Events
{
    public class StartPlanningPeriodEvent : Event
    {
        public StartPlanningPeriodEvent()
        {
        }

        public StartPlanningPeriodEvent(double timeIn, SimulationManager managerIn)
            : base(timeIn, managerIn)
        {
        }

        public override EventState GetEventState()
        {
            StartPlanningPeriodEventState eventState = new StartPlanningPeriodEventState();
            eventState.Time = this.Time;
            return eventState;
        }

        protected override void Operation()
        {
            ConfigurationParameter configuration = this.Manager.Parameter.Configuration;
            JobManager jobManager = this.Manager.JobManager;
            this.Manager.ClearStatistics(this.Time);
            configuration.JobRelease.PreOperation();
            jobManager.ReadReleaseInformation(configuration.JobRelease.GetStream(), this.Time);
            configuration.JobRelease.PostOperation();
            if (configuration.SimulationPeriodType == SimulationPeriodType.JobBased)
            {
                if (jobManager.JobMix.Unreleased == 0 && jobManager.Jobs.Count == 0)
                {
                    this.Manager.EventCalendar.ScheduleEndSimulationEvent(this.Time);
                    return;
                }
            }
            if(this.Manager.Time != 0)  //if (configuration.LoadingPeriodType == LoadingPeriodType.JobBased && this.Manager.Time != 0)
            {                           //Assumption: planning period loading periodun tam katý deðilse, burada loading schedule edip override ediyoruz.
                //IE486fall18 fonksiyon loading period olmadigindan dolayi mundar oldu. (rip)
                //this.Manager.EventCalendar.ScheduleStartLoadingPeriodEvent(this.Manager.Time);
            }
            if (configuration.PlanningPeriodType == PlanningPeriodType.TimeBased)
            {
                this.Manager.EventCalendar.ScheduleEndPlanningPeriodEvent(this.Time + configuration.PlanningPeriodTime);
            }
        }

        protected override void TraceEvent()
        {
            Debug.WriteLine(String.Format("STARTPLANNINGPERIOD [{0}]", this.Time));
        }
    }

    [XmlType("StartPlanningPeriodEventState")]
    public class StartPlanningPeriodEventState : EventState
    {
        public override Event GetEvent(SimulationManager managerIn)
        {
            return new StartPlanningPeriodEvent(this.Time, managerIn);
        }
    }
}
using System;
using System.Diagnostics;
using System.Xml.Serialization;
using FLOW.NET.Layout;

namespace FLOW.NET.Operational.Events
{
    public class DepartureEvent : Event
    {
        private Job job;

        public DepartureEvent()
        {
        }

        public DepartureEvent(double timeIn, SimulationManager managerIn, Job jobIn)
            : base(timeIn, managerIn)
        {
            this.job = jobIn;
        }

        public Job Job
        {
            get { return this.job; }
            set { this.job = value; }
        }

        public override EventState GetEventState()
        {
            DepartureEventState eventState = new DepartureEventState();
            eventState.Job = this.job.Name;
            eventState.Time = this.Time;
            return eventState;
        }

        protected override void Operation()
        {
            JobType jobType = this.job.JobType;
            JobManager jobManager = this.Manager.JobManager;
            jobManager.JobDeparture(this.Time, this.job);
            ConfigurationParameter configuration = this.Manager.Parameter.Configuration;
            if (configuration.SimulationPeriodType == SimulationPeriodType.JobBased)
            {
                if (jobManager.Jobs.Count == 0)
                {
                    this.Manager.EventCalendar.ScheduleEndSimulationEvent(this.Time);
                }
            }
        }

        protected override void TraceEvent()
        {
            Debug.WriteLine(String.Format("DEPARTURE [{0}, {1}]", this.Time, this.job.Name));
        }
    }

    [XmlType("DepartureEventState")]
    public class DepartureEventState : EventState
    {
        private string job;

        [XmlElement("Job")]
        public string Job
        {
            get { return this.job; }
            set { this.job = value; }
        }

        public override Event GetEvent(SimulationManager managerIn)
        {
            JobList jobs = managerIn.JobManager.Jobs;
            return new DepartureEvent(this.Time, managerIn, jobs[this.job]);
        }
    }
}
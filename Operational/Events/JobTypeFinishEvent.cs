using System;
using System.Diagnostics;
using System.Xml.Serialization;
using FLOW.NET.Layout;

namespace FLOW.NET.Operational.Events
{
    public class JobTypeFinishEvent : Event
    {
        private JobType jobType;

        public JobTypeFinishEvent()
        {
        }

        public JobTypeFinishEvent(double timeIn, SimulationManager managerIn, JobType jobTypeIn)
            : base(timeIn, managerIn)
        {
            this.jobType = jobTypeIn;
        }

        public override EventState GetEventState()
        {
            JobTypeFinishEventState eventState = new JobTypeFinishEventState();
            eventState.JobType = this.jobType.Name;
            eventState.Time = this.Time;
            return eventState;
        }

        protected override void Operation()
        {
            ConfigurationParameter configuration = this.Manager.Parameter.Configuration;
            JobManager jobManager = this.Manager.JobManager;
        }

        protected override void TraceEvent()
        {
            Debug.WriteLine(String.Format("RESPONDJOBTYPEFINISH [{0}]", this.Time));
        }
    }

    public class JobTypeFinishEventState : EventState
    {
        private string jobType;

        [XmlElement("JobType")]
        public string JobType
        {
            get { return this.jobType; }
            set { this.jobType = value; }
        }

        public override Event GetEvent(SimulationManager managerIn)
        {
            JobTypeList jobTypes = managerIn.JobManager.JobMix.JobTypes;
            return new JobTypeFinishEvent(this.Time, managerIn, jobTypes[this.jobType]);
        }
    }
}
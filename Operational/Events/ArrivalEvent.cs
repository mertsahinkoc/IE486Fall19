using System;
using System.Diagnostics;
using System.Xml.Serialization;
using System.IO;

namespace FLOW.NET.Operational.Events
{
    public class ArrivalEvent : Event
    {
        private JobType jobType;

        public ArrivalEvent()
        {
        }

        public ArrivalEvent(double timeIn, SimulationManager managerIn, JobType jobTypeIn)
            : base(timeIn, managerIn)
        {
            this.jobType = jobTypeIn;
        }

        public JobType JobType
        {
            get { return this.jobType; }
            set { this.jobType = value; }
        }

        public override EventState GetEventState()
        {
            ArrivalEventState eventState = new ArrivalEventState();
            eventState.JobType = this.jobType.Name;
            eventState.Time = this.Time;
            return eventState;
        }

        protected override void Operation()
        {
            this.Manager.JobManager.JobArrival(this.Time, this.jobType);
            ConfigurationParameter configuration = this.Manager.Parameter.Configuration;
            if (configuration.JobArrivalType == JobArrivalType.DistributionBased)
            {
                double arrivalTime = this.Time + this.jobType.Arrival.GenerateValue();
                if (arrivalTime <= configuration.FinalArrivalTime)
                {
                    this.Manager.EventCalendar.ScheduleArrivalEvent(arrivalTime, this.jobType);
                    TextWriter writer = new StreamWriter("arrived.xml", true);
                    writer.WriteLine("<Arrival>");
                    writer.WriteLine("<Time>" + arrivalTime.ToString() + "</Time>");
                    writer.WriteLine("<Type>" + jobType.Name + "</Type>");
                    writer.WriteLine("<Batch>1</Batch>");
                    writer.WriteLine("</Arrival>");
                    writer.Close();
                }
            }
        }

        protected override void TraceEvent()
        {
        }

        public static object[] GetJobArrivalTypesWithoutHOP()
        {
            return new object[] {
            "DistributionBased",
            "TraceBased"};
        }

        public static object[] GetJobArrivalTypesWithHOP()
        {
            return new object[] {
            "HOPBased"};
        }
    }

    [XmlType("ArrivalEventState")]
    public class ArrivalEventState : EventState
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
           return new ArrivalEvent(this.Time, managerIn, jobTypes[this.jobType]);
        }

        
    }

    

}
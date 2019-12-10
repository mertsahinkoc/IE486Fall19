using System;
using System.Xml.Serialization;

namespace FLOW.NET.Operational
{
    [XmlType("JobMix")]
    public class JobMix : FLOWObject
    {
        private double estimatedTime; //state

        private JobTypeList jobTypes; //state

        private string path;

        public JobMix(string nameIn, FLOWObject parentIn)
            : base(nameIn, parentIn)
        {
            this.jobTypes = new JobTypeList();
        }

        public JobMix()
        {
            this.jobTypes = new JobTypeList();
        }

        [XmlIgnore()]
        public int Buffered
        {
            get
            {
                int sum = 0;
                foreach (JobType jobType in this.jobTypes)
                {
                    sum += jobType.Buffered;
                }
                return sum;
            }
        }

        [XmlElement("EstimatedTime")]
        public double EstimatedTime
        {
            get { return this.estimatedTime; }
            set { this.estimatedTime = value; }
        }

        [XmlArray("JobTypes")]
        [XmlArrayItem(typeof(JobType))]
        public JobTypeList JobTypes
        {
            get { return this.jobTypes; }
            set { this.jobTypes = value; }
        }

        [XmlIgnore()]
        public string Path
        {
            get { return this.path; }
            set { this.path = value; }
        }

        [XmlIgnore()]
        public int Unreleased
        {
            get
            {
                int sum = 0;
                foreach (JobType jobType in this.jobTypes)
                {
                    sum += jobType.Unreleased;
                }
                return sum;
            }
        }

        [XmlIgnore()]
        public int WIP
        {
            get
            {
                int sum = 0;
                foreach (JobType jobType in this.jobTypes)
                {
                    sum += jobType.WIP;
                }
                return sum;
            }
        }

        public override void RelateObjects(FLOWObject parentIn)
        {
            this.Parent = parentIn;
            foreach (JobType jobType in this.jobTypes)
            {
                jobType.RelateObjects(this);
            }
        }
    }

    [XmlType("JobMixState")]
    public class JobMixState : FLOWObjectState
    {
        private int buffered;

        private double estimatedTime;

        private JobTypeStateList jobTypes;

        private int unreleased;

        private int wip;

        public JobMixState()
        {
            this.jobTypes = new JobTypeStateList();
        }

        [XmlElement("Buffered")]
        public int Buffered
        {
            get { return this.buffered; }
            set { this.buffered = value; }
        }

        [XmlElement("EstimatedTime")]
        public double EstimatedTime
        {
            get { return this.estimatedTime; }
            set { this.estimatedTime = value; }
        }

        [XmlArray("JobTypes")]
        [XmlArrayItem(typeof(JobTypeState))]
        public JobTypeStateList JobTypes
        {
            get { return this.jobTypes; }
            set { this.jobTypes = value; }
        }

        [XmlElement("Unreleased")]
        public int Unreleased
        {
            get { return this.unreleased; }
            set { this.unreleased = value; }
        }

        [XmlElement("WIP")]
        public int WIP
        {
            get { return this.wip; }
            set { this.wip = value; }
        }

        public void GetState(JobMix jobMixIn)
        {
            base.GetState(jobMixIn);
            this.buffered = jobMixIn.Buffered;
            this.estimatedTime = jobMixIn.EstimatedTime;
            foreach (JobType jobType in jobMixIn.JobTypes)
            {
                JobTypeState jobTypeState = new JobTypeState();
                jobTypeState.GetState(jobType);
                this.jobTypes.Add(jobTypeState);
            }
            this.unreleased = jobMixIn.Unreleased;
            this.wip = jobMixIn.WIP;
        }

        public void SetState(JobMix jobMixIn, SimulationManager managerIn)
        {
            jobMixIn.EstimatedTime = this.estimatedTime;
            foreach (JobTypeState jobTypeState in this.jobTypes)
            {
                jobTypeState.SetState(jobMixIn.JobTypes[jobTypeState.Name], managerIn);
            }
            base.SetState(jobMixIn);
        }
    }
}
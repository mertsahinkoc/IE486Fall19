using System;
using System.Xml.Serialization;

namespace FLOW.NET.Operational
{
    public class Job : FLOWObject
    {
        private double arrivalTime; //state

        private int batchSize; //state

        private UnitloadList completed; //state

        private double dueDate; //state

        private JobType jobType; //state

        private double startTime; //state

        private UnitloadList unitloads; //state

        public Job()
        {
            this.completed = new UnitloadList();
            this.unitloads = new UnitloadList();
        }

        public Job(string nameIn, FLOWObject parentIn, double arrivalTimeIn, int batchSizeIn, double dueDateIn, JobType jobTypeIn)
            : base(nameIn, parentIn)
        {
            this.arrivalTime = arrivalTimeIn;
            this.batchSize = batchSizeIn;
            this.dueDate = dueDateIn;
            this.jobType = jobTypeIn;
            this.startTime = Double.PositiveInfinity;
            this.completed = new UnitloadList();
            this.unitloads = new UnitloadList();
            for (int i = 1; i <= batchSizeIn; i++)
            {
                Unitload current = new Unitload(String.Format("{0}-Unitload{1}", nameIn, i), this);
                current.Alternates = jobTypeIn.Alternates.Clone();
                this.unitloads.Add(current);
            }
        }

        public double ArrivalTime
        {
            get { return this.arrivalTime; }
            set { this.arrivalTime = value; }
        }

        public int BatchSize
        {
            get { return this.batchSize; }
            set { this.batchSize = value; }
        }

        public UnitloadList Completed
        {
            get { return this.completed; }
            set { this.completed = value; }
        }

        public double DueDate
        {
            get { return this.dueDate; }
            set { this.dueDate = value; }
        }

        public bool IsCompleted
        {
            get { return (this.unitloads.Count == 0); }
        }

        public JobType JobType
        {
            get { return this.jobType; }
            set { this.jobType = value; }
        }

        public double StartTime
        {
            get { return this.startTime; }
            set { this.startTime = value; }
        }

        public UnitloadList Unitloads
        {
            get { return this.unitloads; }
            set { this.unitloads = value; }
        }

        public override void RelateObjects(FLOWObject parentIn)
        {
            this.Parent = parentIn;
            foreach (Unitload current in this.unitloads)
            {
                current.RelateObjects(this);
            }
        }
    }

    [XmlType("JobState")]
    public class JobState : FLOWObjectState
    {
        private double arrivalTime;

        private int batchSize;

        private StringList completed;

        private double dueDate;

        private string jobType;

        private double startTime;

        private StringList unitloads;

        public JobState()
        {
            this.completed = new StringList();
            this.unitloads = new StringList();
        }

        [XmlElement("ArrivalTime")]
        public double ArrivalTime
        {
            get { return this.arrivalTime; }
            set { this.arrivalTime = value; }
        }

        [XmlElement("BatchSize")]
        public int BatchSize
        {
            get { return this.batchSize; }
            set { this.batchSize = value; }
        }

        [XmlArray("Completed")]
        [XmlArrayItem(typeof(string))]
        public StringList Completed
        {
            get { return this.completed; }
            set { this.completed = value; }
        }

        [XmlElement("DueDate")]
        public double DueDate
        {
            get { return this.dueDate; }
            set { this.dueDate = value; }
        }

        [XmlElement("JobType")]
        public string JobType
        {
            get { return this.jobType; }
            set { this.jobType = value; }
        }

        [XmlElement("StartTime")]
        public double StartTime
        {
            get { return this.startTime; }
            set { this.startTime = value; }
        }

        [XmlArray("Unitloads")]
        [XmlArrayItem(typeof(string))]
        public StringList Unitloads
        {
            get { return this.unitloads; }
            set { this.unitloads = value; }
        }

        public void GetState(Job jobIn)
        {
            base.GetState(jobIn);
            this.arrivalTime = jobIn.ArrivalTime;
            this.batchSize = jobIn.BatchSize;
            foreach (Unitload unitload in jobIn.Completed)
            {
                this.completed.Add(unitload.Name);
            }
            this.dueDate = jobIn.DueDate;
            this.jobType = jobIn.JobType.Name;
            this.startTime = jobIn.StartTime;
            foreach (Unitload unitload in jobIn.Unitloads)
            {
                this.unitloads.Add(unitload.Name);
            }
        }

        public void SetState(Job jobIn, SimulationManager managerIn)
        {
            JobManager jobManager = managerIn.JobManager;
            jobIn.Completed.Clear();
            foreach (string unitload in this.completed)
            {
                jobIn.Unitloads.Remove(jobManager.Unitloads[unitload]);
                jobIn.Completed.Add(jobManager.Unitloads[unitload]);
            }
            jobIn.StartTime = this.startTime;
            base.SetState(jobIn);
        }
    }
}
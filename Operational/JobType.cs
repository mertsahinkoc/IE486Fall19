using System;
using System.IO;
using System.Xml.Serialization;
using FLOW.NET.Layout;
using FLOW.NET.Random;

namespace FLOW.NET.Operational
{
    [XmlType("JobType")]
    public class JobType : FLOWObject
    {
        private JobRouteList alternates;

        private RVGenerator arrival; //state

        private RVGenerator batch; //state

        private double betaDeviation; // state

        private double betaMean; // state

        private int buffered; // buffered

        private RVGenerator dueDateTightness;

        private Station inputStation;

        private string inputStationName;

        private JobList jobs; //state 

        private Station outputStation;

        private string outputStationName;

        private int unreleased; //state

		private double weightParameter;

        public JobType(string nameIn, FLOWObject parentIn, RVGenerator arrivalIn, RVGenerator batchIn, RVGenerator dueDateTightnessIn, Station inputStationIn, Station outputStationIn)
            : base(nameIn, parentIn)
        {
            this.arrival = arrivalIn;
            this.batch = batchIn;
            this.dueDateTightness = dueDateTightnessIn;
            this.inputStation = inputStationIn;
            this.outputStation = outputStationIn;
            this.alternates = new JobRouteList();
            this.jobs = new JobList();
			this.weightParameter = 1;
        }

        public JobType()
        {
            this.alternates = new JobRouteList();
            this.jobs = new JobList();
            this.CreateStatistics();
			this.weightParameter = 1;
        }

        [XmlArray("Alternates")]
        [XmlArrayItem(typeof(JobRoute))]
        public JobRouteList Alternates
        {
            get { return this.alternates; }
            set { this.alternates = value; }
        }

        [XmlElement("Arrival")]
        public RVGenerator Arrival
        {
            get { return this.arrival; }
            set { this.arrival = value; }
        }

        [XmlElement("Batch")]
        public RVGenerator Batch
        {
            get { return this.batch; }
            set { this.batch = value; }
        }

        [XmlIgnore()]
        public double BetaDeviation
        {
            get { return this.betaDeviation; }
            set { this.betaDeviation = value; }
        }

        [XmlIgnore()]
        public double BetaMean
        {
            get { return this.betaMean; }
            set { this.betaMean = value; }
        }

        [XmlIgnore()]
        public int Buffered
        {
            get { return this.buffered; }
            set { this.buffered = value; }
        }

        [XmlElement("DueDateTightness")]
        public RVGenerator DueDateTightness
        {
            get { return this.dueDateTightness; }
            set { this.dueDateTightness = value; }
        }

        [XmlIgnore()]
        public Station InputStation
        {
            get { return this.inputStation; }
            set
            {
                this.inputStation = value;
                if (value != null)
                {
                    this.inputStationName = value.Name;
                }
                else
                {
                    this.inputStationName = String.Empty;
                }
            }
        }

        [XmlElement("InputStationName")]
        public string InputStationName
        {
            get { return this.inputStationName; }
            set { this.inputStationName = value; }
        }

        [XmlIgnore()]
        public JobList Jobs
        {
            get { return this.jobs; }
            set { this.jobs = value; }
        }

        [XmlIgnore()]
        public Station OutputStation
        {
            get { return this.outputStation; }
            set
            {
                this.outputStation = value;
                if (value != null)
                {
                    this.outputStationName = value.Name;
                }
                else
                {
                    this.outputStationName = String.Empty;
                }
            }
        }

        [XmlElement("OutputStationName")]
        public string OutputStationName
        {
            get { return this.outputStationName; }
            set { this.outputStationName = value; }
        }

        [XmlIgnore()]
        public int Unreleased
        {
            get { return this.unreleased; }
            set { this.unreleased = value; }
        }

        [XmlIgnore()]
        public int WIP
        {
            get { return this.jobs.Count - this.buffered; }
        }

		[XmlIgnore()]
		public double WeightParameter
		{
			get { return this.weightParameter; }
			set { this.weightParameter = value; }
		}

        public void ClearStatistics(double timeIn)
        {
            Statistics totalTime = this.Statistics["TotalTime"];
            totalTime.Clear(timeIn, 0);
            Statistics shopTime = this.Statistics["ShopTime"];
            shopTime.Clear(timeIn, 0);
            Statistics completed = this.Statistics["Completed"];
            completed.Clear(timeIn, 0);
            Statistics submitted = this.Statistics["Submitted"];
            submitted.Clear(timeIn, 0);
            Statistics earliness = this.Statistics["Earliness"];
			earliness.Clear(timeIn, 0);
			Statistics tardiness = this.Statistics["Tardiness"];
			tardiness.Clear(timeIn, 0);
			Statistics lateness = this.Statistics["Lateness"];
			lateness.Clear(timeIn, 0);
			Statistics numberOfTardyJobs = this.Statistics["NumberofTardyJobs"];
			numberOfTardyJobs.Clear(timeIn, 0);
			foreach (JobRoute alternate in alternates)
			{
				alternate.clearStatistics(timeIn);
			}
        }

        public Job CreateJob(double timeIn, JobArrivalType arrivalTypeIn)
        {
            int batchSize = 0;
            double dueDate = 0;
            JobManager manager = ((JobManager)((JobMix)this.Parent).Parent);
            if (arrivalTypeIn == JobArrivalType.TraceBased)
            {
                batchSize = manager.BatchList[0];
                manager.BatchList.RemoveAt(0);
				dueDate = manager.GenerateDueDate(this);
            }
            else
            {
                batchSize = (int)this.batch.GenerateValue();
                dueDate = manager.GenerateDueDate(this);
            }
            Statistics submitted = this.Statistics["Submitted"];
            submitted.UpdateCount(timeIn, +1);
            Job job = new Job(String.Format("{0}{1} ({2})", this.Name, submitted.Count, Guid.NewGuid()), this.Parent, timeIn, batchSize, dueDate, this);
            this.jobs.Add(job);
            return job;

        }

        public void CreateStatistics()
        {
            this.Statistics.Add("TotalTime", new Statistics(0));
            this.Statistics.Add("ShopTime", new Statistics(0));
            this.Statistics.Add("Completed", new Statistics(0));
            this.Statistics.Add("Submitted", new Statistics(0));
            this.Statistics.Add("Lateness", new Statistics(0));
            this.Statistics.Add("Tardiness", new Statistics(0));
            this.Statistics.Add("NumberofTardyJobs", new Statistics(0));
            this.Statistics.Add("Earliness", new Statistics(0));
        }

        public void DisposeJob(double timeIn, Job jobIn)
        {
            Statistics completed = this.Statistics["Completed"];
            completed.UpdateCount(timeIn, +1);
            Statistics totalTime = this.Statistics["TotalTime"];
            totalTime.UpdateAverage(timeIn, timeIn - jobIn.ArrivalTime);
            Statistics shopTime = this.Statistics["ShopTime"];
            shopTime.UpdateAverage(timeIn, timeIn - jobIn.StartTime);
            double latenessValue = timeIn - jobIn.DueDate;
            Statistics lateness = this.Statistics["Lateness"];
            lateness.UpdateAverage(timeIn, latenessValue);
            double tardinessValue = Math.Max(0, latenessValue);
            Statistics tardiness = this.Statistics["Tardiness"];
            tardiness.UpdateAverage(timeIn, tardinessValue);
            Statistics numberofTardyJobs = this.Statistics["NumberofTardyJobs"];
            if (tardinessValue > 0)
            {
                numberofTardyJobs.UpdateCount(timeIn, 1);
            }
            Statistics earliness = this.Statistics["Earliness"];
            earliness.UpdateAverage(timeIn, Math.Max(-latenessValue, 0));
            this.jobs.Remove(jobIn);
        }

        public override string GetInformation()
        {
            StringWriter writer = new StringWriter();
            writer.WriteLine(base.GetInformation());
            writer.WriteLine("--------------------");
            foreach (JobRoute currentRoute in this.alternates)
            {
                foreach (string currentName in currentRoute.OperationNames)
                {
                    writer.Write(currentName + "-");
                }
                writer.WriteLine(currentRoute.OperationNames[currentRoute.OperationNames.Count - 1]);
            }
            return writer.ToString().Trim();
        }

        public override void RelateObjects(FLOWObject parentIn)
        {
            this.Parent = parentIn;
            foreach (JobRoute currentRoute in this.alternates)
            {
                currentRoute.RelateObjects(this);
            }
        }
    }

    [XmlType("JobTypeState")]
    public class JobTypeState : FLOWObjectState
    {
        private RVGeneratorState arrival;

        private RVGeneratorState batch;

        private double betaDeviation;

        private int buffered;

        private double betaMean;

        private RVGeneratorState dueDateTightness;

        private StringList jobs;

        private int unreleased;

        private int wip;

        public JobTypeState()
        {
            this.jobs = new StringList();
        }

        [XmlElement("Arrival")]
        public RVGeneratorState Arrival
        {
            get { return this.arrival; }
            set { this.arrival = value; }
        }

        [XmlElement("Batch")]
        public RVGeneratorState Batch
        {
            get { return this.batch; }
            set { this.batch = value; }
        }

        [XmlElement("BetaDeviation")]
        public double BetaDeviation
        {
            get { return this.betaDeviation; }
            set { this.betaDeviation = value; }
        }

        [XmlElement("BetaMean")]
        public double BetaMean
        {
            get { return this.betaMean; }
            set { this.betaMean = value; }
        }

        [XmlElement("Buffered")]
        public int Buffered
        {
            get { return this.buffered; }
            set { this.buffered = value; }
        }

        [XmlElement("DueDateTightness")]
        public RVGeneratorState DueDateTightness
        {
            get { return this.dueDateTightness; }
            set { this.dueDateTightness = value; }
        }

        [XmlArray("Jobs")]
        [XmlArrayItem(typeof(string))]
        public StringList Jobs
        {
            get { return this.jobs; }
            set { this.jobs = value; }
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

        public void GetState(JobType jobTypeIn)
        {
            base.GetState(jobTypeIn);
            this.arrival = jobTypeIn.Arrival.GetRVGeneratorState();
            this.batch = jobTypeIn.Batch.GetRVGeneratorState();
            if (this.dueDateTightness != null)
            {
                this.dueDateTightness = jobTypeIn.DueDateTightness.GetRVGeneratorState();
            }
            this.betaDeviation = jobTypeIn.BetaDeviation;
            this.betaMean = jobTypeIn.BetaMean;
            this.buffered = jobTypeIn.Buffered;
            foreach (Job job in jobTypeIn.Jobs)
            {
                this.jobs.Add(job.Name);
            }
            this.unreleased = jobTypeIn.Unreleased;
            this.wip = jobTypeIn.WIP;
        }

        public void SetState(JobType jobTypeIn, SimulationManager managerIn)
        {
            JobManager jobManager = managerIn.JobManager;
            this.arrival.SetState(jobTypeIn.Arrival);
            this.batch.SetState(jobTypeIn.Batch);
            jobTypeIn.BetaDeviation = this.betaDeviation;
            jobTypeIn.BetaMean = this.betaMean;
            jobTypeIn.Buffered = this.buffered;
            this.dueDateTightness.SetState(jobTypeIn.DueDateTightness);
            jobTypeIn.Jobs.Clear();
            foreach (string job in this.Jobs)
            {
                jobTypeIn.Jobs.Add(jobManager.Jobs[job]);
            }
            jobTypeIn.Unreleased = this.unreleased;
            base.SetState(jobTypeIn);
        }
    }
}
using System;
using System.IO;
using System.Data;
using System.Threading;
using System.Xml.Serialization;
using FLOW.NET.Layout;

namespace FLOW.NET.Operational
{
    public class JobManager : FLOWObject
    {
        private IntegerList batchSizeList; //state

        private DoubleList dueDateList; //state

        private JobMix jobMix; //state

        private JobList jobs; //state,

        private UnitloadList unitloadsToPush; //state ie486f18

        private UnitloadList unitloads; //state

        private UnitloadList unitloadsToDecide; //state

        private UnitloadList unitloadsToRoute; //state

        private UnitloadList blockedUnitloads; //state ie486f18


        public JobManager(string nameIn, FLOWObject parentIn)
            : base(nameIn, parentIn)
        {
            this.batchSizeList = new IntegerList();
            this.dueDateList = new DoubleList();
            this.jobs = new JobList();
            this.unitloads = new UnitloadList();
            this.unitloadsToDecide = new UnitloadList();
            this.unitloadsToRoute = new UnitloadList();
            this.unitloadsToPush = new UnitloadList();
            this.blockedUnitloads = new UnitloadList();
        }

        public IntegerList BatchList
        {
            get { return this.batchSizeList; }
            set { this.batchSizeList = value; }
        }

        public DoubleList DueDateList
        {
            get { return this.dueDateList; }
            set { this.dueDateList = value; }
        }

        public JobMix JobMix
        {
            get { return this.jobMix; }
            set { this.jobMix = value; }
        }

        public UnitloadList BlockedUnitloads
        {
            get { return this.blockedUnitloads; }
            set { this.blockedUnitloads = value; }
        }

        public JobList Jobs
        {
            get { return this.jobs; }
            set { this.jobs = value; }
        }

        public UnitloadList Unitloads
        {
            get { return this.unitloads; }
            set { this.unitloads = value; }
        }

        public UnitloadList UnitloadsToPush
        {
            get { return this.unitloadsToPush; }
            set { this.unitloadsToPush = value; }
        }

        public UnitloadList UnitloadsToDecide
        {
            get { return this.unitloadsToDecide; }
            set { this.unitloadsToDecide = value; }
        }

        public UnitloadList UnitloadsToRoute
        {
            get { return this.unitloadsToRoute; }
            set { this.unitloadsToRoute = value; }
        }

        public double CalculateAverageLeadTime(JobType jobTypeIn)
        {
            double leadTime = 0;
			return leadTime; 
        }

        public void ClearStatistics(double timeIn)
        {
            foreach (JobType jobType in this.jobMix.JobTypes)
            {
                jobType.ClearStatistics(timeIn);
            }
        }

        public void FinalizeStatistics(double timeIn)
        {
        }

        public double GenerateDueDate(JobType jobTypeIn)
        {
            SimulationManager simulationManager = (SimulationManager)this.Parent;
			Statistics inputStationInQueueTime = jobTypeIn.InputStation.InQueue.Statistics["Time"];
			double inputStationInQueueAverageWaitTime = 0;
			if (inputStationInQueueTime.Count != 0)
			{
                inputStationInQueueAverageWaitTime = inputStationInQueueTime.Total / inputStationInQueueTime.Count;
			}
            double dueDate = simulationManager.Time + jobTypeIn.DueDateTightness.GenerateValue() * (this.CalculateAverageLeadTime(jobTypeIn) + inputStationInQueueAverageWaitTime);
            return dueDate;
        }

        public void JobArrival(double timeIn, JobType jobTypeIn)
        {
            Station station = jobTypeIn.InputStation; 
            Job newJob = jobTypeIn.CreateJob(timeIn, ((SimulationManager)this.Parent).Parameter.Configuration.JobArrivalType);
            this.jobs.Add(newJob);
            this.unitloads.AddRange(newJob.Unitloads);
            foreach (Unitload current in newJob.Unitloads)
            {
                station.InQueue.Reserved++;
                station.InQueue.Receive(timeIn, current);
                current.Operation = current.Alternates[0].Operations[0];
                current.Station = station;
            }
            ((SimulationManager)this.Parent).TriggerStationControllerAlgorithm(station);
        }


        public void JobDeparture(double timeIn, Job jobIn)
        {
            jobIn.JobType.DisposeJob(timeIn, jobIn);
            this.jobs.Remove(jobIn);
            foreach (Unitload current in jobIn.Completed)
            {
                this.unitloads.Remove(current);
            }
        }

        public void ReadJobMix(Stream streamIn)
        {
            XmlSerializer reader = new XmlSerializer(typeof(JobMix));
            this.jobMix = (JobMix)reader.Deserialize(streamIn);
            this.RelateObjects(this.Parent);
            streamIn.Close();
        }

        public void ReadReleaseInformation(Stream streamIn, double timeIn)
        {
  

            DataSet releases = new DataSet();
            releases.ReadXml(streamIn);
            if (releases.Tables.Count > 0)
            {
                foreach (DataRow currentRow in releases.Tables[0].Rows)
                {
                    int quantity = Int32.Parse(currentRow["Quantity"].ToString());
                    string typeName = currentRow["Type"].ToString();
                    JobType jobType = this.jobMix.JobTypes[typeName];
                    if (quantity >= 0)
                    {
                        jobType.Unreleased += quantity;
                    }
                    else
                    {
                        if (quantity <= jobType.Unreleased)
                        {
                            jobType.Unreleased -= quantity;
                        }
                        else
                        {
                            quantity -= jobType.Unreleased;
                            jobType.Unreleased = 0;

                            JobList cancellableJobs = new JobList();
                            foreach (Job job in jobType.Jobs)
                            {
                                bool cancel = true;
                                foreach (Unitload unitload in job.Unitloads)
                                {
                                    if ((unitload.Location != jobType.InputStation.InQueue) || (unitload.Location == jobType.InputStation.InQueue && unitload.InTransfer == true))
                                    {
                                        cancel = false;
                                        break;
                                    }
                                }

                                if (cancel == true)
                                {
                                    cancellableJobs.Insert(0, job);
                                }
                            }

                            while (quantity > 0 && cancellableJobs.Count > 0)
                            {
                                Job currentJob = cancellableJobs[0];
                                cancellableJobs.RemoveAt(0);
                                quantity--;

                                foreach (Unitload unitload in currentJob.Unitloads)
                                {
                                    this.unitloads.Remove(unitload);
                                }
                                this.jobs.Remove(currentJob);

                                Statistics submitted = jobType.Statistics["Submitted"];
                                submitted.UpdateCount(timeIn, -1);
                            }
                        }
                    }
                }
            }
            streamIn.Close();
        }

        public override void RelateObjects(FLOWObject parentIn)
        {
            this.Parent = parentIn;
            this.jobMix.RelateObjects(this);
        }

        public void Reset(double timeIn)
        {
            SimulationManager manager = (SimulationManager)this.Parent;
            this.FinalizeStatistics(timeIn);
            this.unitloadsToDecide.Clear();
            this.unitloadsToRoute.Clear();
            foreach (Unitload unitload in this.unitloads)
            {
                if (unitload.Location.GetType().Name != "Processor")
                {
                    unitload.Reset(timeIn);
                }
            }
        }
    }

    [XmlType("JobManagerState")]
    public class JobManagerState : FLOWObjectState
    {
        private IntegerList batchSizeList;

        private DoubleList dueDateList;

        private JobMixState jobMix;

        private JobStateList jobs;

        private UnitloadStateList unitloads;

        private StringList unitloadsToDecide;

        private StringList unitloadsToRoute;

        public JobManagerState()
        {
            this.batchSizeList = new IntegerList();
            this.dueDateList = new DoubleList();
            this.jobMix = new JobMixState();
            this.jobs = new JobStateList();
            this.unitloads = new UnitloadStateList();
            this.unitloadsToDecide = new StringList();
            this.unitloadsToRoute = new StringList();
        }

        [XmlIgnore]
        public IntegerList BatchList
        {
            get { return this.batchSizeList; }
            set { this.batchSizeList = value; }
        }

        [XmlIgnore]
        public DoubleList DueDateList
        {
            get { return this.dueDateList; }
            set { this.dueDateList = value; }
        }

        [XmlElement("JobMix")]
        public JobMixState JobMix
        {
            get { return this.jobMix; }
            set { this.jobMix = value; }
        }

        [XmlArray("Jobs")]
        [XmlArrayItem(typeof(JobState))]
        public JobStateList Jobs
        {
            get { return this.jobs; }
            set { this.jobs = value; }
        }

        [XmlArray("Unitloads")]
        [XmlArrayItem(typeof(UnitloadState))]
        public UnitloadStateList Unitloads
        {
            get { return this.unitloads; }
            set { this.unitloads = value; }
        }

        [XmlArray("UnitloadsToDecide")]
        [XmlArrayItem(typeof(string))]
        public StringList UnitloadsToDecide
        {
            get { return this.unitloadsToDecide; }
            set { this.unitloadsToDecide = value; }
        }

        [XmlArray("UnitloadsToRoute")]
        [XmlArrayItem(typeof(string))]
        public StringList UnitloadsToRoute
        {
            get { return this.unitloadsToRoute; }
            set { this.unitloadsToRoute = value; }
        }

        public void GetState(JobManager jobManagerIn)
        {
            base.GetState(jobManagerIn);
            this.jobMix.GetState(jobManagerIn.JobMix);
            foreach (Job job in jobManagerIn.Jobs)
            {
                JobState jobState = new JobState();
                jobState.GetState(job);
                this.jobs.Add(jobState);
            }
            foreach (Unitload unitload in jobManagerIn.Unitloads)
            {
                UnitloadState unitloadState = new UnitloadState();
                unitloadState.GetState(unitload);
                this.unitloads.Add(unitloadState);
            }
            foreach (Unitload unitload in jobManagerIn.UnitloadsToDecide)
            {
                this.unitloadsToDecide.Add(unitload.Name);
            }
            foreach (Unitload unitload in jobManagerIn.UnitloadsToRoute)
            {
                this.unitloadsToRoute.Add(unitload.Name);
            }
            foreach (int batchSize in jobManagerIn.BatchList)
            {
                this.BatchList.Add(batchSize);
            }
            foreach (double dueDate in jobManagerIn.DueDateList)
            {
                this.DueDateList.Add(dueDate);
            }

        }

        public void SetState(JobManager jobManagerIn, SimulationManager managerIn)
        {
            jobManagerIn.Jobs.Clear();
            jobManagerIn.Unitloads.Clear();
            foreach (JobState jobState in this.jobs)
            {
                Job job = new Job(jobState.Name, jobManagerIn, jobState.ArrivalTime, jobState.BatchSize, jobState.DueDate, jobManagerIn.JobMix.JobTypes[jobState.JobType]);
                jobManagerIn.Jobs.Add(job);
                jobManagerIn.Unitloads.AddRange(job.Unitloads);
                jobState.SetState(job, managerIn);
            }
            foreach (UnitloadState unitloadState in this.unitloads)
            {
                unitloadState.SetState(jobManagerIn.Unitloads[unitloadState.Name], managerIn);
            }
            jobManagerIn.UnitloadsToDecide.Clear();
            foreach (string unitload in this.unitloadsToDecide)
            {
                jobManagerIn.UnitloadsToDecide.Add(jobManagerIn.Unitloads[unitload]);
            }
            jobManagerIn.UnitloadsToRoute.Clear();
            foreach (string unitload in this.unitloadsToRoute)
            {
                jobManagerIn.UnitloadsToRoute.Add(jobManagerIn.Unitloads[unitload]);
            }
            this.jobMix.SetState(jobManagerIn.JobMix, managerIn);
            base.SetState(jobManagerIn);
        }
    }
}
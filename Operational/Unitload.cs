using System;
using System.Xml.Serialization;
using FLOW.NET.Layout;
using FLOW.NET.Random;

namespace FLOW.NET.Operational
{
    public class Unitload : MovableObject
    {
        private JobRouteList alternates;
        private Station station;
        private OperationList completed;
        private Cell destination;
        private Operation operation;
        private double endProcessTime;

        public Unitload()
        {
            this.alternates = new JobRouteList();
            this.completed = new OperationList();
        }

        public Unitload(string nameIn, FLOWObject parentIn)
            : base(nameIn, parentIn)
        {
            this.alternates = new JobRouteList();
            this.completed = new OperationList();
        }

        public JobRouteList Alternates
        {
            get { return this.alternates; }
            set { this.alternates = value; }
        }

        public Station Station
        {
            get { return this.station; }
            set { this.station = value; }
        }

        public OperationList Completed
        {
            get { return this.completed; }
            set { this.completed = value; }
        }

        public Cell Destination
        {
            get { return this.destination; }
            set { this.destination = value; }
        }

        public bool IsCompleted
        {
            get
            {
                if (this.alternates.Count == 1)
                {
                    if (this.alternates[0].Operations.Count == this.completed.Count)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        public double EndProcessTime
        {
            get { return this.endProcessTime; }
            set { this.endProcessTime = value; }
        }

        public Operation Operation
        {
            get { return this.operation; }
            set { this.operation = value; }
        }

        public bool CanContinue(Station cellIn)
        {
            OperationList candidateOperations = this.FindCandidateOperations();
            foreach (Operation operation in candidateOperations)
            {
                foreach (Processor processor in cellIn.Processors)
                {
                    if (processor.IsBroken == false && processor.Operations.Contains(operation) == true)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void ChangeBlocked(double timeIn, bool valueIn)
        {
            this.IsBlocked = valueIn;
            Statistics blocked = this.Statistics["Blocked"];
            if (this.IsBlocked == true)
            {
                blocked.UpdateWeighted(timeIn, 1);
            }
            else
            {
                blocked.UpdateWeighted(timeIn, 0);
            }
        }

        public override void ChangeLocation(double timeIn, FLOWObject locationIn)
        {
            this.Location = locationIn;
            this.EntryTime = timeIn;
            this.InTransfer = false;
        }

        public void CompleteOperation()
        {
            if (this.operation != null)
            {
                for (int i = this.alternates.Count - 1; i >= 0; i--)
                {
                    JobRoute currentRoute = this.alternates[i];
                    if (currentRoute.Operations[this.completed.Count] != this.operation)
                    {
                        this.alternates.RemoveAt(i);
                    }
                }
                this.completed.Add(this.operation);
                this.operation = null;
            }
        }

        public OperationList FindCandidateOperations()
        {
            OperationList selectedOperations = new OperationList();
            return selectedOperations;
        }

        public double GetExpectedProcessTime(Station cellIn)
        {
            if (this.operation != null)
            {
                return this.operation.GetExpectedProcessTime(cellIn);
            }
            else
            {
                return 0;
            }
        }

        public double GetProcessTime()
        { 
            RVGenerator generator = this.operation.OperationTime;
            double a = generator.GenerateValue();
            return a;
        }

        public Operation GetShortestOperation(OperationList operationsIn, Station cellIn)
        {
            Operation selectedOperation = null;
            double shortestTime = Double.PositiveInfinity;           
            return selectedOperation;
        }

        public void Reset(double timeIn)
        {
            this.station = null;
            this.destination = null;
            this.InTransfer = false;
        }
    }

    public class UnitloadState : MovableObjectState
    {
        private JobRouteList alternates;

        private string station;

        private StringList completed;

        private string destination;
        private string operation;

        public UnitloadState()
        {
            this.alternates = new JobRouteList();
            this.completed = new StringList();
        }

        [XmlArray("Alternates")]
        [XmlArrayItem(typeof(JobRoute))]
        public JobRouteList Alternates
        {
            get { return this.alternates; }
            set { this.alternates = value; }
        }

        [XmlElement("Station")]
        public string Station
        {
            get { return this.station; }
            set { this.station = value; }
        }

        [XmlArray("Completed")]
        [XmlArrayItem(typeof(string))]
        public StringList Completed
        {
            get { return this.completed; }
            set { this.completed = value; }
        }

        [XmlElement("Destination")]
        public string Destination
        {
            get { return this.destination; }
            set { this.destination = value; }
        }

        [XmlElement("Operation")]
        public string Operation
        {
            get { return this.operation; }
            set { this.operation = value; }
        }

        public void GetState(Unitload unitloadIn)
        {
            base.GetState(unitloadIn);
            foreach (JobRoute jobRoute in unitloadIn.Alternates)
            {
                this.alternates.Add(jobRoute);
            }
            if (unitloadIn.Station != null)
            {
                this.station = unitloadIn.Station.Name;
            }
            foreach (Operation operation in unitloadIn.Completed)
            {
                this.completed.Add(operation.Name);
            }
            if (unitloadIn.Destination != null)
            {
                this.destination = unitloadIn.Destination.Name;
            }
            if (unitloadIn.Operation != null)
            {
                this.operation = unitloadIn.Operation.Name;
            }
        }

        public void SetState(Unitload unitloadIn, SimulationManager managerIn)
        {
            Layout.Layout layout = managerIn.LayoutManager.Layout;
            unitloadIn.Alternates.Clear();
            foreach (JobRoute jobRoute in this.alternates)
            {
                foreach (string operation in jobRoute.OperationNames)
                {
                    jobRoute.Operations.Add(layout.Operations[operation]);
                }
                unitloadIn.Alternates.Add(jobRoute);
            }
            unitloadIn.Station = layout.Stations[this.station]; 
            unitloadIn.Completed.Clear();
            foreach (string operation in this.completed)
            {
                unitloadIn.Completed.Add(layout.Operations[operation]);
            }
            unitloadIn.Destination = layout.Stations[this.destination];
            unitloadIn.Operation = layout.Operations[this.operation];
            base.SetState(unitloadIn);
        }
    }
}
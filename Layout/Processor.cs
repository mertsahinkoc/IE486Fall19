using System;
using System.Xml.Serialization;
using FLOW.NET.Random;
using FLOW.NET.Operational;

namespace FLOW.NET.Layout
{
    [XmlType("Processor")]
    public class Processor : StaticObject, ICloneable
    {
        private Bin bin; //state
        private bool isBroken; //state
        private BinMagazine binMagazine; //state
        private OperationList operations; //state
        private StringList operationNames;  // ie486

        private double nextBreakdown; //state
        private RVGenerator breakdown; //state
        private RVGeneratorType breakdownType;

        private double repairTime; //state
        private RVGenerator repair; //state
        private double estimatedRepairTime; //state

        public Processor()
        {
            this.operations = new OperationList();
            this.CreateStatistics();
        }

        public Processor(string nameIn, FLOWObject parentIn, int capacityIn, RVGenerator breakdownIn, RVGeneratorType breakdownTypeIn, RVGenerator repairIn)
            : base(nameIn, parentIn, capacityIn)
        {
            this.breakdown = breakdownIn;
            this.breakdownType = breakdownTypeIn;
            this.operations = new OperationList();
            this.repair = repairIn;
        }

        #region GETSET functions
        [XmlElement("Breakdown")]
        public RVGenerator Breakdown
        {
            get { return this.breakdown; }
            set { this.breakdown = value; }
        }

        [XmlElement("BreakdownType")]
        public RVGeneratorType BreakdownType
        {
            get { return this.breakdownType; }
            set { this.breakdownType = value; }
        }

        [XmlIgnore()]
        public double EstimatedRepairTime
        {
            get { return this.estimatedRepairTime; }
            set { this.estimatedRepairTime = value; }
        }

        [XmlIgnore()]
        public new bool IsAvailable
        {
            get { return (this.isBroken == false) && (base.IsAvailable == true); }
        }

        [XmlIgnore()]
        public bool IsBroken
        {
            get { return this.isBroken; }
            set { this.isBroken = value; }
        }

        [XmlIgnore()]
        public BinMagazine BinMagazine
        {
            get { return this.binMagazine; }
            set { this.binMagazine = value; }
        }

        [XmlIgnore()]
        public double NextBreakdown
        {
            get { return this.nextBreakdown; }
            set { this.nextBreakdown = value; }
        }

        [XmlIgnore()]
        public OperationList Operations
        {
            get { return this.operations; }
            set { this.operations = value; }
        }

        [XmlArray("OperationNames")]
        [XmlArrayItem(typeof(string))]
        public StringList OperationNames
        {
            get { return this.operationNames; }
            set { this.operationNames = value; }
        }

        [XmlElement("Repair")]
        public RVGenerator Repair
        {
            get { return this.repair; }
            set { this.repair = value; }
        }

        [XmlElement("RepairTime")]
        public double RepairTime
        {
            get { return this.repairTime; }
            set { this.repairTime = value; }
        }

        #endregion

        public void ClearStatistics(double timeIn)
        {
            Statistics usage = this.Statistics["Usage"];
            usage.Clear(timeIn, this.Content.Count);
            Statistics blocked = this.Statistics["Blocked"];
            if (this.IsBlocked == true)
            {
                blocked.Clear(timeIn, 1);
            }
            else
            {
                blocked.Clear(timeIn, 0);
            }
            Statistics starved = this.Statistics["Starved"];
            if (this.IsStarved == true)
            {
                starved.Clear(timeIn, 1);
            }
            else
            {
                starved.Clear(timeIn, 0);
            }
        }

        public object Clone()
        {
            Processor clone = new Processor(this.Name, this.Parent, this.Capacity, (RVGenerator)this.breakdown.Clone(), this.breakdownType, (RVGenerator)this.repair.Clone());
            return clone;
        }

        public void CreateStatistics()
        {
            this.Statistics.Add("Usage", new Statistics(0));
            this.Statistics.Add("Blocked", new Statistics(0));
            this.Statistics.Add("Starved", new Statistics(0));
        }

        public void FinalizeStatistics(double timeIn)
        {
            Statistics usage = this.Statistics["Usage"];
            usage.UpdateWeighted(timeIn, this.Content.Count);
            Statistics blocked = this.Statistics["Blocked"];
            if (this.IsBlocked == true)
            {
                blocked.UpdateWeighted(timeIn, 1);
            }
            else
            {
                blocked.UpdateWeighted(timeIn, 0);
            }
            Statistics starved = this.Statistics["Starved"];
            if (this.IsBlocked == true)
            {
                starved.UpdateWeighted(timeIn, 1);
            }
            else
            {
                starved.UpdateWeighted(timeIn, 0);
            }
        }
        public void Receive(double timeIn, Unitload unitloadIn)
        {
            this.Content.Add(unitloadIn);
            Statistics usage = this.Statistics["Usage"];
            usage.UpdateWeighted(timeIn, this.Content.Count);
            unitloadIn.ChangeLocation(timeIn, this);
            this.Reserved--;
        }

        public override void RelateObjects(FLOWObject parentIn)
        {
            this.Parent = parentIn;
        }

        public void Release(double timeIn, Bin binIn) // IE486f18 bu fonksiyon magazine için olacak.
        {
            
            this.bin = null;
            Statistics mounted = binIn.Statistics["Mounted"]; 
            mounted.UpdateTotal(timeIn, (timeIn - binIn.EntryTime));
            binIn.Location = null;
        }

        public void Release(double timeIn, Unitload unitloadIn)
        {
            this.Content.Remove(unitloadIn);
            Statistics usage = this.Statistics["Usage"];
            usage.UpdateWeighted(timeIn, this.Content.Count);
            unitloadIn.Location = null;
        }

        public void Reset(double timeIn)
        {
            this.Reserved = 0;
            if (this.IsBusy == true)
            {
                Unitload unitload = (Unitload)this.Content[0];
                unitload.InTransfer = false;
                this.ChangeBlocked(timeIn, false);
                unitload.ChangeBlocked(timeIn, false);
            }
        }

        public void Unload(double timeIn)
        {
            this.operations.Clear();
        }
    }

    [XmlType("ProcessorState")]
    public class ProcessorState : StaticObjectState
    {
        private RVGeneratorState breakdown;

        private double estimatedRepairTime;

        private bool isBroken;

        private BinMagazineState magazine;

        private double nextBreakdown;

        private StringList operations;

        private RVGeneratorState repair;

        private double repairTime;

        private string tool;

        public ProcessorState()
        {
            this.magazine = new BinMagazineState();
            this.operations = new StringList();
        }

        [XmlElement("Breakdown")]
        public RVGeneratorState Breakdown
        {
            get { return this.breakdown; }
            set { this.breakdown = value; }
        }

        [XmlElement("EstimatedRepairTime")]
        public double EstimatedRepairTime
        {
            get { return this.estimatedRepairTime; }
            set { this.estimatedRepairTime = value; }
        }

        [XmlElement("IsBroken")]
        public bool IsBroken
        {
            get { return this.isBroken; }
            set { this.isBroken = value; }
        }

        [XmlElement("BinMagazine")]
        public BinMagazineState Magazine
        {
            get { return this.magazine; }
            set { this.magazine = value; }
        }

        [XmlElement("NextBreakdown")]
        public double NextBreakdown
        {
            get { return this.nextBreakdown; }
            set { this.nextBreakdown = value; }
        }

        [XmlArray("Operations")]
        [XmlArrayItem(typeof(string))]
        public StringList Operations
        {
            get { return this.operations; }
            set { this.operations = value; }
        }

        [XmlElement("Repair")]
        public RVGeneratorState Repair
        {
            get { return this.repair; }
            set { this.repair = value; }
        }

        [XmlElement("RepairTime")]
        public double RepairTime
        {
            get { return this.repairTime; }
            set { this.repairTime = value; }
        }


        public void GetState(Processor processorIn)
        {
            base.GetState(processorIn);
            this.breakdown = processorIn.Breakdown.GetRVGeneratorState();
            this.estimatedRepairTime = processorIn.EstimatedRepairTime;
            this.isBroken = processorIn.IsBroken;
            this.nextBreakdown = processorIn.NextBreakdown;
            foreach (Operation operation in processorIn.Operations)
            {
                this.operations.Add(operation.Name);
            }
            this.repair = processorIn.Repair.GetRVGeneratorState();
            this.repairTime = processorIn.RepairTime;
        }

        public void SetState(Processor processorIn, SimulationManager managerIn)
        {
            Layout layout = managerIn.LayoutManager.Layout;
            JobManager jobManager = managerIn.JobManager;
            this.breakdown.SetState(processorIn.Breakdown);
            processorIn.EstimatedRepairTime = this.estimatedRepairTime;
            processorIn.IsBroken = this.isBroken;
            if (SimulationManagerState.FromExecutionToPlanning == false)
            {
                processorIn.NextBreakdown = this.nextBreakdown;
            }
            else
            {
                if(this.isBroken == true)
                {
                    managerIn.EventCalendar.ScheduleProcessorRepairEvent(this.estimatedRepairTime, processorIn);
                }
            }
            processorIn.Operations.Clear();
            foreach (string operationName in this.operations)
            {
                processorIn.Operations.Add(layout.Operations[operationName]);
            }
            this.repair.SetState(processorIn.Repair);
            processorIn.RepairTime = this.repairTime;
            processorIn.Content.Clear();
            foreach (string unitloadName in this.Content)
            {
                Unitload unitload = jobManager.Unitloads[unitloadName];
                processorIn.Receive(unitload.EntryTime, unitload);
            }
            base.SetState(processorIn);
        }
    }
}

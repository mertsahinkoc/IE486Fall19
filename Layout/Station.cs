using System;
using System.Xml.Serialization;
using FLOW.NET.Random;
using FLOW.NET.Operational;

namespace FLOW.NET.Layout
{
    [XmlType("Station")]
    public class Station : Cell
    {
        private BinMagazine binMagazine; //state
        private OperationList operations; //state
        private ProcessorList processors; //state

        public Station()
        {
            this.operations = new OperationList();
            this.processors = new ProcessorList();
            this.CreateStatistics();
        }

        public Station(string nameIn, FLOWObject parentIn, int capacityIn, RVGenerator transferTimeIn)
            : base(nameIn, parentIn, capacityIn)
        {
            this.TransferTime = transferTimeIn;
            this.operations = new OperationList();
            this.processors = new ProcessorList();
        }



        #region GETSET functions
        [XmlIgnore()]
        public new bool IsAvailable
        {
            get
            {
                foreach (Processor processor in this.processors)
                {
                    if (processor.IsAvailable == true)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        [XmlIgnore()]
        public new bool IsBusy
        {
            get
            {
                foreach (Processor processor in this.processors)
                {
                    if (processor.IsBusy == true)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        [XmlElement("BinMagazine")]
        public BinMagazine BinMagazine
        {
            get { return this.binMagazine; }
            set { this.binMagazine = value; }
        }

        [XmlIgnore()]
        public OperationList Operations
        {
            get { return this.operations; }
            set { this.operations = value; }
        }

        [XmlArray("Processors")]
        [XmlArrayItem(typeof(Processor))]
        public ProcessorList Processors
        {
            get { return this.processors; }
            set { this.processors = value; }
        }
        #endregion

        public double CalculateProcessTime(Unitload unitloadIn)
        {
            double processTime = 0;
            if (unitloadIn.Operation != null)
            {
                processTime = unitloadIn.GetProcessTime();
            }
            return processTime;
        }

        public void StarvationUpdate(UnitloadList unitloadstogo,double timeIn) //IE486 Fall18
        {
            int countAvailable=0, countBlocked=0;
            bool anyProcessorIsStarved = false;

            //starvation update for the processors
            foreach (Processor p in this.processors)
            {
                bool starved = false; //not starved at first
                if (p.IsAvailable == true)
                {
                    countAvailable++;
                    foreach (Unitload unit in unitloadstogo)
                    {
                        foreach (JobRoute currentRoute in unit.Alternates)
                        {
                            Operation operation = currentRoute.Operations[unit.Completed.Count];
                            if (p.Operations.Contains(operation)) //there is a job in inqueue that p can process but cannot take it
                            {
                                starved = true;
                                anyProcessorIsStarved = true;
                                break;
                            }
                        }
                        if (starved == true) { break; } // no need to check other unitloads in unitloadstogo
                    }
                }
                else
                {
                    if (p.IsBlocked == true) countBlocked++;
                }
                if (p.IsStarved != starved) { p.ChangeStarved(timeIn, starved); }
            }
            
            // starvation update for the station
            if ((countBlocked + countAvailable== this.processors.Count)&& anyProcessorIsStarved)
            {
              if(this.IsStarved!=true)  this.ChangeStarved(timeIn, true);
            }
            else
            {
                if (this.IsStarved != false) this.ChangeStarved(timeIn, false);
            }           
        }
        //If all processors are blocked, station is blocked. Otherwise station is not blocked, only processor is blocked.
        public void BlockageUpdate(double timeIn)
        {
            bool wasBlocked = this.IsBlocked;

            bool nowBlocked = true;

            foreach (Processor processor in this.Processors)
            {
                if (processor.IsBlocked == false)
                {
                    nowBlocked = false;
                    break;
                }
            }

            if (wasBlocked != nowBlocked)
            {
                this.ChangeBlocked(timeIn, nowBlocked);
            }
        }
        //this function is for just inputstations(copied from inputcell functions after deleting inputcell)
        public UnitloadList FindUnreleasedUnitloads()
        {
            UnitloadList unitloads = new UnitloadList();
            foreach (Unitload unitload in this.InQueue.Content)
            {
                if (unitload.InTransfer == false)
                {
                    unitloads.Add(unitload);
                }
            }
            return unitloads;
        }
        public override void ClearStatistics(double timeIn)
        {
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
            this.InQueue.ClearStatistics(timeIn);
            foreach (Processor processor in this.processors)
            {
                processor.ClearStatistics(timeIn);
            }
            this.binMagazine.ClearStatistics(timeIn);
        }

        public override void CreateStatistics()
        {
            this.Statistics.Add("Blocked", new Statistics(0));
            this.Statistics.Add("Starved", new Statistics(0));
        }

        public double EarliestCompletion(double timeIn)
        {
            double earliestCompletion = 0;
            foreach (Unitload unitload in this.Content)
            {
                if (unitload.InTransfer == false && unitload.IsBlocked == false)
                {
                    double completion = unitload.EntryTime + unitload.GetExpectedProcessTime(this) - timeIn;
                    if (completion < earliestCompletion)
                    {
                        earliestCompletion = completion;
                    }
                }
            }
            return earliestCompletion;
        }

        public override void FinalizeStatistics(double timeIn)
        {
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
            if (this.IsStarved == true)
            {
                starved.UpdateWeighted(timeIn, 1);
            }
            else
            {
                starved.UpdateWeighted(timeIn, 0);
            }
            this.InQueue.FinalizeStatistics(timeIn);
            foreach (Processor processor in this.processors)
            {
                processor.FinalizeStatistics(timeIn);
            }
           this.binMagazine.FinalizeStatistics(timeIn);
        }

        public Processor GetBlockedProcessor()
        {
            foreach (Processor processor in this.processors)
            {
                if (processor.IsBlocked == true)
                {
                    return processor;
                }
            }
            return null;
        }

        public ProcessorList GetBusyWaitingProcessors()
        {
            ProcessorList processors = new ProcessorList();
            foreach (Processor processor in this.processors)
            {
                if (processor.IsBusy == true && processor.IsBlocked == false)
                {
                    Unitload unitload = (Unitload)processor.Content[0];
                    if (unitload.InTransfer == false && unitload.Operation == null)
                    {
                        processors.Add(processor);
                    }
                }
            }
            return processors;
        }

        public ProcessorList GetFreeProcessors()
        {
            ProcessorList processors = new ProcessorList();
            foreach (Processor processor in this.processors)
            {
                if (processor.IsAvailable == true)
                {
                    processors.Add(processor);
                }
            }
            return processors;
        }

        public override void RelateObjects(FLOWObject parentIn)
        {
            base.RelateObjects(parentIn);
            foreach (Processor processor in this.processors)
            {
                processor.RelateObjects(this);
            }
            this.binMagazine.RelateObjects(this);
        }

        public override void Reset(double timeIn)
        {
            base.Reset(timeIn);
            this.binMagazine.Reset(timeIn);
            foreach (Processor processor in this.processors)
            {
                processor.Reset(timeIn);
            }
        }

        public void Unload(double timeIn)
        {
            this.operations.Clear();
            this.binMagazine.Unload(timeIn);
            foreach (Processor processor in this.processors)
            {
                processor.Unload(timeIn);
            }
        }
    }

    [XmlType("StationState")]
    public class StationState : CellState
    {
        private BinMagazineState magazine;

        private StringList operations;

        private ProcessorStateList processors;

        public StationState()
        {
            this.magazine = new BinMagazineState();
            this.operations = new StringList();
            this.processors = new ProcessorStateList();
        }

        [XmlElement("MagazineState")]
        public BinMagazineState Magazine
        {
            get { return this.magazine; }
            set { this.magazine = value; }
        }

        [XmlArray("Operations")]
        [XmlArrayItem(typeof(string))]
        public StringList Operations
        {
            get { return this.operations; }
            set { this.operations = value; }
        }

        [XmlArray("Processors")]
        [XmlArrayItem(typeof(ProcessorState))]
        public ProcessorStateList Processors
        {
            get { return this.processors; }
            set { this.processors = value; }
        }

        public void GetState(Station stationIn)
        {
            base.GetState(stationIn);
            this.magazine.GetState(stationIn.BinMagazine);
            foreach (Operation operation in stationIn.Operations)
            {
                this.operations.Add(operation.Name);
            }
            foreach (Processor processor in stationIn.Processors)
            {
                ProcessorState processorState = new ProcessorState();
                processorState.GetState(processor);
                this.processors.Add(processorState);
            }
        }

        public void SetState(Station stationIn, SimulationManager managerIn)
        {
            Layout layout = managerIn.LayoutManager.Layout;
            this.magazine.SetState(stationIn.BinMagazine, managerIn);
            stationIn.Operations.Clear();
            foreach (string operationName in this.operations)
            {
                stationIn.Operations.Add(layout.Operations[operationName]);
            }
            foreach (ProcessorState processorState in this.processors)
            {
                processorState.SetState(stationIn.Processors[processorState.Name], managerIn);
            }
            base.SetState(stationIn, managerIn);
        }
    }
}
using System;
using System.Xml.Serialization;
using FLOW.NET.Random;
using FLOW.NET.Operational;

namespace FLOW.NET.Layout
{
    [XmlType("Cell")]
    [XmlInclude(typeof(BufferCell))]
    [XmlInclude(typeof(Station))]
    public abstract class Cell : StaticObject
    {
        private Queue inQueue; //state
        private StringList location;
        private RVGenerator transferTime; //state

        public Cell()
        {
            this.location = new StringList();
        }

        public Cell(string nameIn, FLOWObject parentIn, int capacityIn)
            : base(nameIn, parentIn, capacityIn)
        {
            this.location = new StringList();
        }

        [XmlElement("InQueue")]
        public Queue InQueue
        {
            get { return this.inQueue; }
            set { this.inQueue = value; }
        }

        [XmlArray("Location")]
        [XmlArrayItem(typeof(String))]
        public StringList Location
        {
            get { return this.location; }
            set { this.location = value; }
        }

        [XmlElement("TransferTime", typeof(RVGenerator))]
        public RVGenerator TransferTime
        {
            get { return this.transferTime; }
            set { this.transferTime = value; }
        }

        public virtual void ClearStatistics(double timeIn)
        {
            this.InQueue.ClearStatistics(timeIn);
        }

        public virtual void CreateStatistics()
        {
        }

        public virtual void FinalizeStatistics(double timeIn)
        {
            this.InQueue.FinalizeStatistics(timeIn);
        }

        public override void RelateObjects(FLOWObject parentIn)
        {
            this.Parent = parentIn;
            this.InQueue.RelateObjects(this);
        }

        public virtual void Reset(double timeIn)
        {
            this.inQueue.Reserved = 0;
        }
    }

    [XmlType("CellState")]
    public class CellState : StaticObjectState
    {
        private QueueState inQueue;

        private RVGeneratorState transferTime;

        public CellState()
        {
            this.inQueue = new QueueState();
        }

        [XmlElement("InQueueState")]
        public QueueState InQueue
        {
            get { return this.inQueue; }
            set { this.inQueue = value; }
        }

        [XmlElement("TransferTime")]
        public RVGeneratorState TransferTime
        {
            get { return this.transferTime; }
            set { this.transferTime = value; }
        }

        public void GetState(Cell cellIn)
        {
            base.GetState(cellIn);
            this.inQueue.GetState(cellIn.InQueue);
            this.transferTime = cellIn.TransferTime.GetRVGeneratorState();
        }

        public void SetState(Cell cellIn, SimulationManager managerIn)
        {
            this.inQueue.SetState(cellIn.InQueue, managerIn);
            this.transferTime.SetState(cellIn.TransferTime);
            base.SetState(cellIn);
        }
    }
}
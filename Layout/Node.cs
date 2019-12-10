using System;
using System.IO;
using System.Xml.Serialization;
using FLOW.NET.Operational;

namespace FLOW.NET.Layout
{
    public enum NodeType
    {
        Conveyor,

        Park,

        Path,

        Polygon,

        Transfer,
    }

    [XmlType("Node")]
    public class Node : StaticObject
    {
        //private LaneList ending;

        private Queue inQueue;

        private System.Drawing.Point location;

        private BinMagazine binMagazine;

        private NodeType type;

        public Node()
        {
            this.CreateStatistics();
        }

        public Node(string nameIn, FLOWObject parentIn, int capacityIn, System.Drawing.Point locationIn, NodeType typeIn)
            : base(nameIn, parentIn, capacityIn)
        {
            this.Location = locationIn;
            this.type = typeIn;
        }

        [XmlIgnore()]
        public Queue InQueue
        {
            get { return this.inQueue; }
            set { this.inQueue = value; }
        }

        [XmlIgnore()]
        public BinMagazine BinMagazine
        {
            get { return this.binMagazine; }
            set { this.binMagazine = value; }
        }

        [XmlIgnore()]
        public bool IsCongested
        {
            get
            {
                if (this.Type == NodeType.Conveyor)
                {
                }
                else
                {
                }
                return false;
            }
        }

        [XmlElement("Location")]
        public System.Drawing.Point Location
        {
            get { return this.location; }
            set { this.location = value; }
        }

        [XmlIgnore()]
        public System.Drawing.Rectangle Rectangle
        {
            get { return new System.Drawing.Rectangle(new System.Drawing.Point(this.location.X - 10, this.location.Y - 10), this.Size); }
        }

        [XmlIgnore()]
        public System.Drawing.Size Size
        {
            get { return new System.Drawing.Size(20, 20); }
        }

        //[XmlIgnore()]
        //public LaneList Starting
        //{
        //    get { return this.starting; }
        //    set { this.starting = value; }
        //}

        [XmlElement("Type")]
        public NodeType Type
        {
            get { return this.type; }
            set { this.type = value; }
        }

        public void ClearStatistics(double timeIn)
        {
            Statistics busy = this.Statistics["Busy"];
            if (this.IsBusy == true)
            {
                busy.Clear(timeIn, 1);
            }
            else
            {
                busy.Clear(timeIn, 0);
            }
            Statistics blocked = this.Statistics["Blocked"];
            if (this.IsBlocked == true)
            {
                blocked.Clear(timeIn, 1);
            }
            else
            {
                blocked.Clear(timeIn, 0);
            }
        }

        public void CreateStatistics()
        {
            this.Statistics.Add("Busy", new Statistics(0));
            this.Statistics.Add("Jammed", new Statistics(0));
            this.Statistics.Add("Blocked", new Statistics(0));
        }

        public void FinalizeStatistics(double timeIn)
        {
            Statistics busy = this.Statistics["Busy"];
            if (this.IsBusy == true)
            {
                busy.UpdateWeighted(timeIn, 1);
            }
            else
            {
                busy.UpdateWeighted(timeIn, 0);
            }
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

        public override string GetInformation()
        {
            StringWriter writer = new StringWriter();
            writer.WriteLine(base.GetInformation());
            writer.WriteLine("Type : {0}", this.type);
            writer.WriteLine("Location : {0}", this.location);
            if (this.inQueue != null)
            {
                writer.WriteLine("Cell : {0}", this.inQueue.Parent.Name);
                writer.WriteLine("Input Queue : {0}", this.inQueue.Name);
            }
            return writer.ToString().Trim();
        }

        public void Receive(double timeIn, MovableObject objectIn)
        {
            this.Content.Add(objectIn);
            Statistics busy = this.Statistics["Busy"];
            busy.UpdateWeighted(timeIn, 1);
            objectIn.ChangeLocation(timeIn, this);
        }

        public void Release(double timeIn, MovableObject objectIn)
        {
            this.Content.Remove(objectIn);
            if (this.IsBusy == false)
            {
                Statistics busy = this.Statistics["Busy"];
                busy.UpdateWeighted(timeIn, 0);
            }
            objectIn.Location = null;
        }
    }

    [XmlType("NodeState")]
    public class NodeState : StaticObjectState
    {
        public NodeState()
        {
        }

        public void GetState(Node nodeIn)
        {
            base.GetState(nodeIn);
        }

        public void SetState(Node nodeIn, SimulationManager managerIn)
        {
            Layout layout = managerIn.LayoutManager.Layout;
            nodeIn.Content.Clear();
            base.SetState(nodeIn);
        }

    }
}

using System;
using System.IO;
using System.Xml.Serialization;
using FLOW.NET.Layout;
using FLOW.NET.Random;
using FLOW.NET.IO;

namespace FLOW.NET.Operational
{
    public class LayoutManager : FLOWObject
    {
        private NodeList congestedNodes; //state

        private QueueList queuesToPull; //state ie486f18
        private RequestList requestsToReplenish; // IE486 Fall18

        private StationList stationsToDecide; //state

        private Layout.Layout layout; //state

        public LayoutManager(string nameIn, FLOWObject parentIn)
            : base(nameIn, parentIn)
        {
            this.congestedNodes = new NodeList();
            this.stationsToDecide = new StationList();
            this.queuesToPull = new QueueList(); //ie486f18
            this.requestsToReplenish = new RequestList();
        }

        public StationList StationsToDecide
        {
            get { return this.stationsToDecide; }
            set { this.stationsToDecide = value; }
        }

        public NodeList CongestedNodes
        {
            get { return this.congestedNodes; }
            set { this.congestedNodes = value; }
        }

        public QueueList QueuesToPull
        {
            get { return this.queuesToPull; }
            set { this.queuesToPull = value; }
        }
        public OrderList OrdersToReplenish
        {
            get { return this.ordersToReplenish; }
            set { this.ordersToReplenish = value; }
        }

        public Layout.Layout Layout
        {
            get { return this.layout; }
            set { this.layout = value; }
        }

        public void ClearStatistics(double timeIn)
        {
            this.layout.ClearStatistics(timeIn);
        }

        public void FinalizeStatistics(double timeIn)
        {
            this.layout.FinalizeStatistics(timeIn);
        }

        public void PrepareLayout()
        {
            //IE486fall18
            this.layout.Bins = new BinList();
            foreach (Station currentCell in this.layout.Stations)
            {
                foreach (Processor currentProcessor in currentCell.Processors)
                {
                    if (currentProcessor.BreakdownType == RVGeneratorType.BusyTime)
                    {
                        currentProcessor.NextBreakdown = currentProcessor.Breakdown.GenerateValue();
                    }
                    else
                    {
                        SimulationManager manager = (SimulationManager)this.Parent;
                        manager.EventCalendar.ScheduleProcessorBreakdownEvent(currentProcessor.Breakdown.GenerateValue(), currentProcessor);
                        currentProcessor.NextBreakdown = Double.PositiveInfinity;
                    }
                    currentProcessor.RepairTime = Double.PositiveInfinity;
                    currentProcessor.EstimatedRepairTime = Double.PositiveInfinity;
                }
            }
        }

        public void ReadLayout(Stream streamIn)
        {
            XmlSerializer reader = new XmlSerializer(typeof(Layout.Layout));
            this.layout = (Layout.Layout)reader.Deserialize(streamIn);
            this.RelateObjects(this.Parent);
            this.Layout.OnHandWriter = new TextOutput(((SimulationManager)this.Parent).path);
            this.PrepareLayout();
            streamIn.Close();
        }

        public override void RelateObjects(FLOWObject parentIn)
        {
            this.Parent = parentIn;
            this.layout.RelateObjects(this);
        }
        public void WriteLayout(TextOutput text)
        {
            text.WriteToText(layout.Parent.ToString());
            foreach (Station st in Layout.Stations)
            {
                text.WriteToText("   " + st.ToString());
                foreach (Processor p in st.Processors)
                {
                    text.WriteToText("     " + p.ToString());
                    foreach (Operation op in p.Operations)
                    {
                        text.WriteToText("       " + op.ToString());
                    }
                }
                text.WriteToText("     " + st.InQueue.ToString());
                foreach (Operation op in st.Operations)
                {
                    text.WriteToText("     " + op.ToString());
                    if(op.Processor != null)
                        text.WriteToText("       " + op.Processor.ToString());
                    text.WriteToText("       " + op.OperationTime.ToString());
                    foreach (ComponentType ct in op.ComponentUsages.Keys)
                    {
                        text.WriteToText("        " + ct.ToString());
                        text.WriteToText("         " + op.ComponentUsages[ct].ToString());
                    }
                }
                text.WriteToText("     " + st.BinMagazine.ToString());
                foreach (Bin b in st.BinMagazine.Content)
                {
                    text.WriteToText("       " + b.ToString());
                  //  text.WriteToText("         " + b.Location.ToString());
                    text.WriteToText("         " + b.ComponentType.ToString());
                    text.WriteToText("         " + b.ComponentType.AmountPerBin.ToString());
                }
            }
            //bins
            foreach (Bin b in this.Layout.Bins)
            {
                text.WriteToText("       " + b.ToString());
               // text.WriteToText("         " + b.Location.ToString());
                text.WriteToText("         " + b.ComponentType.ToString());
                text.WriteToText("         " + b.ComponentType.AmountPerBin.ToString());
            }
            //operations
            foreach (Operation op in this.Layout.Operations)
            {
                text.WriteToText("     " + op.ToString());
                if (op.Processor != null)
                {
                    text.WriteToText("       " + op.Processor.ToString());
                }
                text.WriteToText("       " + op.OperationTime.ToString());
                foreach (ComponentType ct in op.ComponentUsages.Keys)
                {
                    text.WriteToText("        " + ct.ToString());
                    text.WriteToText("         " + op.ComponentUsages[ct].ToString());
                }
            }
            //nodes
            foreach (Node n in this.Layout.Nodes)
            {
                text.WriteToText("     " + n.ToString());
                text.WriteToText("       " + n.Location.X.ToString());
                text.WriteToText("       " + n.Location.Y.ToString());
                if (n.BinMagazine != null) text.WriteToText("       " + n.BinMagazine.ToString());
            }
            text.CloseFile();
        }

        public void Reset(double timeIn)
        {
            SimulationManager manager = (SimulationManager)this.Parent;
            this.FinalizeStatistics(timeIn);
            this.congestedNodes.Clear();
            foreach (Station station in this.layout.Stations)
            {
                station.Reset(timeIn);
            }
        }
    }

    [XmlType("LayoutManagerState")]
    public class LayoutManagerState : FLOWObjectState
    {
        private StringList agvsToDispatch;

        private StringList agvsToRoute;

        private StringList blockedAgvs;

        private StringList congestedNodes;

        private LayoutState layout;

        public LayoutManagerState()
        {
            this.agvsToDispatch = new StringList();
            this.agvsToRoute = new StringList();
            this.blockedAgvs = new StringList();
            this.congestedNodes = new StringList();
            this.layout = new LayoutState();
        }

        [XmlArray("AgvsToDispatch")]
        [XmlArrayItem(typeof(string))]
        public StringList AgvsToDispatch
        {
            get { return this.agvsToDispatch; }
            set { this.agvsToDispatch = value; }
        }

        [XmlArray("AgvsToRoute")]
        [XmlArrayItem(typeof(string))]
        public StringList AgvsToRoute
        {
            get { return this.agvsToRoute; }
            set { this.agvsToRoute = value; }
        }

        [XmlArray("BlockedAgvs")]
        [XmlArrayItem(typeof(string))]
        public StringList BlockedAgvs
        {
            get { return this.blockedAgvs; }
            set { this.blockedAgvs = value; }
        }

        [XmlArray("CongestedNodes")]
        [XmlArrayItem(typeof(string))]
        public StringList CongestedNodes
        {
            get { return this.congestedNodes; }
            set { this.congestedNodes = value; }
        }

        public LayoutState Layout
        {
            get { return this.layout; }
            set { this.layout = value; }
        }

        public void GetState(LayoutManager layoutManagerIn)
        {
            base.GetState(layoutManagerIn);
            foreach (Node node in layoutManagerIn.CongestedNodes)
            {
                this.congestedNodes.Add(node.Name);
            }
            this.layout.GetState(layoutManagerIn.Layout);
        }

        public void SetState(LayoutManager layoutManagerIn, SimulationManager managerIn)
        {
            Layout.Layout layout = layoutManagerIn.Layout;
            layoutManagerIn.CongestedNodes.Clear();
            foreach (string node in this.congestedNodes)
            {
                layoutManagerIn.CongestedNodes.Add(layout.Nodes[node]);
            }
            this.layout.SetState(layoutManagerIn.Layout, managerIn);
            base.SetState(layoutManagerIn);
        }
    }
}
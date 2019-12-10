using System;
using System.Xml.Serialization;
using FLOW.NET.Operational;
using FLOW.NET.IO;

namespace FLOW.NET.Layout
{
    [XmlType("Layout")]
    public class Layout : FLOWObject
    {
        private StationList inputStations; //state
        private StationList outputStations; //state
        private BufferCellList bufferCells; //state
        private StationList stations; //state
        //private Station[,] distancematrix; //state??
        private BinList bins; //state
        private ComponentTypeList componentTypes; //state
        private OperationList operations; //state
        //private LaneList lanes; //state
        private NodeList nodes; //state
        private UnitloadList unitloadsonMover; // state ie486f18
        private string path;
        private TextOutput onHandWriter; //IE486
        private double scale;
        private bool gridEnabled;
        private System.Drawing.Size gridSize;
        private System.Drawing.Size size;
        private Transporter transporter; // bypass için şimdilik bir tane uncapacitated transporter yaratıldı. IE486
        public Layout()
        {
            this.bufferCells = new BufferCellList();
            this.inputStations = new StationList();
            this.nodes = new NodeList();
            this.operations = new OperationList();
            this.outputStations = new StationList();
            this.stations = new StationList();
            this.bins = new BinList();
            this.componentTypes = new ComponentTypeList();
            this.unitloadsonMover = new UnitloadList();
            this.transporter = new Transporter();
        }

        public Layout(string nameIn, FLOWObject parentIn)
            : base(nameIn, parentIn)
        {
            this.bufferCells = new BufferCellList();
            this.inputStations = new StationList();
            this.nodes = new NodeList();
            this.operations = new OperationList();
            this.outputStations = new StationList();
            this.stations = new StationList();
            this.bins = new BinList();
            this.componentTypes = new ComponentTypeList();
            this.unitloadsonMover = new UnitloadList();
            this.transporter = new Transporter();
            this.onHandWriter = new TextOutput(((SimulationManager)parentIn.Parent).path);
        }

        #region GETSET functions
        [XmlIgnore()]
        public BinList Bins
        {
            get { return this.bins; }
            set { this.bins = value; }
        }

        [XmlIgnore()]
        public BufferCellList BufferCells
        {
            get { return this.bufferCells; }
            set { this.bufferCells = value; }
        }

        [XmlIgnore()]
        public TextOutput OnHandWriter
        {
            get { return this.onHandWriter; }
            set { this.onHandWriter = value; }
        }

        [XmlIgnore()]
        public UnitloadList UnitloadsOnMover
        {
            get { return this.unitloadsonMover; }
            set { this.unitloadsonMover = value; }
        }

        [XmlArray("Stations")]
        [XmlArrayItem(typeof(Station))]
        public StationList Stations
        {
            get { return this.stations; }
            set { this.stations = value; }
        }

        [XmlArray("ComponentTypes")]
        [XmlArrayItem(typeof(ComponentType))]
        public ComponentTypeList ComponentTypes
        {
            get { return this.componentTypes; }
            set { this.componentTypes = value; }
        }

        [XmlElement("GridEnabled")]
        public bool GridEnabled
        {
            get { return this.gridEnabled; }
            set { this.gridEnabled = value; }
        }

        [XmlElement("GridSize")]
        public System.Drawing.Size GridSize
        {
            get { return this.gridSize; }
            set { this.gridSize = value; }
        }

        [XmlIgnore()]
        public StationList InputStations
        {
            get { return this.inputStations; }
            set { this.inputStations = value; }
        }

        [XmlArray("Nodes")]
        [XmlArrayItem(typeof(Node))]
        public NodeList Nodes
        {
            get { return this.nodes; }
            set { this.nodes = value; }
        }

        //[XmlArray("Lanes")]
        //[XmlArrayItem(typeof(Lane))]
        //public LaneList Lanes
        //{
        //    get { return this.lanes; }
        //    set { this.lanes = value; }
        //}

        [XmlArray("Operations")]
        [XmlArrayItem(typeof(Operation))]
        public OperationList Operations
        {
            get { return this.operations; }
            set { this.operations = value; }
        }

        [XmlIgnore()]
        public StationList OutputStations
        {
            get { return this.outputStations; }
            set { this.outputStations = value; }
        }

        [XmlElement("Transporter")]
        public Transporter Transporter
        {
            get { return this.transporter; }
            set { this.transporter = value; }
        }

        [XmlIgnore()]
        public string Path
        {
            get { return this.path; }
            set { this.path = value; }
        }

        [XmlElement("Scale")]
        public double Scale
        {
            get { return this.scale; }
            set { this.scale = value; }
        }

        [XmlElement("Size")]
        public System.Drawing.Size Size
        {
            get { return this.size; }
            set { this.size = value; }
        }

        #endregion


        public void CreateOnHandInventoryText()
        {
            string binmagazineTitle = "", componenttypeTitle = "", timeCountTitle = "";
            foreach (Station station in this.stations)
            {
                if (!(this.inputStations.Contains(station) || this.outputStations.Contains(station)))
                {
                    binmagazineTitle += station.BinMagazine.ToString();
                    foreach (ComponentType ct in station.BinMagazine.ComponentTypes)
                    {
                        binmagazineTitle += "\t" + "\t";//two tabs
                        componenttypeTitle += ct.ToString() + "\t" + "\t";
                        timeCountTitle += "Time" + "\t" + "OnHand" + "\t";
                    }
                }
            }
            this.onHandWriter.WriteToText(binmagazineTitle);
            this.onHandWriter.WriteToText(componenttypeTitle);
            this.onHandWriter.WriteToText(timeCountTitle);
        }
        public void WriteInventoryOnHand(Station stationIn, ComponentType componentTypeIn, double TimeIn, double countIn)
        {
            string textToWrite = "";
            foreach (Station station in this.stations)
            {
                if (!(this.inputStations.Contains(station) || this.outputStations.Contains(station)))
                {
                    if (!(stationIn == station))
                    {
                        for (int i = 0; i < station.BinMagazine.ComponentTypes.Count; i++)
                        {
                            textToWrite += "\t" + "\t";//two tabs
                        }
                    }
                    else
                    {
                        foreach (ComponentType ct in station.BinMagazine.ComponentTypes)
                        {
                            if (ct != componentTypeIn)
                            {
                                textToWrite += "\t" + "\t";//two tabs
                            }
                            else textToWrite += TimeIn + "\t" + countIn;
                        }
                    }
                }
            }
            this.onHandWriter.WriteToText(textToWrite);
        }
        public void ClearBinLoading(double timeIn)
        {
            foreach (Bin bin in this.bins)
            {
                if (bin.IsLoaded == true)
                {
                    bin.Unload(timeIn);
                }
            }
            foreach (Operation operation in this.operations)
            {
                operation.Station = null;
            }
            foreach (Station station in this.stations)
            {
                station.Unload(timeIn);
            }
        }

        public void ClearStatistics(double timeIn)
        {
            //foreach (Lane lane in this.lanes)
            //{
            //    lane.ClearStatistics(timeIn);
            //}
            foreach (Node node in this.nodes)
            {
                node.ClearStatistics(timeIn);
            }
            foreach (Station station in this.stations) //IE486f18
            {
                station.ClearStatistics(timeIn);
            }
            foreach (ComponentType componentType in this.componentTypes)
            {
                componentType.ClearStatistics(timeIn);
            }
            this.transporter.ClearStatistics(timeIn);
        }

        public void FinalizeStatistics(double timeIn)
        {
            foreach (Station station in this.stations)  //IE486f18
            {
                station.FinalizeStatistics(timeIn);
            }
            foreach (ComponentType componentType in this.componentTypes)
            {
                componentType.FinalizeStatistics(timeIn);
            }
            this.transporter.FinalizeStatistics(timeIn);
            onHandWriter.CloseFile();
        }

        public void PerformBinLoading(double timeIn, BinLoading binLoadingIn)
        {
            //BinLoading with respect to layout.xml 
            foreach (Station station2 in this.Stations)
            {
                foreach (ComponentType ct in station2.BinMagazine.ComponentTypes)
                {
                    for (int i = 0; i < (station2.BinMagazine.InventoryPolicies[ct].Q + station2.BinMagazine.InventoryPolicies[ct].S); i++)
                    {
                        Bin bin = new Bin(ct.Name + "-Bin" + i, this, ct);
                        bin.Count = ct.AmountPerBin;
                        this.Bins.Add(bin);
                        station2.BinMagazine.LoadBin(timeIn, bin, true);
                    }
                }
            }
            foreach (StationLoading stationLoading in binLoadingIn.Stations)
            {
                Station station = this.stations[stationLoading.Name];
                //write initial inventory levels
                foreach (ComponentType ct in station.BinMagazine.ComponentTypes)
                {
                    this.WriteInventoryOnHand(station, ct, timeIn, station.BinMagazine.GetNumberOfBins(ct));
                }
            }
        }

        public override void RelateObjects(FLOWObject parentIn)
        {
            this.Parent = parentIn;
            foreach (Node node in this.nodes)
            {
                node.RelateObjects(this);
            }
            foreach (Operation operation in this.operations)
            {
                operation.RelateObjects(this);
            }
            foreach (Station station in this.stations)
            {
                station.RelateObjects(this);
            }

            foreach (ComponentType componentType in this.componentTypes)
            {
                componentType.RelateObjects(this);
            }
        }
    }

    [XmlType("LayoutState")]
    public class LayoutState : FLOWObjectState
    {
        private BufferCellStateList bufferCells;
        private StationStateList inputStations;
        //private LaneStateList lanes;
        private NodeStateList nodes;
        private OperationStateList operations;
        private StationStateList outputStations;
        private StationStateList stations;
        private BinStateList tools;
        private ToolTypeStateList toolTypes;

        public LayoutState()
        {
            this.bufferCells = new BufferCellStateList();
            this.inputStations = new StationStateList();
            //this.lanes = new LaneStateList();
            this.nodes = new NodeStateList();
            this.operations = new OperationStateList();
            this.outputStations = new StationStateList();
            this.stations = new StationStateList();
            this.tools = new BinStateList();
            this.toolTypes = new ToolTypeStateList();
        }

        #region GETSET functions
        [XmlArray("BufferCells")]
        [XmlArrayItem(typeof(BufferCellState))]
        public BufferCellStateList BufferCells
        {
            get { return this.bufferCells; }
            set { this.bufferCells = value; }
        }

        [XmlArray("InputStations")]
        [XmlArrayItem(typeof(StationState))]
        public StationStateList InputStations
        {
            get { return this.inputStations; }
            set { this.inputStations = value; }
        }

        [XmlArray("Nodes")]
        [XmlArrayItem(typeof(NodeState))]
        public NodeStateList Nodes
        {
            get { return this.nodes; }
            set { this.nodes = value; }
        }

        [XmlArray("Operations")]
        [XmlArrayItem(typeof(OperationState))]
        public OperationStateList Operations
        {
            get { return this.operations; }
            set { this.operations = value; }
        }

        [XmlArray("OutputStations")]
        [XmlArrayItem(typeof(StationState))]
        public StationStateList OutputStations
        {
            get { return this.outputStations; }
            set { this.outputStations = value; }
        }

        [XmlArray("ProcessCells")]
        [XmlArrayItem(typeof(StationState))]
        public StationStateList ProcessCells
        {
            get { return this.stations; }
            set { this.stations = value; }
        }

        #endregion

        public void GetState(Layout layoutIn)
        {
            base.GetState(layoutIn);

            foreach (BufferCell bufferCell in layoutIn.BufferCells)
            {
                BufferCellState bufferCellState = new BufferCellState();
                bufferCellState.GetState(bufferCell);
                this.bufferCells.Add(bufferCellState);
            }
            foreach (Station inputStation in layoutIn.InputStations)
            {
                StationState inputStationState = new StationState();
                inputStationState.GetState(inputStation);
                this.inputStations.Add(inputStationState);
            }

            foreach (Node node in layoutIn.Nodes)
            {
                NodeState nodeState = new NodeState();
                nodeState.GetState(node);
                this.nodes.Add(nodeState);
            }
            foreach (Operation operation in layoutIn.Operations)
            {
                OperationState operationState = new OperationState();
                operationState.GetState(operation);
                this.operations.Add(operationState);
            }
            foreach (Station outputStation in layoutIn.OutputStations)
            {
                StationState outputStationState = new StationState();
                outputStationState.GetState(outputStation);
                this.outputStations.Add(outputStationState);
            }
            foreach (Station processCell in layoutIn.Stations)
            {
                StationState processCellState = new StationState();
                processCellState.GetState(processCell);
                this.stations.Add(processCellState);
            }
        }

        public void SetState(Layout layoutIn, SimulationManager managerIn)
        {
            //foreach (LaneState laneState in this.lanes)
            //{
            //    laneState.SetState(layoutIn.Lanes[laneState.Name], managerIn);
            //}
            foreach (NodeState nodeState in this.nodes)
            {
                nodeState.SetState(layoutIn.Nodes[nodeState.Name], managerIn);
            }
            foreach (BufferCellState bufferCellState in this.bufferCells)
            {
                bufferCellState.SetState(layoutIn.BufferCells[bufferCellState.Name], managerIn);
            }
            foreach (StationState inputStationState in this.inputStations)
            {
                inputStationState.SetState(layoutIn.InputStations[inputStationState.Name], managerIn);
            }
            foreach (OperationState operationState in this.operations)
            {
                operationState.SetState(layoutIn.Operations[operationState.Name], managerIn);
            }
            foreach (StationState outputStationState in this.outputStations)
            {
                outputStationState.SetState(layoutIn.OutputStations[outputStationState.Name], managerIn);
            }
            foreach (StationState processCellState in this.stations)
            {
                processCellState.SetState(layoutIn.Stations[processCellState.Name], managerIn);
            }
            base.SetState(layoutIn);
        }
    }
}
using System;
using System.Xml.Serialization;
using FLOW.NET.Random;
using FLOW.NET.Operational;

namespace FLOW.NET.Layout
{
    [XmlType("ComponentType")]
    public class ComponentType : FLOWObject
    {
        private StationList stations;

        private StringList stationNames;

        private int count;

        private int amountPerBin;   //    IE486fall18

        private int totalNumberOfBins;  //  IE486fall18 

        public ComponentType()
        {
            this.stationNames = new StringList();
            this.stations = new StationList();
            this.CreateStatistics();
        }

        public ComponentType(string nameIn, FLOWObject parentIn, int countIn, int sizeIn)
            : base(nameIn, parentIn)
        {
            this.count = countIn;
            this.stationNames = new StringList();
            this.stations = new StationList();
        }

        [XmlArray("CellNames")]
        [XmlArrayItem(typeof(string))]
        public StringList CellNames
        {
            get { return this.stationNames; }
            set { this.stationNames = value; }
        }

        [XmlIgnore()]
        public StationList Cells
        {
            get { return this.stations; }
            set { this.stations = value; }
        }

        [XmlElement("TotalNumberOfBins")]
        public int TotalNumberOfBins
        {
            get { return this.totalNumberOfBins; }
            set { this.totalNumberOfBins = value; }
        }

        [XmlElement("AmountPerBin")]
        public int AmountPerBin
        {
            get { return this.amountPerBin; }
            set { this.amountPerBin = value; }
        }

        [XmlElement("Count")]
        public int Count
        {
            get { return this.count; }
            set { this.count = value; }
        }

        public void ClearStatistics(double timeIn)
        {
        }

        public void CreateStatistics()
        {
        }

        public void FinalizeStatistics(double timeIn)
        {
        }

        public double GetExpectedProcessTime(Operation operationIn, Station processCellIn)
        {
            return Double.PositiveInfinity;
        }
    }

    [XmlType("ToolTypeState")]
    public class ToolTypeState : FLOWObjectState
    {
        private RVGeneratorStateList generators;

        public ToolTypeState()
        {
            this.generators = new RVGeneratorStateList();
        }

        [XmlArray("Generators")]
        [XmlArrayItem(typeof(RVGeneratorState))]
        public RVGeneratorStateList Generators
        {
            get { return this.generators; }
            set { this.generators = value; }
        }

        public void GetState(ComponentType componentTypeIn)
        {
            base.GetState(componentTypeIn);
        }

        public void SetState(ComponentType componentTypeIn, SimulationManager managerIn)
        {
            base.SetState(componentTypeIn);
        }
    }
}
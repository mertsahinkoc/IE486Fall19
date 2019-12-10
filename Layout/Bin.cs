using System;
using System.Xml.Serialization;
using FLOW.NET.Random;
using FLOW.NET.Operational;

namespace FLOW.NET.Layout
{
    public class Bin : MovableObject
    {
        private RVGeneratorList generators;
        private double loadingTime; //state
        private Storage destination; //11.12.18 get set
        private RVGenerator bintransferTime; //state

        private ComponentType componentType;

        private int count; //IE486fall18 flow additions

        public Bin(string nameIn, FLOWObject parentIn, ComponentType componentTypeIn)
            : base(nameIn, parentIn)
        {
            this.componentType = componentTypeIn;
            this.generators = new RVGeneratorList();
            this.CreateStatistics(); //IE486fall18 bakilsin
            count = 0; //IE486fall18 flow additions bincountproblem
        }

        public Bin() { }

        public Storage Destination
        {
            get { return this.destination; }
            set { this.destination = value; }
        }

        public int Count
        {
            get { return this.count; }
            set { this.count = value; }
        }

        public RVGeneratorList Generators
        {
            get { return this.generators; }
            set { this.generators = value; }
        }

        public bool IsLoaded
        {
            get { return (this.Location != null); }
        }

        public double LoadingTime
        {
            get { return this.loadingTime; }
            set { this.loadingTime = value; }
        }

        public ComponentType ComponentType
        {
            get { return this.componentType; }
            set { this.componentType = value; }
        }

        public override void ChangeLocation(double timeIn, FLOWObject locationIn)
        {
            this.Location = locationIn;
            this.EntryTime = timeIn;
            this.InTransfer = false;
        }

        public void ClearStatistics(double timeIn)
        {
            Statistics loaded = this.Statistics["Loaded"];
            loaded.Clear(timeIn, 0);
            Statistics mounted = this.Statistics["Mounted"];
            mounted.Clear(timeIn, 0);
        }

        public void CreateStatistics()
        {
            this.Statistics.Add("Loaded", new Statistics(0));
            this.Statistics.Add("Mounted", new Statistics(0));
        }

        public void FinalizeStatistics(double timeIn)
        {
            Statistics loaded = this.Statistics["Loaded"];
            if (this.IsLoaded == true)
            {
                loaded.UpdateTotal(timeIn, timeIn - this.loadingTime);
                this.loadingTime = timeIn;
            }
            Statistics mounted = this.Statistics["Mounted"];
            if (this.Location != null)
            {
                if (this.Location.GetType().Name == "Processor")
                {
                    mounted.UpdateTotal(timeIn, timeIn - this.EntryTime);
                    this.EntryTime = timeIn;
                }
            }
        }

        public void Unload(double timeIn)
        {
            Statistics loaded = this.Statistics["Loaded"];
            loaded.UpdateTotal(timeIn, (timeIn - this.loadingTime));
            this.ChangeLocation(timeIn, null);
            this.generators.Clear();
        }
    }

    [XmlType("BinState")]
    public class BinState : MovableObjectState
    {
        private string station;

        private double loadingTime;

        private string binMagazine;

        public BinState()
        {
        }

        [XmlElement("Station")]
        public string Station
        {
            get { return this.station; }
            set { this.station = value; }
        }

        [XmlElement("LoadingTime")]
        public double LoadingTime
        {
            get { return this.loadingTime; }
            set { this.loadingTime = value; }
        }

        [XmlElement("BinMagazine")]
        public string BinMagazine
        {
            get { return this.binMagazine; }
            set { this.binMagazine = value; }
        }

        public void GetState(Bin binIn)
        {
            base.GetState(binIn);
            this.loadingTime = binIn.LoadingTime;

        }

        public void SetState(Bin binIn, SimulationManager managerIn)
        {
            Layout layout = managerIn.LayoutManager.Layout;
            binIn.LoadingTime = this.loadingTime;
            base.SetState(binIn);
        }
    }
}
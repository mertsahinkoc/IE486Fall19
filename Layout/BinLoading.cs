using System;
using System.Xml.Serialization;

namespace FLOW.NET.Layout
{
    [XmlType("BinLoading")]
    public class BinLoading : FLOWObject
    {
        private StationLoadingList stations;

        private string path;

        public BinLoading(string nameIn, FLOWObject parentIn)
            : base(nameIn, parentIn)
        {
            this.stations = new StationLoadingList();
        }

        public BinLoading()
        {
            this.stations = new StationLoadingList();
        }

        [XmlArray("Stations")]
        [XmlArrayItem("Station", typeof(StationLoading))]
        public StationLoadingList Stations
        {
            get { return this.stations; }
            set { this.stations = value; }
        }

        [XmlIgnore()]
        public string Path
        {
            get { return this.path; }
            set { this.path = value; }
        }

        public void AddBinToStationMagazine(string binNameIn, string cellNameIn)
        {
            foreach (StationLoading stationLoading in this.stations)
            {
                if (stationLoading.Name == cellNameIn)
                {
                    stationLoading.MagazineLoading.Bins.Add(binNameIn);
                    return;
                }
            }
        }
    }

}

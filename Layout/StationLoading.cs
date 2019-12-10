using System;
using System.Xml.Serialization;

namespace FLOW.NET.Layout
{
    [XmlType("StationLoading")]
    public class StationLoading : FLOWObject
    {
        private MagazineLoading magazineLoading;


        public StationLoading(string nameIn, FLOWObject parentIn)
            : base(nameIn, parentIn)
        {
        }

        public StationLoading()
        {
        }

        [XmlElement("BinMagazine", typeof(MagazineLoading))]
        public MagazineLoading MagazineLoading
        {
            get { return this.magazineLoading; }
            set { this.magazineLoading = value; }
        }

    }
}

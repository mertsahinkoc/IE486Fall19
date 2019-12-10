using System;
using System.Xml.Serialization;

namespace FLOW.NET.Layout
{
    public class MagazineLoading : FLOWObject
    {
        private StringList bins;

        public MagazineLoading()
        {
            this.bins = new StringList();
        }

        public MagazineLoading(string nameIn, FLOWObject parentIn)
            : base(nameIn, parentIn)
        {
            this.bins = new StringList();
        }

        [XmlArray("Bins")]
        [XmlArrayItem("Bin", typeof(string))]
        public StringList Bins
        {
            get { return this.bins; }
            set { this.bins = value; }
        }
    }

}
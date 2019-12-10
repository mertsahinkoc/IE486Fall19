using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using FLOW.NET.Random;

namespace FLOW.NET.Layout
{
    [XmlType("Supermarket")]
    public class Supermarket : Storage
    {
        //ie486f18
        private RVGenerator loadTime;

        private RVGenerator unloadTime;

        [XmlElement("UnloadTime", typeof(RVGenerator))]
        public RVGenerator UnloadTime
        {
            get { return this.unloadTime; }
            set { this.unloadTime = value; }
        }

        [XmlElement("LoadTime", typeof(RVGenerator))]
        public RVGenerator LoadTime
        {
            get { return this.loadTime; }
            set { this.loadTime = value; }
        }
    }
}

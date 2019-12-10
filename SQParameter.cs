using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

//Every component type has its own s and Q values in each bin magazine.

namespace FLOW.NET
{
    [XmlType("SQParameter")]
    public class SQParameter
    {
        private double s;
        private int q;

        public SQParameter()
        { 
        }

        public SQParameter(double sIn, int qIn)
        {
            s = sIn;
            q = qIn;
        }

        [XmlElement("S")]
        public double S
        {
            get { return this.s; }
            set { this.s = value; }
        }

        [XmlElement("Q")]
        public int Q
        {
            get { return this.q; }
            set { this.q = value; }
        }


    }
}
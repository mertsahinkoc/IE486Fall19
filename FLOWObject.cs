using System;
using System.IO;
using System.Xml.Serialization;

namespace FLOW.NET
{
    [XmlType("FLOWObject")]
    public abstract class FLOWObject : IComparable
    {
        private string name;
        private FLOWObject parent;
        private StatisticsStringDictionary statistics;

        public FLOWObject()
        {
            this.statistics = new StatisticsStringDictionary();
        }

        public FLOWObject(string nameIn, FLOWObject parentIn)
        {
            this.name = nameIn;
            this.parent = parentIn;
            this.statistics = new StatisticsStringDictionary();
        }

        #region GETSET functions
        [XmlElement("Name")]
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        [XmlIgnore()]
        public FLOWObject Parent
        {
            get { return this.parent; }
            set { this.parent = value; }
        }

        [XmlIgnore()]
        public StatisticsStringDictionary Statistics
        {
            get { return this.statistics; }
            set { this.statistics = value; }
        }
        #endregion

        public virtual int CompareTo(object obj)
        {
            return this.name.CompareTo(((FLOWObject)obj).Name);
        }

        public virtual string GetInformation()
        {
            StringWriter writer = new StringWriter();
            writer.WriteLine("Name : {0}", this.name);
            return writer.ToString().Trim();
        }

        public virtual void RelateObjects(FLOWObject parentIn)
        {
            this.parent = parentIn;
        }

        public override string ToString()
        {
            return this.name;
        }
    }

    [XmlType("FLOWObjectState")]
    public class FLOWObjectState
    {
        private string name;
        private StatisticsStateList statistics;
        public FLOWObjectState()
        {
            this.statistics = new StatisticsStateList();
        }

        #region GETSET functions
        [XmlElement("Name")]
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        [XmlArray("Statistics")]
        [XmlArrayItem(typeof(StatisticsState))]
        public StatisticsStateList Statistics
        {
            get { return this.statistics; }
            set { this.statistics = value; }
        }
        #endregion

        public void GetState(FLOWObject flowObjectIn)
        {
            this.name = flowObjectIn.Name;
            foreach (string key in flowObjectIn.Statistics.Keys)
            {
                StatisticsState statistics = new StatisticsState();
                statistics.GetState(flowObjectIn.Statistics[key], key);
                this.statistics.Add(statistics);
            }
        }

        public void SetState(FLOWObject flowObjectIn)
        {
            foreach (StatisticsState statisticsState in this.statistics)
            {
                statisticsState.SetState(flowObjectIn.Statistics[statisticsState.Name]);
            }
        }
    }
}
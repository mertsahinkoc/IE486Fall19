using System;
using System.Xml.Serialization;

namespace FLOW.NET.Operational
{
    [XmlType("SimulationParameter")]
    public class SimulationParameter
    {
        private AlgorithmParameter algorithms;

        private ConfigurationParameter configuration;

        private string jobPath;

        private string layoutPath;

        private int seed;

        public SimulationParameter()
        {
            this.algorithms = new AlgorithmParameter();
            this.configuration = new ConfigurationParameter();
        }

        [XmlElement("Algorithms")]
        public AlgorithmParameter Algorithms
        {
            get { return this.algorithms; }
            set { this.algorithms = value; }
        }

        [XmlElement("Configuration")]
        public ConfigurationParameter Configuration
        {
            get { return this.configuration; }
            set { this.configuration = value; }
        }

        [XmlElement("JobPath")]
        public string JobPath
        {
            get { return this.jobPath; }
            set { this.jobPath = value; }
        }

        [XmlElement("LayoutPath")]
        public string LayoutPath
        {
            get { return this.layoutPath; }
            set { this.layoutPath = value; }
        }

        [XmlElement("Seed")]
        public int Seed
        {
            get { return this.seed; }
            set { this.seed = value; }
        }
    }
}
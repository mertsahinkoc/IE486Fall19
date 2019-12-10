using System;
using System.Xml.Serialization;

namespace FLOW.NET.Operational
{
    [XmlType("BatchRun")]
    public class BatchRun
    {
        private string path;

        private SimulationParameterList simulationParameters;

        public BatchRun()
        {
            this.simulationParameters = new SimulationParameterList();
        }

        [XmlIgnore()]
        public string Path
        {
            get { return this.path; }
            set { this.path = value; }
        }

        [XmlArray("SimulationParameters")]
        [XmlArrayItem(typeof(SimulationParameter))]
        public SimulationParameterList SimulationParameters
        {
            get { return this.simulationParameters; }
            set { this.simulationParameters = value; }
        }
    }
}
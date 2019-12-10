using System;
using System.Xml.Serialization;

namespace FLOW.NET.Operational
{
    [XmlType("AlgorithmParameter")]
    public class AlgorithmParameter
    {
        private string operationSelectionAlgorithm;

        private string orderReleaseAlgorithm;

        private string stationControllerAlgorithm;

        private string replenishmentControllerAlgorithm;

        private string partSequencingForProcessorAlgorithm;

        private string pullAlgorithm;

        private string pushAlgorithm;

        private string processorSelectionAlgorithm;

        private string stationSelectionAlgorithm;

        public AlgorithmParameter()
        {
        }

        [XmlElement("PullAlgorithm")]
        public string PullAlgorithm
        {
            get { return this.pullAlgorithm; }
            set { this.pullAlgorithm = value; }
        }

        [XmlElement("PushAlgorithm")]
        public string PushAlgorithm
        {
            get { return this.pushAlgorithm; }
            set { this.pushAlgorithm = value; }
        }


        [XmlElement("ProcessorSelectionAlgorithm")]
        public string ProcessorSelectionAlgorithm
        {
            get { return this.processorSelectionAlgorithm; }
            set { this.processorSelectionAlgorithm = value; }
        }

        [XmlElement("StationSelectionAlgorithm")]
        public string StationSelectionAlgorithm
        {
            get { return this.stationSelectionAlgorithm; }
            set { this.stationSelectionAlgorithm = value; }
        }

        [XmlElement("OperationSelectionAlgorithm")]
        public string OperationSelectionAlgorithm
        {
            get { return this.operationSelectionAlgorithm; }
            set { this.operationSelectionAlgorithm = value; }
        }

        [XmlElement("OrderReleaseAlgorithm")]
        public string OrderReleaseAlgorithm
        {
            get { return this.orderReleaseAlgorithm; }
            set { this.orderReleaseAlgorithm = value; }
        }

        [XmlElement("StationControllerAlgorithm")]
        public string StationControllerAlgorithm
        {
            get { return this.stationControllerAlgorithm; }
            set { this.stationControllerAlgorithm = value; }
        }

        [XmlElement("ReplenishmentControllerAlgorithm")]
        public string ReplenishmentControllerAlgorithm
        {
            get { return this.replenishmentControllerAlgorithm; }
            set { this.replenishmentControllerAlgorithm = value; }
        }

        [XmlElement("PartSequencingForProcessorAlgorithm")]
        public string PartSequencingForProcessorAlgorithm
        {
            get { return this.partSequencingForProcessorAlgorithm; }
            set { this.partSequencingForProcessorAlgorithm = value; }
        }
    }
}
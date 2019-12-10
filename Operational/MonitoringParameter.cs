using System;
using System.Xml.Serialization;

using FLOW.NET.IO;

namespace FLOW.NET.Operational
{
    [XmlType("MonitoringParameter")]
    public class MonitoringParameter
    {
        private int monitoringLeftThreshold;

        private double monitoringLeftZValue;

        private double monitoringPeriodTime;

        private int monitoringRightThreshold;

        private double monitoringRightZValue;

        [XmlElement("MonitoringLeftThreshold")]
        public int MonitoringLeftThreshold
        {
            get { return this.monitoringLeftThreshold; }
            set { this.monitoringLeftThreshold = value; }
        }

        [XmlElement("MonitoringLeftZValue")]
        public double MonitoringLeftZValue
        {
            get { return this.monitoringLeftZValue; }
            set { this.monitoringLeftZValue = value; }
        }

        [XmlElement("MonitoringPeriodTime")]
        public double MonitoringPeriodTime
        {
            get { return this.monitoringPeriodTime; }
            set { this.monitoringPeriodTime = value; }
        }

        [XmlElement("MonitoringRightThreshold")]
        public int MonitoringRightThreshold
        {
            get { return this.monitoringRightThreshold; }
            set { this.monitoringRightThreshold = value; }
        }

        [XmlElement("MonitoringRightZValue")]
        public double MonitoringRightZValue
        {
            get { return this.monitoringRightZValue; }
            set { this.monitoringRightZValue = value; }
        }
    }
}
using System;
using System.Xml.Serialization;

using FLOW.NET.IO;

namespace FLOW.NET.Operational
{
    public enum JobArrivalType
    {
        DistributionBased,

        TraceBased,

        HOPBased,
    }

    public enum LoadingPeriodType
    {
        JobBased,

        TimeBased,
    }

    public enum PlanningPeriodType
    {
        JobBased,

        TimeBased,
    }

    public enum RunningMode
    {
        Default,

        Execution,

        Planning
    }

    public enum SimulationPeriodType
    {
        JobBased,

        TimeBased,
    }

    [XmlType("ConfigurationParameter")]
    public class ConfigurationParameter
    {
        private double finalArrivalTime;

        private bool fromExecutionToPlanning;

        private Input inputState;

        private JobArrivalType jobArrivalType;

        private Input jobRelease;

        private double loadingPeriodTime;

        private LoadingPeriodType loadingPeriodType;

        private MonitoringParameter monitoring;

        private Output periodState;

        private double planningPeriodTime;

        private PlanningPeriodType planningPeriodType;

        private double replanningThreshold;

        private RunningMode runningMode;

        private double simulationPeriodTime;

        private SimulationPeriodType simulationPeriodType;

        private string toolPath;

        private string tracePath;

        private double warmupPeriodTime;

        public ConfigurationParameter()
        {
        }

        [XmlElement("FinalArrivalTime")]
        public double FinalArrivalTime
        {
            get { return this.finalArrivalTime; }
            set { this.finalArrivalTime = value; }
        }

        [XmlElement("FromExecutionToPlanning")]
        public bool FromExecutionToPlanning
        {
            get { return this.fromExecutionToPlanning; }
            set { this.fromExecutionToPlanning = value; }
        }

        [XmlElement("InputState")]
        public Input InputState
        {
            get { return this.inputState; }
            set { this.inputState = value; }
        }

        [XmlElement("JobArrivalType")]
        public JobArrivalType JobArrivalType
        {
            get { return this.jobArrivalType; }
            set { this.jobArrivalType = value; }
        }

        [XmlElement("JobRelease")]
        public Input JobRelease
        {
            get { return this.jobRelease; }
            set { this.jobRelease = value; }
        }

        [XmlElement("LoadingPeriodTime")]
        public double LoadingPeriodTime
        {
            get { return this.loadingPeriodTime; }
            set { this.loadingPeriodTime = value; }
        }

        [XmlElement("LoadingPeriodType")]
        public LoadingPeriodType LoadingPeriodType
        {
            get { return this.loadingPeriodType; }
            set { this.loadingPeriodType = value; }
        }

        [XmlElement("Monitoring")]
        public MonitoringParameter Monitoring
        {
            get { return this.monitoring; }
            set { this.monitoring = value; }
        }

        [XmlElement("PeriodState")]
        public Output PeriodState
        {
            get { return this.periodState; }
            set { this.periodState = value; }
        }

        [XmlElement("PlanningPeriodTime")]
        public double PlanningPeriodTime
        {
            get { return this.planningPeriodTime; }
            set { this.planningPeriodTime = value; }
        }

        [XmlElement("PlanningPeriodType")]
        public PlanningPeriodType PlanningPeriodType
        {
            get { return this.planningPeriodType; }
            set { this.planningPeriodType = value; }
        }

        [XmlElement("ReplanningThreshold")]
        public double ReplanningThreshold
        {
            get { return this.replanningThreshold; }
            set { this.replanningThreshold = value; }
        }

        [XmlElement("RunningMode")]
        public RunningMode RunningMode
        {
            get { return this.runningMode; }
            set { this.runningMode = value; }
        }

        [XmlElement("SimulationPeriodTime")]
        public double SimulationPeriodTime
        {
            get { return this.simulationPeriodTime; }
            set { this.simulationPeriodTime = value; }
        }

        [XmlElement("SimulationPeriodType")]
        public SimulationPeriodType SimulationPeriodType
        {
            get { return this.simulationPeriodType; }
            set { this.simulationPeriodType = value; }
        }

        [XmlElement("ToolPath")]
        public string ToolPath
        {
            get { return this.toolPath; }
            set { this.toolPath = value; }
        }

        [XmlElement("TracePath")]
        public string TracePath
        {
            get { return this.tracePath; }
            set { this.tracePath = value; }
        }

        [XmlElement("WarmupPeriodTime")]
        public double WarmupPeriodTime
        {
            get { return this.warmupPeriodTime; }
            set { this.warmupPeriodTime = value; }
        }

        public void InitializeStreams()
        {
            if (this.inputState != null)
            {
                this.inputState.Initialize();
            }
            if (this.jobRelease != null)
            {
                this.jobRelease.Initialize();
            }
            if (this.periodState != null)
            {
                this.periodState.Initialize();
            }
        }
    }
}
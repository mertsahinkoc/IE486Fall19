using System;
using System.IO;
using System.Data;
using System.Xml.Serialization;
using FLOW.NET.Layout;
using FLOW.NET.Random;
using FLOW.NET.Decision;
using FLOW.NET.Operational.Events;
using FLOW.NET.IO;

namespace FLOW.NET.Operational
{
    public class SimulationManager : FLOWObject
    {
        private AlgorithmCollection algorithms; //state
        private EventCalendar eventCalendar; //state
        private JobManager jobManager; //state
        private LayoutManager layoutManager; //state
        private SimulationParameter parameter;
        private double startTime; //state
        private double time; //state
        public string path; //Onhand inventory path

        public SimulationManager()
            : base("SimulationManager", null)
        {
            this.algorithms = new AlgorithmCollection();
            this.eventCalendar = new EventCalendar(this);
            this.layoutManager = new LayoutManager("LayoutManager", this);
            this.jobManager = new JobManager("JobManager", this);
        }

        #region GETSET functions
        public AlgorithmCollection Algorithms
        {
            get { return this.algorithms; }
            set { this.algorithms = value; }
        }

        public EventCalendar EventCalendar
        {
            get { return this.eventCalendar; }
            set { this.eventCalendar = value; }
        }

        public JobManager JobManager
        {
            get { return this.jobManager; }
            set { this.jobManager = value; }
        }

        public LayoutManager LayoutManager
        {
            get { return this.layoutManager; }
            set { this.layoutManager = value; }
        }

        public SimulationParameter Parameter
        {
            get { return this.parameter; }
            set { this.parameter = value; }
        }

        public double StartTime
        {
            get { return this.startTime; }
            set { this.startTime = value; }
        }

        public double Time
        {
            get { return this.time; }
            set { this.time = value; }
        }
        #endregion

        public void ChangeBinLoading(Stream streamIn)
        {
            XmlSerializer reader = new XmlSerializer(typeof(BinLoading));
            BinLoading toolLoading = (BinLoading)reader.Deserialize(streamIn);
            this.layoutManager.Layout.PerformBinLoading(this.time, toolLoading);
            streamIn.Close();
        }

        public void ClearStatistics(double timeIn)
        {
            this.startTime = timeIn;
            this.jobManager.ClearStatistics(timeIn);
            this.layoutManager.ClearStatistics(timeIn);
        }
        //Just binmagazines have node in whole system.
        public void ConnectNodesAndBinMagazines()
        {
            NodeList nodes = this.layoutManager.Layout.Nodes;
            StationList stations = this.layoutManager.Layout.Stations;
            foreach (Node node in nodes)
            {
                foreach (Station station in stations)
                {
                    if (station.BinMagazine.NodeName == node.Name)
                    {
                        station.BinMagazine.Node = node;
                    }
                }
            }
        }

       
        public void ConnectJobTypesAndOperations()
        {
            JobTypeList jobTypes = this.jobManager.JobMix.JobTypes;
            foreach (JobType jobType in jobTypes)
            {
                foreach (JobRoute jobRoute in jobType.Alternates)
                {
                    foreach (string operationName in jobRoute.OperationNames)
                    {
                        Operation operation = this.layoutManager.Layout.Operations[operationName];
                        jobRoute.Operations.Add(operation);
                    }
                }
                jobType.InputStation = this.layoutManager.Layout.Stations[jobType.InputStationName];
                jobType.OutputStation = this.layoutManager.Layout.Stations[jobType.OutputStationName];
                if (!this.layoutManager.Layout.InputStations.Contains(jobType.InputStation))
                {
                    this.layoutManager.Layout.InputStations.Add(jobType.InputStation);
                }
                if (!this.layoutManager.Layout.OutputStations.Contains(jobType.OutputStation))
                {
                    this.layoutManager.Layout.OutputStations.Add(jobType.OutputStation);
                }
            }
        }

        //Every station knows its operations at the beginning of simulation.
        public void ConnectStationsAndOperations()
        {
            OperationList operations = this.layoutManager.Layout.Operations;
            StationList stations = this.layoutManager.Layout.Stations;
            foreach (Station station in stations)
            {
                foreach (Processor processor in station.Processors)
                {
                    foreach (Operation operation in operations)
                    {
                        for (int i = 0; i < processor.OperationNames.Count; i++)
                        {
                            if (operation.Name == processor.OperationNames[i])
                            {
                                processor.Operations.Add(operation);
                                station.Operations.Add(operation);
                                
                                //add to binmagazine componenttypes if not already included IE486
                                foreach(ComponentType ct in operation.ComponentUsages.Keys)
                                {
                                    if (!station.BinMagazine.ComponentTypes.Contains(ct))
                                    {
                                        station.BinMagazine.ComponentTypes.Add(ct);
                                    }
                                } 
                            }
                        }                       
                    }
                }              
            }
        }

        //Connecting componenttype usages and operations with respect to inputs in XML
        public void ConnectComponentTypesAndOperations()
        {
            OperationList operationlist = this.layoutManager.Layout.Operations;
            foreach (Operation op in operationlist)
            {
                foreach (string componentType in op.ComponentUsagesList)
                {
                    string[] seperated = componentType.Split(',');
                    string componentTypeName = seperated[0];
                    int usageRatio = Convert.ToInt32(seperated[1]);
                    op.ComponentUsages.Add(this.layoutManager.Layout.ComponentTypes[componentTypeName],usageRatio);
                }
            }
        }

        //Every bin magazine has different inventory policy for each component type.
        public void ConnectInventoryPoliciesAndBinMagazines()
        {
            StationList stationlist = this.layoutManager.Layout.Stations;
            foreach (Station station in stationlist)
            {
                foreach (string inventoryPolicy in station.BinMagazine.InventoryPolicyNameList)
                {
                    string[] seperated = inventoryPolicy.Split(',');
                    string componentTypeName = seperated[0];
                    double reorderPoint = Convert.ToDouble(seperated[1],System.Globalization.CultureInfo.InvariantCulture); 
                    int orderAmount = Convert.ToInt32(seperated[2]);
                    SQParameter InventoryPolicy = new SQParameter(reorderPoint, orderAmount);
                    station.BinMagazine.InventoryPolicies.Add(this.layoutManager.Layout.ComponentTypes[componentTypeName], InventoryPolicy);
                }
                station.BinMagazine.CreateStatistics();
            }
            this.layoutManager.Layout.CreateOnHandInventoryText();//IE486f18
        }


        public void ConnectOperationsAndProcessors()
        {
            OperationList operations = this.layoutManager.Layout.Operations;
            StationList stations = this.layoutManager.Layout.Stations;

            //DistanceMatrix Region ie486f18
            //foreach(Station station in stations)
            //{
            //    foreach(Station station2 in stations)
            //    {
            //       this.layoutManager.Layout.DistanceMatrix[station,station2] = 1;
            //    }
            //}
            //DistanceMatrix Region End ie486f18

            //Assumption: Same operation can be performed in different processors of the same station, it can be done in different stations.
            // Operation names are uniquely given to stations. 
            
            foreach (Operation operation in operations)
            {
                foreach (Station station in stations)
                {
                    if (station.Operations.Contains(operation))
                    {
                        if (station.Processors.Count != 1) //if it is one, its processor will be NULL too. 
                        {
                            int count = 0; // count how many processor can perform it to find out whether it is identical or not
                            Processor currentprocessor=new Processor();
                            foreach (Processor processor in station.Processors)
                            {
                                for (int i = 0; i < processor.OperationNames.Count; i++)
                                {
                                    if (processor.OperationNames[i] == operation.Name)
                                    {
                                        currentprocessor = processor;
                                        count++;
                                    }
                                }
                            }
                            if (count==1) //just one processor can do
                            {
                                operation.Processor = currentprocessor;
                            }
                        }
                        operation.Station = station;
                        break; // we found which station can perform current operation, we move to next operation.
                    }
                }
            }
        }

        public void ConstructSystem()
        {
            RngStream.SetPackageSeed(this.parameter.Seed);
            this.layoutManager.ReadLayout(new FileStream(this.parameter.LayoutPath, FileMode.Open));
            this.jobManager.ReadJobMix(new FileStream(this.parameter.JobPath, FileMode.Open));
            this.ConnectJobTypesAndOperations();
            this.ConnectComponentTypesAndOperations();
            this.ConnectStationsAndOperations();
            this.ConnectOperationsAndProcessors();
            this.ConnectNodesAndBinMagazines();
            this.ConnectInventoryPoliciesAndBinMagazines();
            ConfigurationParameter configuration = this.parameter.Configuration;
            configuration.InitializeStreams();
            if (configuration.JobArrivalType != JobArrivalType.HOPBased)
            {
                this.ChangeBinLoading(new FileStream(configuration.ToolPath, FileMode.Open));
            }
            this.algorithms.InitializeAlgorithms(this);

            if (configuration.InputState != null)
            {
                configuration.InputState.PreOperation();
                this.ReadSimulationManagerState(configuration.InputState.GetStream(), configuration.FromExecutionToPlanning);
                configuration.InputState.PostOperation();
            }

            TextOutput text = new TextOutput("JobMix.txt");
            WriteJobMix(text);
            TextOutput text2 = new TextOutput("Layout.txt");
            this.LayoutManager.WriteLayout(text2);

        }
        public void WriteJobMix(TextOutput text)
        {
            text.WriteToText(this.ToString());
            text.WriteToText(this.jobManager.ToString());
            foreach (JobType jt in this.jobManager.JobMix.JobTypes)
            {
                text.WriteToText("   " + jt.ToString());
                foreach (JobRoute jr in jt.Alternates)
                {
                    text.WriteToText("     " + jr.ToString());
                    foreach (Operation op in jr.Operations)
                    {
                        text.WriteToText("        " + op.ToString());
                        text.WriteToText("        " + op.Station.ToString());
                        if(op.Processor != null)
                            text.WriteToText("        " + op.Processor.ToString());
                        text.WriteToText("        " + op.OperationTime.ToString());
                        foreach (ComponentType ct in op.ComponentUsages.Keys)
                        {
                            text.WriteToText("          " + ct.ToString());
                            text.WriteToText("          " + op.ComponentUsages[ct].ToString());
                        }
                    }
                }
            }
            text.CloseFile();
        }
        public void WriteSimulationManagerState(bool fromExecutionToPlanningIn)
        {
            using (StreamWriter writetext = new StreamWriter("write.txt"))
            {
                writetext.WriteLine(layoutManager.Layout);
                writetext.WriteLine(layoutManager.Layout.Stations);
                writetext.WriteLine(layoutManager.Layout.Bins);
            }
        }

        public void FinalizeStatistics(double timeIn)
        {
            this.jobManager.FinalizeStatistics(timeIn);
            this.layoutManager.FinalizeStatistics(timeIn);
        }

        public void PerformSimulation()
        {
			Event currentEvent = null;
            while (this.eventCalendar.Events.Count != 0)
            {
                EventList eventList = this.eventCalendar.Events.Values[0];
                if (eventList.Count != 0)
                {
                    while (eventList.Count != 0)
                    {
                        currentEvent = eventList[0];
						
						eventList.RemoveAt(0);
                        this.time = currentEvent.Time;
                        currentEvent.Execute();
                    }
                    this.algorithms.ExecuteAlgorithms();
                }
                if (this.eventCalendar.Events.Count > 0 && this.eventCalendar.Events.Values[0].Count == 0)
                {
                    this.eventCalendar.Events.RemoveAt(0);
                }
            }
            this.WriteSimulationManagerState(false);
            this.FinalizeStatistics(this.time);
        }

        public void PrepareArrivalsFromTrace()
        {
            DataSet arrivals = new DataSet();
            arrivals.ReadXml(this.parameter.Configuration.TracePath);
            if (arrivals.Tables.Count > 0)
            {
                JobTypeList jobTypes = this.jobManager.JobMix.JobTypes;
                foreach (DataRow currentRow in arrivals.Tables[0].Rows)
                {
                    double arrivalTime = Double.Parse(currentRow["Time"].ToString());
                    string typeName = currentRow["Type"].ToString();
                    this.jobManager.BatchList.Add(Int32.Parse(currentRow["Batch"].ToString()));
                    this.eventCalendar.ScheduleArrivalEvent(arrivalTime, jobTypes[typeName]);
                }
            }
        }

        public void PrepareInitialArrivals()
        {
            JobTypeList jobTypes = this.jobManager.JobMix.JobTypes;
            foreach (JobType jobType in jobTypes)
            {
                double arrivalTime = jobType.Arrival.GenerateValue();
                if (arrivalTime <= this.parameter.Configuration.FinalArrivalTime)
                {
                    this.eventCalendar.ScheduleArrivalEvent(arrivalTime, jobType);
                }
            }
        }

        public void PrepareSimulation()
        {
            ConfigurationParameter configuration = this.parameter.Configuration;
            if (configuration.JobArrivalType == JobArrivalType.DistributionBased)
            {
                this.PrepareInitialArrivals();
            }
            else
            {
                if (configuration.JobArrivalType == JobArrivalType.HOPBased)
                {
                    this.eventCalendar.ScheduleStartPlanningPeriodEvent(this.time);
                    if (this.time == 0)
                    {
                    }
                }
                else
                {
                    if (configuration.JobArrivalType == JobArrivalType.TraceBased)
                    {
                        this.PrepareArrivalsFromTrace();
                    }
                }
            }
            if (configuration.SimulationPeriodType == SimulationPeriodType.TimeBased)
            {
                if (configuration.WarmupPeriodTime != 0)
                {
                    this.eventCalendar.ScheduleEndWarmupEvent(configuration.WarmupPeriodTime);
                }
                this.eventCalendar.ScheduleEndSimulationEvent(configuration.SimulationPeriodTime);
            }
        }

        public void ReadSimulationManagerState(Stream streamIn, bool fromExecutionToPlanningIn)
        {
            XmlSerializer reader = new XmlSerializer(typeof(SimulationManagerState));
            SimulationManagerState simulationManagerState = (SimulationManagerState)reader.Deserialize(streamIn);
            SimulationManagerState.FromExecutionToPlanning = fromExecutionToPlanningIn;
            simulationManagerState.SetState(this);
            streamIn.Close();

            if (fromExecutionToPlanningIn == true)
            {
                this.eventCalendar.Events.Clear();
                this.jobManager.Reset(this.time);
                this.layoutManager.Reset(this.time);
            }
        }

        public void TriggerOperationDecisionAlgorithm(Unitload unitloadIn)
        {
            if (this.jobManager.UnitloadsToDecide.Contains(unitloadIn) == false)
            {
                this.jobManager.UnitloadsToDecide.Add(unitloadIn);
            }
        }

        public void TriggerStationControllerAlgorithm(Station stationIn)
        {
            if (this.layoutManager.StationsToDecide.Contains(stationIn) == false)
            {
                this.layoutManager.StationsToDecide.Add(stationIn);
            }
        }

        public void TriggerPull(Queue queueIn)
        {
            this.LayoutManager.QueuesToPull.Add(queueIn);
        }

        public void TriggerPush(Unitload unitloadIn)
        {
            this.JobManager.UnitloadsToPush.Add(unitloadIn);
        }
        public void TriggerOrder(Order orderIn)
        {
            this.LayoutManager.OrdersToReplenish.Add(orderIn);
        }
    }

    [XmlType("SimulationManagerStateType")]
    public enum SimulationManagerStateType
    {
        EndPlanningPeriod,

        LessProductionRate,

        MoreProductionRate,

        ResourceChange
    }

    [XmlType("SimulationManagerState")]
    public class SimulationManagerState : FLOWObjectState
    {
        private AlgorithmCollectionState algorithms;

        private EventCalendarState eventCalendar;

        private static bool fromExecutionToPlanning = false;

        private JobManagerState jobManager;

        private LayoutManagerState layoutManager;

        private SimulationManagerStateType stateType;

        private double startTime;

        private double time;

        public SimulationManagerState()
        {
            this.algorithms = new AlgorithmCollectionState();
            this.eventCalendar = new EventCalendarState();
            this.jobManager = new JobManagerState();
            this.layoutManager = new LayoutManagerState();
        }

        [XmlElement("Algorithms")]
        public AlgorithmCollectionState Algorithms
        {
            get { return this.algorithms; }
            set { this.algorithms = value; }
        }

        [XmlElement("EventCalendar")]
        public EventCalendarState EventCalendar
        {
            get { return this.eventCalendar; }
            set { this.eventCalendar = value; }
        }

        [XmlIgnore()]
        public static bool FromExecutionToPlanning
        {
            get { return SimulationManagerState.fromExecutionToPlanning; }
            set { SimulationManagerState.fromExecutionToPlanning = value; }
        }

        [XmlElement("JobManager")]
        public JobManagerState JobManager
        {
            get { return this.jobManager; }
            set { this.jobManager = value; }
        }

        [XmlElement("LayoutManager")]
        public LayoutManagerState LayoutManager
        {
            get { return this.layoutManager; }
            set { this.layoutManager = value; }
        }

        [XmlElement("StartTime")]
        public double StartTime
        {
            get { return this.startTime; }
            set { this.startTime = value; }
        }

        [XmlElement("Time")]
        public double Time
        {
            get { return this.time; }
            set { this.time = value; }
        }

        [XmlElement("StateType")]
        public SimulationManagerStateType StateType
        {
            get { return this.stateType; }
            set { this.stateType = value; }
        }

        public void GetState(SimulationManager managerIn)
        {
            base.GetState(managerIn);
            this.algorithms.GetState(managerIn.Algorithms);
            this.eventCalendar.GetState(managerIn.EventCalendar);
            this.jobManager.GetState(managerIn.JobManager);
            this.layoutManager.GetState(managerIn.LayoutManager);
            this.startTime = managerIn.StartTime;
            this.time = managerIn.Time;
        }

        public void SetState(SimulationManager managerIn)
        {
            this.algorithms.SetState(managerIn.Algorithms, managerIn);
            this.eventCalendar.SetState(managerIn.EventCalendar, managerIn);
            this.jobManager.SetState(managerIn.JobManager, managerIn);
            this.layoutManager.SetState(managerIn.LayoutManager, managerIn);
            managerIn.StartTime = this.startTime;
            managerIn.Time = this.time;
            base.SetState(managerIn);
        }
    }
}
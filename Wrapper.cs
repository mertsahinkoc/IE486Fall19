using System;
using System.IO;
using FLOW.NET.Operational;
using FLOW.NET.IO;
using FLOW.NET;
using FLOW.NET.Layout;
using System.Xml.Serialization;
using FLOW.NET.Random;
using FLOW.NET.Operational.Events;
using System.Xml;

namespace Wrapper
{
    class Wrapper
    {
        [STAThread]
        static void Main(string[] args)
        {

            SimulationParameter parameters = new SimulationParameter();
            parameters.Seed = 46;

            //1-Specify the xml files
            parameters.LayoutPath = "flow_layout.xml";
            parameters.JobPath = "flow_jobmix.xml";
            parameters.Configuration.ToolPath = "flow_binloading.xml";
            //parameters.Configuration.TracePath = Directory.GetCurrentDirectory() +  "\\Trace Files\\" + parameters.Seed + ".xml";
            // parameters.Configuration.TracePath = Directory.GetCurrentDirectory() + "\\Trace Files\\A100.xml";   //IE486f18
            parameters.Configuration.TracePath = Directory.GetCurrentDirectory() + "\\Trace Files\\Trace-B.xml";   //IE486f18
            // parameters.Configuration.TracePath = Directory.GetCurrentDirectory() + "\\Trace Files\\B100.xml";   //IE486f18
            //parameters.Configuration.TracePath = Directory.GetCurrentDirectory() + "\\Trace Files\\A1B1.xml";   //IE486f18
            //parameters.Configuration.TracePath = Directory.GetCurrentDirectory() + "\\Trace Files\\A10B10.xml";   //IE486f18
            //parameters.Configuration.TracePath = Directory.GetCurrentDirectory() + "\\Trace Files\\A50B50.xml";   //IE486f18
            
            //2-Specify the simulation run time parameters 
            parameters.Configuration.SimulationPeriodTime = 75600;
            parameters.Configuration.WarmupPeriodTime = 18000;  //  486update 25.12.2018
            parameters.Configuration.FinalArrivalTime = 75600;
            parameters.Configuration.PlanningPeriodTime = 75600;

            //Below are the set of algorithms that will be used during the simulation
            //3-Modify the algorithms as you desire 
            parameters.Algorithms.PartSequencingForProcessorAlgorithm = "FirstComeMustGo";
            parameters.Algorithms.StationControllerAlgorithm = "MixedProcessorConfiguration";
            parameters.Algorithms.OrderControllerAlgorithm = "ByPassReplenish";
            parameters.Algorithms.OrderReleaseAlgorithm = "CyclicalImmediate";
            parameters.Algorithms.StationSelectionAlgorithm = "SelectionOfFirstStation";
            parameters.Algorithms.PullAlgorithm = "FirstBlockedFirstServed";
            parameters.Algorithms.PushAlgorithm = "AlwaysPush";
            parameters.Algorithms.ProcessorSelectionAlgorithm = "NonDelaySPT";


            parameters.Configuration.JobArrivalType = JobArrivalType.TraceBased;
            //parameters.Configuration.JobArrivalType = JobArrivalType.DistributionBased; //IE486f18
            parameters.Configuration.SimulationPeriodType = SimulationPeriodType.TimeBased;
            parameters.Configuration.PlanningPeriodType = PlanningPeriodType.TimeBased;
            parameters.Configuration.LoadingPeriodType = LoadingPeriodType.TimeBased;

            parameters.Configuration.PeriodState = new FileOutput(true, "periodstate.xml");

            //4-Prepare the environment and run the simulation
            SimulationManager simulationManager = new SimulationManager();
            string path = "Seed-" + parameters.Seed + "OnhandInventory.txt";
            simulationManager.path = path;
            simulationManager.Parameter = parameters;
            simulationManager.ConstructSystem();
            simulationManager.PrepareSimulation();
            simulationManager.PerformSimulation();

            //5-Reporting. Note that the report xml file will be generated in the Wrapper\bin\Debug folder with a random name i.e. c2ed605b-088d-474a-98c6-f939765683cd.xml
            Reporter reporter = new Reporter();
            
            path = "Seed-" + parameters.Seed + "Report";
            reporter.BuildStatisticsReport(simulationManager,path);
        }
    }

    class Reporter
    {

        public void BuildStatisticsReport(SimulationManager simulationManager, string reporterPath)
        {
            StringWriter writer = new StringWriter();

            writer.WriteLine("<Report>");

            #region Basic Parameters
            SimulationParameter parameter = simulationManager.Parameter;
            writer.WriteLine("<JobPath>{0}</JobPath>", parameter.JobPath);
            writer.WriteLine("<LayoutPath>{0}</LayoutPath>", parameter.LayoutPath);
            writer.WriteLine("<Seed>{0}</Seed>", parameter.Seed);
            #endregion

            #region Algorithm Names
            AlgorithmParameter algorithms = parameter.Algorithms;
            writer.WriteLine(String.Format("<OperationSelection>{0}</OperationSelection>", algorithms.OperationSelectionAlgorithm));
            writer.WriteLine(String.Format("<OrderRelease>{0}</OrderRelease>", algorithms.OrderReleaseAlgorithm));
            writer.WriteLine(String.Format("<StationController>{0}</StationController>", algorithms.StationControllerAlgorithm));
            writer.WriteLine(String.Format("<OrderController>{0}</OrderController>", algorithms.OrderControllerAlgorithm));
            writer.WriteLine(String.Format("<PartSequencingForProcessor>{0}</PartSequencingForProcessor>", algorithms.PartSequencingForProcessorAlgorithm));
            //writer.WriteLine(String.Format("<PartSequencingForOutQueue>{0}</PartSequencingForOutQueue>", algorithms.PartSequencingForOutQueueAlgorithm));
            #endregion

            #region Configuration Parameters
            ConfigurationParameter configuration = parameter.Configuration;
            writer.WriteLine(String.Format("<FinalArrivalTime>{0}</FinalArrivalTime>", configuration.FinalArrivalTime));
            writer.WriteLine(String.Format("<JobRelease>{0}</JobRelease>", configuration.JobRelease));
            writer.WriteLine(String.Format("<JobArrivalType>{0}</JobArrivalType>", configuration.JobArrivalType));
            writer.WriteLine(String.Format("<LoadingPeriodTime>{0}</LoadingPeriodTime>", configuration.LoadingPeriodTime));
            writer.WriteLine(String.Format("<LoadingPeriodType>{0}</LoadingPeriodType>", configuration.LoadingPeriodType));
            writer.WriteLine(String.Format("<PeriodState>{0}</PeriodState>", configuration.PeriodState));
            writer.WriteLine(String.Format("<PlanningPeriodTime>{0}</PlanningPeriodTime>", configuration.PlanningPeriodTime));
            writer.WriteLine(String.Format("<PlanningPeriodType>{0}</PlanningPeriodType>", configuration.PlanningPeriodType));
            writer.WriteLine(String.Format("<SimulationPeriodTime>{0}</SimulationPeriodTime>", configuration.SimulationPeriodTime));
            writer.WriteLine(String.Format("<SimulationPeriodType>{0}</SimulationPeriodType>", configuration.SimulationPeriodType));
            writer.WriteLine(String.Format("<ToolPath>{0}</ToolPath>", configuration.ToolPath));
            writer.WriteLine(String.Format("<TracePath>{0}</TracePath>", configuration.TracePath));
            writer.WriteLine(String.Format("<WarmupPeriodTime>{0}</WarmupPeriodTime>", configuration.WarmupPeriodTime));
            #endregion

            #region Transporter Statistics
            writer.WriteLine("<Transporter>");
            Transporter transporter = simulationManager.LayoutManager.Layout.Transporter;
            writer.WriteLine("<Name>{0}</Name>", transporter.Name);

            writer.WriteLine("<Statistics>");

            Statistics load = (Statistics)transporter.Statistics["Load"];
            writer.WriteLine(ReportUtilizationBasedStatistics("Load", load, simulationManager));

            Statistics busy = (Statistics)transporter.Statistics["Busy"];
            writer.WriteLine(ReportUtilizationBasedStatistics("Busy", busy, simulationManager));

            writer.WriteLine("</Statistics>");
            writer.WriteLine("</Transporter>");
            #endregion

            #region Cell Statistics
            writer.WriteLine("<Cells>");
            foreach (Station cell in simulationManager.LayoutManager.Layout.Stations)
            {
                writer.WriteLine("<Cell>");
                writer.WriteLine("<Name>{0}</Name>", cell.Name);

                writer.WriteLine("<Statistics>");

                if (cell.GetType().Name == "Station")
                {
                    Statistics blocked = (Statistics)cell.Statistics["Blocked"];
                    writer.WriteLine(ReportUtilizationBasedStatistics("Blocked", blocked, simulationManager));
                }

                if (cell.GetType().Name == "Station")
                {
                    Statistics starved = (Statistics)cell.Statistics["Starved"];
                    writer.WriteLine(ReportUtilizationBasedStatistics("Starved", starved, simulationManager));
                }

                Statistics inqueueLength = (Statistics)cell.InQueue.Statistics["Length"];
                writer.WriteLine(ReportTimeBasedStatistics("InqueueLength", inqueueLength, simulationManager));

                Statistics inqueueTime = (Statistics)cell.InQueue.Statistics["Time"];
                writer.WriteLine(ReportAverageBasedStatistics("InqueueTime", inqueueTime));

                writer.WriteLine("</Statistics>");

                if (cell.GetType().Name == "Station")
                {
                           Station processCell = (Station)cell;
                    writer.WriteLine("<Processors>");
                    foreach (Processor processor in processCell.Processors)
                    {
                        writer.WriteLine("<Processor>");
                        writer.WriteLine("<Name>{0}</Name>", processor.Name);

                        writer.WriteLine("<Statistics>");

                        Statistics usage = (Statistics)processor.Statistics["Usage"];
                        writer.WriteLine(ReportUtilizationBasedStatistics("Usage", usage, simulationManager));

                        Statistics blocked = (Statistics)processor.Statistics["Blocked"];
                        writer.WriteLine(ReportUtilizationBasedStatistics("Blocked", blocked, simulationManager));

                        Statistics starved = (Statistics)processor.Statistics["Starved"];
                        writer.WriteLine(ReportUtilizationBasedStatistics("Starved", starved, simulationManager));

                        writer.WriteLine("</Statistics>");

                     
                               writer.WriteLine("</Processor>");
                    }
                    writer.WriteLine("</Processors>");

                }
                writer.WriteLine("</Cell>");
            }
            writer.WriteLine("</Cells>");
            #endregion

            #region JobType Statistics
            writer.WriteLine("<JobTypes>");
            foreach (JobType jobType in simulationManager.JobManager.JobMix.JobTypes)
            {
                writer.WriteLine("<JobType>");
                writer.WriteLine("<Name>{0}</Name>", jobType.Name);

                writer.WriteLine("<Statistics>");
                Statistics submitted = (Statistics)jobType.Statistics["Submitted"];
                writer.WriteLine(ReportCountBasedStatistics("Submitted", submitted));

                Statistics completed = (Statistics)jobType.Statistics["Completed"];
                writer.WriteLine(ReportCountBasedStatistics("Completed", completed));

                Statistics shopTime = (Statistics)jobType.Statistics["ShopTime"];
                writer.WriteLine(ReportAverageBasedStatistics("ShopTime", shopTime));

                Statistics totalTime = (Statistics)jobType.Statistics["TotalTime"];
                writer.WriteLine(ReportAverageBasedStatistics("TotalTime", totalTime));

                Statistics lateness = (Statistics)jobType.Statistics["Lateness"];
                writer.WriteLine(ReportAverageBasedStatistics("Lateness", lateness));

                Statistics earliness = (Statistics)jobType.Statistics["Earliness"];
                writer.WriteLine(ReportAverageBasedStatistics("Earliness", earliness));

                Statistics tardiness = (Statistics)jobType.Statistics["Tardiness"];
                writer.WriteLine(ReportAverageBasedStatistics("Tardiness", tardiness));

                Statistics numberofTardyJobs = (Statistics)jobType.Statistics["NumberofTardyJobs"];
                writer.WriteLine(ReportCountBasedStatistics("NumberofTardyJobs", numberofTardyJobs));

                writer.WriteLine("</Statistics>");

				writer.WriteLine("<Alternates>");
				foreach (JobRoute alternate in jobType.Alternates)
				{
					writer.WriteLine("<Alternate>");
					writer.WriteLine("<Name>" + alternate.Name + "</Name>");
					Statistics usage = (Statistics)alternate.Usage;
					writer.WriteLine(ReportCountBasedStatistics("UsageCount", usage));
					writer.WriteLine("</Alternate>");
				}
				writer.WriteLine("</Alternates>");

                writer.WriteLine("</JobType>");
            }
            writer.WriteLine("</JobTypes>");
            #endregion


            #region BinMagazine Statistics
            writer.WriteLine("<BinMagazines>");
            foreach (Station station in simulationManager.LayoutManager.Layout.Stations)
            {


                if (!(simulationManager.LayoutManager.Layout.InputStations.Contains(station) || simulationManager.LayoutManager.Layout.OutputStations.Contains(station)))
                {
                    writer.WriteLine("<BinMagazine>");
                    writer.WriteLine("<Name>{0}</Name>", station.BinMagazine.Name);
                    writer.WriteLine("<Statistics>");
                    Statistics leadtime = (Statistics)station.BinMagazine.Statistics["OrderLeadTime"];
                    writer.WriteLine(ReportAverageBasedStatistics("OrderLeadTime", leadtime));

                    Statistics outstanding = (Statistics)station.BinMagazine.Statistics["OutstandingOrderCount"];
                    writer.WriteLine(ReportUtilizationBasedStatistics("OutstandingOrderCount", outstanding, simulationManager));


                    foreach (ComponentType ct in station.BinMagazine.ComponentTypes)
                    {
                        Statistics componentcount = (Statistics)station.BinMagazine.Statistics["ComponentTypeCount-" + ct.Name];
                        writer.WriteLine(ReportUtilizationBasedStatistics("ComponentTypeCount-" + ct.Name, componentcount, simulationManager));

                        Statistics bintime = (Statistics)station.BinMagazine.Statistics["BinTime-" + ct.Name];
                        writer.WriteLine(ReportAverageBasedStatistics("BinTime-" + ct.Name, bintime));
                    }
                    writer.WriteLine("</Statistics>");
                    writer.WriteLine("</BinMagazine>");
                }
            }
            writer.WriteLine("</BinMagazines>");

            #endregion


            

            writer.WriteLine("</Report>");

            XmlDocument document = new XmlDocument();
			string s = writer.ToString().Trim();
            document.LoadXml(s);

            document.Save(String.Format(reporterPath + "{0}.xml", Guid.NewGuid()));
        }

        public string ReportCountBasedStatistics(string nameIn, Statistics statisticsIn)
        {
            StringWriter writer = new StringWriter();

            writer.WriteLine("<Statistic>");
            writer.WriteLine("<Name>{0}</Name>", nameIn);
            writer.WriteLine("<Count>{0}</Count>", statisticsIn.Count);
            writer.WriteLine("</Statistic>");

            return writer.ToString().Trim();
        }

        public string ReportTotalBasedStatistics(string nameIn, double totalIn)
        {
            StringWriter writer = new StringWriter();

            writer.WriteLine("<Statistic>");
            writer.WriteLine("<Name>{0}</Name>", nameIn);
            writer.WriteLine("<Total>{0}</Total>", totalIn);
            writer.WriteLine("</Statistic>");

            return writer.ToString().Trim();
        }

        public string ReportAverageBasedStatistics(string nameIn, Statistics statisticsIn)
        {
            StringWriter writer = new StringWriter();

            writer.WriteLine("<Statistic>");
            writer.WriteLine("<Name>{0}</Name>", nameIn);
            writer.WriteLine("<Mean>{0}</Mean>", statisticsIn.Total / statisticsIn.Count);
            writer.WriteLine("<Minimum>{0}</Minimum>", statisticsIn.Minimum);
            writer.WriteLine("<Maximum>{0}</Maximum>", statisticsIn.Maximum);
            writer.WriteLine("<Deviation>{0}</Deviation>", Math.Sqrt((statisticsIn.SumSquare - statisticsIn.Count * Math.Pow(statisticsIn.Total / statisticsIn.Count, 2)) / (statisticsIn.Count - 1)));
            writer.WriteLine("</Statistic>");

            return writer.ToString().Trim();
        }

        public string ReportTimeBasedStatistics(string nameIn, Statistics statisticsIn, SimulationManager manager)
        {
            StringWriter writer = new StringWriter();

            writer.WriteLine("<Statistic>");
            writer.WriteLine("<Name>{0}</Name>", nameIn);
            writer.WriteLine("<Mean>{0}</Mean>", statisticsIn.Total / (manager.Time - manager.StartTime));
            writer.WriteLine("<Minimum>{0}</Minimum>", statisticsIn.Minimum);
            writer.WriteLine("<Maximum>{0}</Maximum>", statisticsIn.Maximum);
            writer.WriteLine("<Deviation>{0}</Deviation>", Math.Sqrt((statisticsIn.SumSquare / (manager.Time - manager.StartTime)) - Math.Pow(statisticsIn.Total / (manager.Time - manager.StartTime), 2)));
            writer.WriteLine("</Statistic>");

            return writer.ToString().Trim();
        }

        public string ReportUtilizationBasedStatistics(string nameIn, Statistics statisticsIn, SimulationManager manager)
        {
            StringWriter writer = new StringWriter();

            writer.WriteLine("<Statistic>");
            writer.WriteLine("<Name>{0}</Name>", nameIn);
            writer.WriteLine("<Mean>{0}</Mean>", statisticsIn.Total / (manager.Time - manager.StartTime));
            writer.WriteLine("</Statistic>");

            return writer.ToString().Trim();
        }
        
    }
}

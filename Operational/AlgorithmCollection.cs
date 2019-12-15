using System;
using System.IO;
using System.Data;
using System.Xml.Serialization;
using FLOW.NET.Layout;
using FLOW.NET.Random;
using FLOW.NET.Decision;
using FLOW.NET.Operational.Events;

namespace FLOW.NET.Operational
{
    public class AlgorithmCollection
    {

        private OperationSelectionAlgorithm operationSelection; //state

        private OrderReleaseAlgorithm orderRelease; //state

        private ProcessorSelectionAlgorithm processorSelection; //state ie486f18

        private StationSelectionAlgorithm stationSelection; //state ie486f18

        private PartSequencingForProcessorAlgorithm partSequencingForProcessor; //state

        private StationControllerAlgorithm stationControllerAlgorithm; //state

        private OrderControllerAlgorithm orderControllerAlgorithm; //IE486 12.12.18

        private PullAlgorithm pullAlgorithm;

        private PushAlgorithm pushAlgorithm;

        public ProcessorSelectionAlgorithm ProcessorSelection
        {
            get { return this.processorSelection; }
            set { this.processorSelection = value; }
        }

        public StationSelectionAlgorithm StationSelection
        {
            get { return this.stationSelection; }
            set { this.stationSelection = value; }
        }

        public PullAlgorithm Pull
        {
            get { return this.pullAlgorithm; }
            set { this.pullAlgorithm = value; }
        }

        public PushAlgorithm Push
        {
            get { return this.pushAlgorithm; }
            set { this.pushAlgorithm = value; }
        }

        public OperationSelectionAlgorithm OperationSelection
        {
            get { return this.operationSelection; }
            set { this.operationSelection = value; }
        }       

        public OrderReleaseAlgorithm OrderRelease
        {
            get { return this.orderRelease; }
            set { this.orderRelease = value; }
        }

        public StationControllerAlgorithm StationController
        {
            get { return this.stationControllerAlgorithm; }
            set { this.stationControllerAlgorithm = value; }
        }

        public OrderControllerAlgorithm OrderController
        {
            get { return this.orderControllerAlgorithm; }
            set { this.orderControllerAlgorithm = value; }
        }

        public PartSequencingForProcessorAlgorithm PartSequencingForProcessor
        {
            get { return this.partSequencingForProcessor; }
            set { this.partSequencingForProcessor = value; }
        }


        public void InitializeAlgorithms(SimulationManager manager)
        {

            this.orderRelease = OrderReleaseAlgorithm.GetAlgorithmByName(manager.Parameter.Algorithms.OrderReleaseAlgorithm);
            this.stationControllerAlgorithm = StationControllerAlgorithm.GetAlgorithmByName(manager.Parameter.Algorithms.StationControllerAlgorithm);
            this.orderControllerAlgorithm = OrderControllerAlgorithm.GetAlgorithmByName(manager.Parameter.Algorithms.OrderControllerAlgorithm);
            this.partSequencingForProcessor = PartSequencingForProcessorAlgorithm.GetAlgorithmByName(manager.Parameter.Algorithms.PartSequencingForProcessorAlgorithm);
            this.pullAlgorithm = PullAlgorithm.GetAlgorithmByName(manager.Parameter.Algorithms.PullAlgorithm);
            this.pushAlgorithm = PushAlgorithm.GetAlgorithmByName(manager.Parameter.Algorithms.PushAlgorithm);
            this.processorSelection = ProcessorSelectionAlgorithm.GetAlgorithmByName(manager.Parameter.Algorithms.ProcessorSelectionAlgorithm);
            this.stationSelection = StationSelectionAlgorithm.GetAlgorithmByName(manager.Parameter.Algorithms.StationSelectionAlgorithm);
            this.orderRelease.Initialize(manager);
            this.stationControllerAlgorithm.Initialize(manager);
            this.orderControllerAlgorithm.Initialize(manager);
            this.partSequencingForProcessor.Initialize(manager);
            this.pullAlgorithm.Initialize(manager);     //IE486f18
            this.pushAlgorithm.Initialize(manager);     //IE486f18
            this.processorSelection.Initialize(manager);     //IE486f18
            this.stationSelection.Initialize(manager);   //IE486f18
        }

        public void ExecuteAlgorithms()
        {
            this.orderRelease.Execute();
            this.pullAlgorithm.Execute();
            this.pushAlgorithm.Execute();
            this.StationController.Execute();
            this.OrderController.Execute();

        }


        public Processor ExecuteOperationProcessorSelectionAlgorithm(Unitload unitloadIn)
        {
            return this.processorSelection.Execute(unitloadIn);
        }

        public void ExecuteOperationStationSelectionAlgorithm(Unitload unitloadIn)
        {
            this.stationSelection.Execute(unitloadIn);
        }


        public void ExecutePartSequencingForProcessorAlgorithm(UnitloadList unitloadsIn)
        {
            this.partSequencingForProcessor.Execute(unitloadsIn);
        }

    }

    [XmlType("AlgorithmCollectionState")]
    public class AlgorithmCollectionState
    {
        private RngStreamState operationSelection;

        private RngStreamState orderRelease;

        private RngStreamState partSequencingForProcessor;

        private RngStreamState stationSelection;

        public AlgorithmCollectionState()
        {
            this.operationSelection = new RngStreamState();
            this.orderRelease = new RngStreamState();
            this.partSequencingForProcessor = new RngStreamState();
            this.stationSelection = new RngStreamState();
        }


        [XmlElement("OperationSelection")]
        public RngStreamState OperationSelection
        {
            get { return this.operationSelection; }
            set { this.operationSelection = value; }
        }

        [XmlElement("OrderRelease")]
        public RngStreamState OrderRelease
        {
            get { return this.orderRelease; }
            set { this.orderRelease = value; }
        }

        [XmlElement("PartSequencingForProcessor")]
        public RngStreamState PartSequencingForProcessor
        {
            get { return this.partSequencingForProcessor; }
            set { this.partSequencingForProcessor = value; }
        }

        [XmlElement("StationSelection")]
        public RngStreamState StationSelection
        {
            get { return this.stationSelection; }
            set { this.stationSelection = value; }
        }

        public void GetState(AlgorithmCollection algorithms)
        {
            this.operationSelection.GetState(algorithms.OperationSelection.Stream);
            this.orderRelease.GetState(algorithms.OrderRelease.Stream);
            this.partSequencingForProcessor.GetState(algorithms.PartSequencingForProcessor.Stream);
        }

        public void SetState(AlgorithmCollection algorithms, SimulationManager managerIn)
        {
            this.operationSelection.SetState(algorithms.OperationSelection.Stream);
            this.orderRelease.SetState(algorithms.OrderRelease.Stream);
            this.partSequencingForProcessor.SetState(algorithms.PartSequencingForProcessor.Stream);
        }
    }
}
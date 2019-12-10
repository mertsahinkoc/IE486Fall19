using System;
using System.IO;
using System.Xml.Serialization;
using FLOW.NET.Operational;
using FLOW.NET.Random;

namespace FLOW.NET.Layout
{
    [XmlType("Operation")]
    public class Operation : FLOWObject
    {
        private Station station;        //  IE486fall18
        private Processor processor;    //  IE486fall18
        private ComponentTypeUsageDictionary componentUsages;  //   IE486fall18
        private StringList componentUsagesList;  // IE486fall18 
        private RVGenerator operationTime;  //IE486fall18

        public Operation()
        {
            componentUsages = new ComponentTypeUsageDictionary();
        }

        public Operation(string nameIn, FLOWObject parentIn): base(nameIn, parentIn)
        {
        }

        #region GETSET functions
        [XmlElement("OperationTime", typeof(RVGenerator))]
        public RVGenerator OperationTime
        {
            get { return this.operationTime; }
            set { this.operationTime = value; }
        }


        [XmlArray("ComponentUsagesList")] //  IE486fall18
        [XmlArrayItem(typeof(string))]
        public StringList ComponentUsagesList
        {
            get { return this.componentUsagesList; }
            set { this.componentUsagesList = value; }
        }

        [XmlIgnore()]
        public Station Station
        {
            get { return this.station; }
            set { this.station = value; }
        }

        [XmlIgnore()]
        public Processor Processor
        {
            get { return this.processor; }
            set { this.processor = value; }
        }

        [XmlIgnore()]
        public ComponentTypeUsageDictionary ComponentUsages
        {
            get { return this.componentUsages; }
            set { this.componentUsages = value; }
        }



        #endregion

        public double GetExpectedProcessTime(Station StationIn)
        {
            return this.operationTime.ExpectedValue();
        }


        public override string GetInformation()
        {
            StringWriter writer = new StringWriter();
            writer.WriteLine(base.GetInformation());
            return writer.ToString().Trim();
        }
    }

    [XmlType("OperationState")]
    public class OperationState : FLOWObjectState
    {
        public OperationState()
        {
        }

        public void GetState(Operation operationIn)
        {
            base.GetState(operationIn);
        }

        public void SetState(Operation operationIn, SimulationManager managerIn)
        {
            base.SetState(operationIn);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using FLOW.NET.Random;

namespace FLOW.NET.Layout
{
    [XmlType("Supermarket")]
    public class Supermarket : Storage
    {
        /*
        //ie486f18
        private RVGenerator loadTime;

        private RVGenerator unloadTime;

        [XmlElement("UnloadTime", typeof(RVGenerator))]
        public RVGenerator UnloadTime
        {
            get { return this.unloadTime; }
            set { this.unloadTime = value; }
        }

        [XmlElement("LoadTime", typeof(RVGenerator))]
        public RVGenerator LoadTime
        {
            get { return this.loadTime; }
            set { this.loadTime = value; }
        }
        */ //bence bu ara komple olmamalı çünkü bu bypass olucağı assume edilerek yazılmış

        //ie486fall19
        private OrderList UnassignedOrderList; //dynamic
        private BinList UnassignedBinList; //static!
        private TransferTaskList TransferTasks; //dynamic
        private TransporterList ReadyTransportersAtDock; //dynamic
        private BinList EmptyBinList; //dynamic
        private BinList AssignedBinList; //dynamic
        private BinList ReadyBinList; //dynamic

        [XmlIgnore()]
        public OrderList UnassignedOrderList
        {
            get { return this.UnassignedOrderList; }
            set { this.UnassignedOrderList = value; }
        }

        [XmlElement("UnassignedBinList")]
        public BinList UnassignedBinList
        {
            get { return this.UnassignedBinList; }
            set { this.UnassignedBinList = value; }
        }

        [XmlIgnore()]
        public TransferTaskList TransferTasks
        {
            get { return this.TransferTasks; }
            set { this.TransferTasks = value; }
        }

        [XmlIgnore()]
        public TransporterList ReadyTransportersAtDock
        {
            get { return this.ReadyTransportersAtDock; }
            set { this.ReadyTransportersAtDock = value; }
        }

        [XmlIgnore()]
        public BinList EmptyBinList
        {
            get { return this.EmptyBinList; }
            set { this.EmptyBinList = value; }
        }

        [XmlIgnore()]
        public BinList AssignedBinList
        {
            get { return this.AssignedBinList; }
            set { this.AssignedBinList = value; }
        }

        [XmlIgnore()]
        public BinList ReadyBinList
        {
            get { return this.ReadyBinList; }
            set { this.ReadyBinList = value; }
        }
    }
}

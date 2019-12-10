using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FLOW.NET.Layout
{
    [XmlType("Storage")]
    public abstract class Storage : StaticObject
    {
        private Node node;
        private string nodeName;
        private StorageList upstreamSuppliers;
        private StorageList downstreamCustomers;
        //ie486f18

        public Storage()
        {
            upstreamSuppliers = new StorageList();
            downstreamCustomers = new StorageList();
        }

        //From upstream to downstream list: Warehouse->Supermarket->BinMagazine
        public Storage(string nameIn, FLOWObject parentIn, int capacityIn, Node nodeIn) : base(nameIn, parentIn, capacityIn)
        {
            if (nodeIn != null)
            {
                this.Node = nodeIn;
                this.NodeName = nodeIn.Name;
            }
            upstreamSuppliers = new StorageList();
            downstreamCustomers = new StorageList();
        }

        [XmlElement("UpstreamSuppliers")]
        public StorageList UpstreamSuppliers
        {
            get { return this.upstreamSuppliers; }
            set { this.upstreamSuppliers = value; }
        }

        [XmlElement("DownstreamCustomers")]
        public StorageList DownstreamCustomers
        {
            get { return this.downstreamCustomers; }
            set { this.downstreamCustomers = value; }
        }
        [XmlIgnore()]
        public Node Node
        {
            get { return this.node; }
            set
            {
                this.node = value; if (value != null)
                {
                    this.nodeName = value.Name;
                }
                else
                {
                    this.nodeName = String.Empty;
                }
            }
        }

        [XmlElement("NodeName")]
        public string NodeName
        {
            get { return this.nodeName; }
            set { this.nodeName = value; }
        }
    }
}
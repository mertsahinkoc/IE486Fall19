using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using FLOW.NET.Operational;
using FLOW.NET.Random;

namespace FLOW.NET.Layout
{
    [XmlType("Transporter")]
    public class Transporter : MovableObject
    {

        // IE 486 Fall 19
        private double capacity;
        private Node parkNode; //Park Node number is assumed to be 1 for Fall19
        private NodeList serviceNodes;
        private double speed;
        // IE 486 Fall 19
        //private StorageList storages;
        private NodeList route;
        //private RVGenerator transferTime; //for bypass algorithm
        //private RVGenerator travelTime; //for bypass algorithm
        private BinList content;

        //Transporter number is assumed to be 1 for ie486f18

        public Transporter()
        {
            content = new BinList();
            this.CreateStatistics();
        }

        public Transporter (string nameIn, FLOWObject parentIn)
            : base(nameIn, parentIn)
        {
            this.CreateStatistics();
        }


        //[XmlIgnore()]
        //public StorageList Storages
        //{
        //    get { return this.storages; }
        //    set { this.storages = value; }
        //}
        [XmlIgnore()]
        public BinList Content
        {
            get { return this.content; }
            set { this.content = value; }
        }
        //[XmlElement("TravelTime", typeof(RVGenerator))]
        //public RVGenerator TravelTime
        //{
        //    get { return this.travelTime; }
        //    set { this.travelTime = value; }
        //}


        [XmlIgnore()]
        public NodeList Route
        {
            get { return this.route; }
            set { this.route = value; }
        }

        //Fall19
        [XmlElement("Capacity")]
        public double Capacity
        {
            get { return this.capacity; }
            set { this.capacity = value; }
        }
        [XmlElement("ParkNode")]
        public Node ParkNode
        {
            get { return this.parkNode; }
            set { this.parkNode = value; }
        }
        [XmlElement("ServiceNodes")]
        public NodeList ServiceNodes
        {
            get { return this.serviceNodes; }
            set { this.serviceNodes = value; }
        }
        [XmlElement("Speed")]
        public double Speed
        {
            get { return this.speed; }
            set { this.speed = value; }
        }

        //Fall19


        public void Release(double timeIn, Bin binIn)
        {
            this.content.Remove(binIn);
            Statistics load = this.Statistics["Load"];
            load.UpdateWeighted(timeIn, this.Content.Count);
            binIn.ChangeLocation(timeIn, null);
            binIn.Destination = null;
            if (this.content.Count == 0) // for bypass 
            {
                Statistics busy = this.Statistics["Busy"];
                busy.UpdateWeighted(timeIn, 0);
            }            
        }

        public void Receive(double timeIn, Bin binIn)
        {
            binIn.ChangeLocation(timeIn,this);
            this.content.Add(binIn);
            Statistics busy = this.Statistics["Busy"]; //for bypass
            busy.UpdateWeighted(timeIn, 1);
            Statistics load = this.Statistics["Load"];
            load.UpdateWeighted(timeIn, this.Content.Count);
        }
        public void ClearStatistics(double timeIn)
        {
            Statistics load = this.Statistics["Load"];
            load.Clear(timeIn, this.Content.Count);
            Statistics busy = this.Statistics["Busy"];
            if (this.content.Count !=0) // for bypass
            {
                busy.Clear(timeIn, 1);
            }
            else
            {
                busy.Clear(timeIn, 0);
            }          
        }

        public void CreateStatistics()
        {
            this.Statistics.Add("Load", new Statistics(0));
            this.Statistics.Add("Busy", new Statistics(0));
        }

        public void FinalizeStatistics(double timeIn)
        {
            Statistics load = this.Statistics["Load"];
            load.UpdateWeighted(timeIn, this.content.Count);
            Statistics busy= this.Statistics["Busy"];
            busy.UpdateWeighted(timeIn, this.content.Count);
        }

    }
}

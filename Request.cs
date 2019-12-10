using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using FLOW.NET.Layout;



namespace FLOW.NET
{
    public class Request //IE486 Fall18
    {
        private Storage owner;
        private ComponentType componentType;
        private double time;

        public Request() { }
        public Request(Storage ownerIn, ComponentType componentypeIn, double timeIn)
        {
            owner = ownerIn;
            componentType = componentypeIn;
            time = timeIn;
        }

        public Storage Owner
        {
            get { return this.owner; }
            set { this.owner = value; }
        }
        public ComponentType ComponentType
        {
            get { return this.componentType; }
            set { this.componentType = value; }
        }
        public double Time
        {
            get { return this.time; }
            set { this.time = value; }
        }
    }
}
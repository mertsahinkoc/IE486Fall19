using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using FLOW.NET.Layout;



namespace FLOW.NET
{
    public class Order //IE486 Fall18
    {
        private Storage owner;
        private Storage supplier;
        private ComponentType componentType;
        private double orderTime;
        private ComponentType returnType;
        private double dueDate;
        private int priority;

        public Order() { }
        public Order(Storage ownerIn, ComponentType componentypeIn, double timeIn)
        {
            owner = ownerIn;
            componentType = componentypeIn;
            orderTime = timeIn;
        }

        public Storage Owner
        {
            get { return this.owner; }
            set { this.owner = value; }
        }
        public Storage Supplier
        {
            get { return this.supplier; }
            set { this.supplier = value; }
        }
        public ComponentType ComponentType
        {
            get { return this.componentType; }
            set { this.componentType = value; }
        }
        public ComponentType ReturnType
        {
            get { return this.returnType; }
            set { this.returnType = value; }
        }
        public double OrderTime
        {
            get { return this.orderTime; }
            set { this.orderTime = value; }
        }
        public double DueDate
        {
            get { return this.dueDate; }
            set { this.dueDate = value; }
        }
        public int Priority
        {
            get { return this.priority; }
            set { this.priority = value; }
        }
    }
}
using System;
using System.Xml.Serialization;

namespace FLOW.NET.Operational
{
    [XmlType("JobRoute")]
    public class JobRoute : FLOWObject
    {
        private StringList operationNames;

        private OperationList operations;

        private Statistics usage;

        public JobRoute()
        {
            this.operations = new OperationList();
            this.operationNames = new StringList();
			createStatistics();
        }

        public JobRoute(string nameIn, FLOWObject parentIn)
            : base(nameIn, parentIn)
        {
            this.operations = new OperationList();
            this.operationNames = new StringList();
			createStatistics();
        }

        [XmlArray("OperationNames")]
        [XmlArrayItem("OperationName", typeof(string))]
        public StringList OperationNames
        {
            get { return this.operationNames; }
            set { this.operationNames = value; }
        }

        [XmlIgnore()]
        public OperationList Operations
        {
            get { return this.operations; }
            set { this.operations = value; }
        }

		[XmlIgnore()]
		public Statistics Usage
		{
			get { return this.usage; }
			set { this.usage = value; }
		}

		public void updateUsage(double timeIn)
		{
			usage.UpdateCount(timeIn, 1);
		}

		public void createStatistics()
		{
			this.usage = new Statistics(0);
		}

		public void clearStatistics(double timeIn)
		{
			this.usage.Clear(timeIn, 0);
		}
    }
}
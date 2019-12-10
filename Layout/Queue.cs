using System;
using System.IO;
using System.Xml.Serialization;
using FLOW.NET.Operational;
using FLOW.NET;
using FLOW.NET.Layout;

namespace FLOW.NET.Layout
{
    [XmlType("Queue")]
    public class Queue : StaticObject
    {
        private UnitloadList content;

        public Queue()
        {
            this.CreateStatistics();
        }

        public Queue(string nameIn, FLOWObject parentIn, int capacityIn, Node nodeIn)
            : base(nameIn, parentIn, capacityIn)
        {
            this.content = new UnitloadList();
        }

        [XmlIgnore()]
        public UnitloadList QueueContent
        {
            get { return this.content; }
            set { this.content = value; }
        }

        public void ClearStatistics(double timeIn)
        {
            Statistics length = this.Statistics["Length"];
            length.Clear(timeIn, this.Content.Count);
            Statistics time = this.Statistics["Time"];
            time.Clear(timeIn, 0);
        }

        public void CreateStatistics()
        {
            this.Statistics.Add("Length", new Statistics(0));
            this.Statistics.Add("Time", new Statistics(0));
        }

        public void FinalizeStatistics(double timeIn)
        {
            Statistics length = this.Statistics["Length"];
            length.UpdateWeighted(timeIn, this.Content.Count);
        }

        public override string GetInformation()
        {
            StringWriter writer = new StringWriter();
            writer.WriteLine(base.GetInformation());
            writer.WriteLine("Capacity : {0}", this.Capacity.ToString());
            return writer.ToString().Trim();
        }

        public void Receive(double timeIn, Unitload unitloadIn)
        {
            this.Content.Add(unitloadIn);
            Statistics length = this.Statistics["Length"];
            length.UpdateWeighted(timeIn, this.Content.Count);
            unitloadIn.ChangeLocation(timeIn, this);
            this.Reserved--;
        }

        public void Release(double timeIn, Unitload unitloadIn)
        {
            this.Content.Remove(unitloadIn);
            Statistics length = this.Statistics["Length"];
            length.UpdateWeighted(timeIn, this.Content.Count);
            Statistics time = this.Statistics["Time"];
            time.UpdateAverage(timeIn, (timeIn - unitloadIn.EntryTime));
            unitloadIn.Location = null;
        }
    }

    [XmlType("QueueState")]
    public class QueueState : StaticObjectState
    {
        public QueueState()
        {
        }

        public void GetState(Queue queueIn)
        {
            base.GetState(queueIn);
        }

        public void SetState(Queue queueIn, SimulationManager managerIn)
        {
            JobManager jobManager = managerIn.JobManager;
            queueIn.Content.Clear();
            foreach (string unitloadName in this.Content)
            {
                Unitload unitload = jobManager.Unitloads[unitloadName];
                queueIn.Receive(unitload.EntryTime, unitload);
            }
            base.SetState(queueIn);
        }
    }

}

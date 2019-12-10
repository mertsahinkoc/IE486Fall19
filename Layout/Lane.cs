using System;
using System.IO;
using System.Xml.Serialization;
using FLOW.NET.Operational;

namespace FLOW.NET.Layout
{
    [XmlType("Lane")]
    public class Lane : StaticObject
    {
        private Node begin;

        private string beginName;

        private Node end;

        private string endName;

        private double length;

        [XmlIgnore()]
        public Node Begin
        {
            get { return this.begin; }
            set
            {
                this.begin = value;
                if (value != null)
                {
                    this.beginName = value.Name;
                    if (this.end != null)
                    {
                        this.length = this.CalculateLength();
                    }
                }
                else
                {
                    this.beginName = String.Empty;
                    this.length = 0;
                }
            }
        }

        [XmlElement("BeginName")]
        public string BeginName
        {
            get { return this.beginName; }
            set { this.beginName = value; }
        }

        [XmlIgnore()]
        public Node End
        {
            get { return this.end; }
            set
            {
                this.end = value;
                if (value != null)
                {
                    this.endName = value.Name;
                    if (this.begin != null)
                    {
                        this.length = this.CalculateLength();
                    }
                }
                else
                {
                    this.endName = String.Empty;
                    this.length = 0;
                }
            }
        }

        [XmlElement("EndName")]
        public string EndName
        {
            get { return this.endName; }
            set { this.endName = value; }
        }

        [XmlElement("Length")]
        public double Length
        {
            get { return this.length; }
            set { this.length = value; }
        }

        private double CalculateLength()
        {
            double x1 = this.begin.Location.X;
            double y1 = this.begin.Location.Y;
            double x2 = this.end.Location.X;
            double y2 = this.end.Location.Y;
            return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }

        public void ClearStatistics(double timeIn)
        {
            Statistics busy = this.Statistics["Busy"];
            if (this.IsBusy == true)
            {
                busy.Clear(timeIn, 1);
            }
            else
            {
                busy.Clear(timeIn, 0);
            }
            Statistics blocked = this.Statistics["Blocked"];
            if (this.IsBlocked == true)
            {
                blocked.Clear(timeIn, 1);
            }
            else
            {
                blocked.Clear(timeIn, 0);
            }
        }

        public void CreateStatistics()
        {
            this.Statistics.Add("Busy", new Statistics(0));
            this.Statistics.Add("Blocked", new Statistics(0));
        }

        public void FinalizeStatistics(double timeIn)
        {
            Statistics busy = this.Statistics["Busy"];
            if (this.IsBusy == true)
            {
                busy.UpdateWeighted(timeIn, 1);
            }
            else
            {
                busy.UpdateWeighted(timeIn, 0);
            }
            Statistics blocked = this.Statistics["Blocked"];
            if (this.IsBlocked == true)
            {
                blocked.UpdateWeighted(timeIn, 1);
            }
            else
            {
                blocked.UpdateWeighted(timeIn, 0);
            }
        }

        public override string GetInformation()
        {
            StringWriter writer = new StringWriter();
            writer.WriteLine(base.GetInformation());
            writer.WriteLine("Begin Node : {0}", this.begin.Name);
            writer.WriteLine("End Node : {0}", this.end.Name);
            writer.WriteLine("Length : {0}", this.length);
            return writer.ToString().Trim();
        }

        public void Receive(double timeIn, MovableObject objectIn)
        {
            this.Content.Add(objectIn);
            Statistics busy = this.Statistics["Busy"];
            busy.UpdateWeighted(timeIn, 1);
            objectIn.ChangeLocation(timeIn, this);
        }

        public void Release(double timeIn, MovableObject objectIn)
        {
            this.Content.Remove(objectIn);
            if (this.IsBusy == false)
            {
                Statistics busy = this.Statistics["Busy"];
                busy.UpdateWeighted(timeIn, 0);
            }
            objectIn.Location = null;
        }
    }

    [XmlType("LaneState")]
    public class LaneState : StaticObjectState
    {
        public LaneState()
        {
        }

        public void GetState(Lane laneIn)
        {
            base.GetState(laneIn);
        }

        public void SetState(Lane laneIn, SimulationManager managerIn)
        {
            Layout layout = managerIn.LayoutManager.Layout;
            laneIn.Content.Clear();
            base.SetState(laneIn);
        }
    }
}

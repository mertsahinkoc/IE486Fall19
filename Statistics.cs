using System;
using System.Xml.Serialization;

namespace FLOW.NET
{
    public class Statistics
    {
        private int count;
        private double maximum;
        private double minimum;
        private double sumSquare;
        private double time;
        private double total;
        private double value;

        public Statistics(double valueIn)
        {
            this.maximum = Double.NegativeInfinity;
            this.minimum = Double.PositiveInfinity;
            this.value = valueIn;
        }

        #region GETSET functions
        public int Count
        {
            get { return this.count; }
            set { this.count = value; }
        }

        public double Maximum
        {
            get { return this.maximum; }
            set { this.maximum = value; }
        }

        public double Minimum
        {
            get { return this.minimum; }
            set { this.minimum = value; }
        }

        public double SumSquare
        {
            get { return this.sumSquare; }
            set { this.sumSquare = value; }
        }

        public double Time
        {
            get { return this.time; }
            set { this.time = value; }
        }

        public double Total
        {
            get { return this.total; }
            set { this.total = value; }
        }

        public double Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        #endregion

        public void Clear(double timeIn, double valueIn)
        {
            this.count = 0;
            this.maximum = Double.NegativeInfinity;
            this.minimum = Double.PositiveInfinity;
            this.sumSquare = 0;
            this.time = timeIn;
            this.total = 0;
            this.value = valueIn;
        }

        public void UpdateAverage(double timeIn, double sampleIn)
        {
            this.time = timeIn;
            this.total += sampleIn;
            this.count++;
            this.sumSquare += sampleIn * sampleIn;
            if (this.minimum > sampleIn)
            {
                this.minimum = sampleIn;
            }
            else
            {
                if (this.maximum < sampleIn)
                {
                    this.maximum = sampleIn;
                }
            }
        }

        public void UpdateCount(double timeIn, int changeIn)
        {
            this.time = timeIn;
            this.count += changeIn;
        }

        public void UpdateTotal(double timeIn, double valueIn)
        {
            this.time = timeIn;
            this.total += valueIn;
            this.count++;
        }

        public void UpdateWeighted(double timeIn, double valueIn)
        {
            double delta = timeIn - this.time;
            this.total += this.value * delta;
            this.count++;
            this.sumSquare += this.value * this.value * delta;
            this.value = valueIn;
            this.time = timeIn;
            if (this.minimum > this.value)
            {
                this.minimum = this.value;
            }
            else
            {
                if (this.maximum < this.value)
                {
                    this.maximum = this.value;
                }
            }
        }
    }

    [XmlType("StatisticsState")]
    public class StatisticsState
    {
        private int count;
        private double maximum;
        private double minimum;
        private string name;
        private double sumSquare;
        private double time;
        private double total;
        private double value;

        public StatisticsState()
        {
        }

        #region GETSET functions
        [XmlElement("Count")]
        public int Count
        {
            get { return this.count; }
            set { this.count = value; }
        }

        [XmlElement("Maximum")]
        public double Maximum
        {
            get { return this.maximum; }
            set { this.maximum = value; }
        }

        [XmlElement("Minimum")]
        public double Minimum
        {
            get { return this.minimum; }
            set { this.minimum = value; }
        }

        [XmlElement("Name")]
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        [XmlElement("SumSquare")]
        public double SumSquare
        {
            get { return this.sumSquare; }
            set { this.sumSquare = value; }
        }

        [XmlElement("Time")]
        public double Time
        {
            get { return this.time; }
            set { this.time = value; }
        }

        [XmlElement("Total")]
        public double Total
        {
            get { return this.total; }
            set { this.total = value; }
        }

        [XmlElement("Value")]
        public double Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        #endregion

        public void GetState(Statistics statisticsIn, string nameIn)
        {
            this.count = statisticsIn.Count;
            this.maximum = statisticsIn.Maximum;
            this.minimum = statisticsIn.Minimum;
            this.name = nameIn;
            this.sumSquare = statisticsIn.SumSquare;
            this.time = statisticsIn.Time;
            this.total = statisticsIn.Total;
            this.value = statisticsIn.Value;
        }

        public void SetState(Statistics statisticsIn)
        {
            statisticsIn.Count = this.count;
            statisticsIn.Maximum = this.maximum;
            statisticsIn.Minimum = this.minimum;
            statisticsIn.SumSquare = this.sumSquare;
            statisticsIn.Time = this.time;
            statisticsIn.Total = this.total;
            statisticsIn.Value = this.value;
        }
    }
}
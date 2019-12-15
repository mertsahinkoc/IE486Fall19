using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FLOW.NET.Layout;


namespace FLOW.NET
{
    public class TransferTask
    {
        private BinList listOfBin;
        private Storage location;
        private double issueTime;
        private double boardingTime;

        public TransferTask() { }

        public BinList ListOfBin
        {
            get { return this.listOfBin; }
            set { this.listOfBin = value; }
        }
        public Storage Location
        {
            get { return this.location; }
            set { this.location = value; }
        }
        public double IssueTime
        {
            get { return this.issueTime; }
            set { this.issueTime = value; }
        }
        public double BoardingTime
        {
            get { return this.boardingTime; }
            set { this.boardingTime = value; }
        }

    }
}

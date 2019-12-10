using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FLOW.NET.Layout;
using FLOW.NET.Operational;

namespace FLOW.NET.Collections
{
    public class PullDecision 
    {
        private Queue inQueue;
        private Unitload unitload;

        public PullDecision()
        {
            this.inQueue = new Queue();
            this.unitload = new Unitload();
        }

        public Queue InQueue
        {
            get { return this.inQueue; }
            set { this.inQueue = value; }
        }
        public Unitload Unitload
        {
            get { return this.unitload; }
            set { this.unitload = value; }
        }
    }
}

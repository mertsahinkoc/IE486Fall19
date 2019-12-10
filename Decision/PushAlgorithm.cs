using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FLOW.NET.Layout;
using FLOW.NET.Operational;
using FLOW.NET.Decision;
using FLOW.NET.Collections;

namespace FLOW.NET.Decision
{
    public abstract class PushAlgorithm : DecisionAlgorithm
    {
        public void Execute()
        {
            UnitloadList currentList = this.Manager.JobManager.UnitloadsToPush;
            this.PreOperation();
            this.AlwaysPushing(currentList);
            this.PostOperation();
        }

        protected void PostOperation()
        {
            
        }
        
        public virtual void AlwaysPushing(UnitloadList currentlist)
        {

        }

        protected virtual void PreOperation() { }

        //key function to define unitload prioritizing for different implementations


        public static PushAlgorithm GetAlgorithmByName(string nameIn)
        {
            switch (nameIn)
            {
                case "AlwaysPush":
                    return new Push.AlwaysPush();
                default:
                    throw new Exception("Invalid job type finish algorithm name.");
            }
        }

        public static StringList GetAlgorithmNames()
        {
            StringList names = new StringList();
            names.Add("NonDelaySPT");
            return names;
        }
    }
}

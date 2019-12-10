using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FLOW.NET.Layout;
using FLOW.NET.Operational;

namespace FLOW.NET.Decision
{
    public abstract class ProcessorSelectionAlgorithm : DecisionAlgorithm
    {
        //ProcessorSelectionAlgorithm
        public Processor Execute(Unitload unitloadIn)
        {
            this.PreOperation();
            ProcessorOperationCouple decision = this.Select(unitloadIn); 	
            this.PostOperation(decision, unitloadIn);
            return decision.Processor;
        }


        protected void PostOperation(ProcessorOperationCouple decision,Unitload unitloadIn)
        {
            unitloadIn.Operation = decision.Operation;
        }


        protected virtual void PreOperation() { }

        //key function to define unitload prioritizing for different implementations


        public abstract ProcessorOperationCouple Select(Unitload unit);

        public static StringList GetAlgorithmNames()
        {
            StringList names = new StringList();
            names.Add("NonDelaySPT");
            return names;
        }

        public static ProcessorSelectionAlgorithm GetAlgorithmByName(string nameIn)
        {
            switch (nameIn)
            {
                case "NonDelaySPT":
                    return new ProcessorSelection.NonDelaySPT();
                default:
                    throw new Exception("Invalid job type finish algorithm name.");
            }
        }

        //make your general function definitions here
    }
}

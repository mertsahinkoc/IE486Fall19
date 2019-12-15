using System;
using System.Collections.Generic;
using System.Text;
using FLOW.NET.Layout;
using FLOW.NET.Operational;

namespace FLOW.NET.Decision
{
    public abstract class PrepareTransferTaskAlgorithm : DecisionAlgorithm
    {
        public void Execute(BinList binListIn)
        {
            this.PreOperation();
            if (binListIn.Count != 0)
            {
                this.CreateNewTasks(binListIn);
            }
            this.PostOperation();
        }
        protected virtual void PostOperation() { }
        protected virtual void PreOperation() { }

        protected abstract void CreateNewTasks(BinList binListIn); //creating new tasks for different implementations
        public static PrepareTransferTaskAlgorithm GetAlgorithmByName(string nameIn)
        {
            //add your algorithm names here for initialization
            switch (nameIn)
            {
                case "NoBatching":
                    return new PrepareTransferTask.NoBatching();
                default:
                    throw new Exception("Invalid part sequencing for processor algorithm name.");
            }
        }

        public static StringList GetAlgorithmNames()
        {

            StringList names = new StringList();
            names.Add("NoBatching");
            return names;

        }


    }
}

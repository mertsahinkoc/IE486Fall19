using System;
using System.Collections.Generic;
using System.Text;
using FLOW.NET.Layout;
using FLOW.NET.Operational;

namespace FLOW.NET.Decision
{
    public abstract class PartSequencingForProcessorAlgorithm : DecisionAlgorithm
    {
        public UnitloadList Execute(UnitloadList unitloadsIn)
        {
            UnitloadList Unitloads = new UnitloadList();
            this.PreOperation();
            if (unitloadsIn.Count != 0)
            {
                Unitloads = this.UnitloadListSelection(unitloadsIn);
            }
            this.Prioritize(unitloadsIn);
            this.PostOperation();
            return Unitloads;
        }

        protected virtual void PostOperation() { }

        protected virtual void PreOperation() { }
        protected abstract UnitloadList UnitloadListSelection(UnitloadList unitloads);
        //key function to define unitload prioritizing for different implementations
        protected abstract void Prioritize(UnitloadList unitloadsToProcess);

        public static PartSequencingForProcessorAlgorithm GetAlgorithmByName(string nameIn)
        {
            //add your algorithm names here for initialization
            switch (nameIn)
            {
                case "FirstComeMustGo":
                    return new PartSequencingForProcessor.FirstComeMustGo();
                default:
                    throw new Exception("Invalid part sequencing for processor algorithm name.");
            }
        }

        public static StringList GetAlgorithmNames()
        {

            StringList names = new StringList();
            names.Add("FirstComeMustGo");
            return names;

        }

        //make your general function definitions here
    }
}

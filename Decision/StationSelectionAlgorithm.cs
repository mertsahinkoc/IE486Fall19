using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FLOW.NET.Layout;
using FLOW.NET.Operational;

namespace FLOW.NET.Decision
{
    public abstract class StationSelectionAlgorithm : DecisionAlgorithm
    {

        public void Execute(Unitload unitloadIn)
        {
            this.PreOperation();
            SelectStation(unitloadIn);

            this.PostOperation();

        }


        protected void PostOperation()
        {
           
        }


        protected virtual void PreOperation() { }

        //key function to define unitload prioritizing for different implementations


        public abstract void SelectStation(Unitload unitloadIn);

        public static StringList GetAlgorithmNames()
        {
            StringList names = new StringList();
            names.Add("SelectionOfFirstStation");
            return names;
        }

        public static StationSelectionAlgorithm GetAlgorithmByName(string nameIn)
        {
            switch (nameIn)
            {
                case "SelectionOfFirstStation":
                    return new StationSelection.SelectionOfFirstStation();
                default:
                    throw new Exception("Invalid job type finish algorithm name.");
            }
        }
    }
}

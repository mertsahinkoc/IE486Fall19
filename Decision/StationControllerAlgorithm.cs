using System;
using System.Collections.Generic;
using System.Text;
using FLOW.NET.Layout;
using FLOW.NET.Operational;

namespace FLOW.NET.Decision
{
    public abstract class StationControllerAlgorithm : DecisionAlgorithm
    {
        public void Execute()
        {
            this.PreOperation();
            StationList stationList = new StationList();

            if (this.Manager.LayoutManager.StationsToDecide != null)
            {
                stationList = this.Manager.LayoutManager.StationsToDecide;
            }
            //some logic here

            foreach (Station station in stationList)
            {
                this.ControlStation(station);
            }
           

            this.PostOperation();
            
        }

        protected virtual void PostOperation() { }

        protected virtual void PreOperation() { }

        protected abstract void ControlStation(FLOW.NET.Layout.Station stationIn);

        //key function to define controlling strategies for different implementations
               

       
        public static StationControllerAlgorithm GetAlgorithmByName(string nameIn)
        {
            //add your algorithm names here for initialization
            switch (nameIn)
            {
                case "MixedProcessorConfiguration":
                    return new StationController.MixedConfiguration();
                default:
                    throw new Exception("Invalid station controller algorithm name.");
            }
        }

        public static StringList GetAlgorithmNames()
        {

            StringList names = new StringList();
            names.Add("MixedProcessorConfiguration");
            return names;

        }

        //make your general function definitions here
    }


}

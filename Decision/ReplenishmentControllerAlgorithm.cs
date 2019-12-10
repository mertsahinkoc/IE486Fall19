using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FLOW.NET.Layout;
using FLOW.NET.Operational;

namespace FLOW.NET.Decision
{
    public abstract class ReplenishmentControllerAlgorithm : DecisionAlgorithm
    {
        public void Execute()
        {
            if(this.Manager.Time > 32)
            {

            }
            this.PreOperation();
            RequestList requestList = new RequestList();

            if (this.Manager.LayoutManager.RequestsToReplenish != null)
            {
                requestList = this.Manager.LayoutManager.RequestsToReplenish;
            }
            //some logic here


                this.ControlRequest(requestList);

          

            this.PostOperation();

        }

        protected virtual void PostOperation() { }

        protected virtual void PreOperation() { }

        protected abstract void ControlRequest(RequestList requestListIn);

        //key function to define controlling strategies for different implementations



        public static ReplenishmentControllerAlgorithm GetAlgorithmByName(string nameIn)
        {
            //add your algorithm names here for initialization
            switch (nameIn)
            {
                case "ByPassReplenish":
                    return new ReplenishmentController.ByPassReplenish();
                default:
                    throw new Exception("Invalid station controller algorithm name.");
            }
        }

        public static StringList GetAlgorithmNames()
        {

            StringList names = new StringList();
            names.Add("ByPassReplenish");
            return names;

        }

        //make your general function definitions here



    }


}


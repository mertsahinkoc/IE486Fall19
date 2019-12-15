using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FLOW.NET.Layout;
using FLOW.NET.Operational;

namespace FLOW.NET.Decision
{
    public abstract class OrderControllerAlgorithm : DecisionAlgorithm
    {
        public void Execute()
        {
            if(this.Manager.Time > 32)
            {

            }
            this.PreOperation();
            OrderList orderList = new OrderList();

            if (this.Manager.LayoutManager.OrdersToReplenish != null)
            {
                orderList = this.Manager.LayoutManager.OrdersToReplenish;
            }
            //some logic here


                this.ControlOrder(orderList);

          

            this.PostOperation();

        }

        protected virtual void PostOperation() { }

        protected virtual void PreOperation() { }

        protected abstract void ControlOrder(OrderList orderListIn);

        //key function to define controlling strategies for different implementations



        public static OrderControllerAlgorithm GetAlgorithmByName(string nameIn)
        {
            //add your algorithm names here for initialization
            switch (nameIn)
            {
                case "ByPassReplenish":
                    return new OrderController.ByPassWarehouse();
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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FLOW.NET.Layout;
using FLOW.NET.Operational;

//Bypass algorithm is used instead of supermarkets in ie486f18

namespace FLOW.NET.Decision.OrderController
{
    public class ByPassWarehouse : OrderControllerAlgorithm
    {
        //whenever there is a request, create an event(bypass)
        protected override void ControlOrder(OrderList orderListIn)   
        {
            Dictionary<Storage, BinList> eventsToCreate = new Dictionary<Storage, BinList>();
            Transporter transporter = this.Manager.LayoutManager.Layout.Transporter;
            foreach (Order order in orderListIn)
            {
                Bin bin = new Bin(order.ComponentType + "-Bin", this.Manager.LayoutManager.Layout, order.ComponentType);
                this.Manager.LayoutManager.Layout.Bins.Add(bin);
                bin.Destination = order.Owner;
                transporter.Receive(this.Manager.Time, bin);
                if(eventsToCreate.Keys.Contains(order.Owner))
                {
                    eventsToCreate[order.Owner].Add(bin); 
                }
                else
                {
                    eventsToCreate.Add(order.Owner, new BinList());
                    eventsToCreate[order.Owner].Add(bin);
                }
            }
            
            foreach(Storage storage in eventsToCreate.Keys)
            {
                double time=this.Manager.Time;
                time += transporter.TravelTime.GenerateValue();
                for(int i = 0; i < eventsToCreate[storage].Count; i++)
                {
                    time += transporter.TransferTime.GenerateValue();
                }
                this.Manager.EventCalendar.ScheduleEndLoadUnloadEvent(time, eventsToCreate[storage], transporter,(BinMagazine)storage);
            }

            orderListIn.Clear();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FLOW.NET.Layout;
using FLOW.NET.Operational;

//Bypass algorithm is used instead of supermarkets in ie486f18

namespace FLOW.NET.Decision.ReplenishmentController
{
    public class ByPassReplenish : ReplenishmentControllerAlgorithm
    {
        //whenever there is a request, create an event(bypass)
        protected override void ControlRequest(RequestList requestListIn)   
        {
            Dictionary<Storage, BinList> eventsToCreate = new Dictionary<Storage, BinList>();
            Transporter transporter = this.Manager.LayoutManager.Layout.Transporter;
            foreach (Request request in requestListIn)
            {
                Bin bin = new Bin(request.ComponentType + "-Bin", this.Manager.LayoutManager.Layout, request.ComponentType);
                this.Manager.LayoutManager.Layout.Bins.Add(bin);
                bin.Destination = request.Owner;
                transporter.Receive(this.Manager.Time, bin);
                if(eventsToCreate.Keys.Contains(request.Owner))
                {
                    eventsToCreate[request.Owner].Add(bin); 
                }
                else
                {
                    eventsToCreate.Add(request.Owner, new BinList());
                    eventsToCreate[request.Owner].Add(bin);
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

            requestListIn.Clear();
        }
    }
}

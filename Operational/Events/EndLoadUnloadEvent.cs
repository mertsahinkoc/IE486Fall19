using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FLOW.NET.Layout;

namespace FLOW.NET.Operational.Events
{
    public class EndLoadUnloadEvent : Event
    {
        private Transporter transporter;
        private BinMagazine binMagazine;
        private BinList binsToUnload;  
        public EndLoadUnloadEvent()
        {

        }
        public EndLoadUnloadEvent(double timeIn, BinList binsToUnloadIn, SimulationManager managerIn, Transporter transporterIn, BinMagazine
            binMagazineIn)  
           : base(timeIn, managerIn)
        {
            this.transporter = transporterIn;
            this.binMagazine = binMagazineIn;
            this.binsToUnload = binsToUnloadIn;
        }
        public override EventState GetEventState()
        {
            throw new NotImplementedException();
        }


        protected override void Operation()   
        {
            for(int i = 0; i < this.binsToUnload.Count; i++) //bypass 
            {
                Bin bin = this.binsToUnload[i]; //search over just events' bins
                if ((BinMagazine)bin.Destination == this.binMagazine) 
                {
                    this.transporter.Release(this.Time, bin); 
                    Bin binToCollect = this.binMagazine.GetBinWithMinimumCount(bin.ComponentType);
                    this.binMagazine.Release(this.Time, binToCollect); 
                    //this.transporter.Receive(this.Time, binToCollect); Just for bypass, since we dont collect empty bins now
                    this.Manager.LayoutManager.Layout.Bins.Remove(binToCollect);  //delete empty bins
                    this.binMagazine.LoadBin(this.Time, bin,false); //statistics update
                    this.Manager.TriggerStationControllerAlgorithm((Station)this.binMagazine.Parent);            
                }
                else
                {
                    throw new Exception("buraya girmemeliydi :(");
                }
            }
        }

        protected override void TraceEvent()
        {
        }
    }
}

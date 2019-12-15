using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FLOW.NET.Layout;
using FLOW.NET.Operational;


namespace FLOW.NET.Decision.SupermarketController
{
    public class MixedSupermarketConfiguration : SupermarketControllerAlgorithm
    {
        protected override void ControlSupermarket(Storage supermarketIn)
        {
            CheckAvailability(supermarketIn);
            Picking(supermarketIn);
            this.Manager.Algorithms.PrepareTransferTaskAlgorithm.Execute(supermarketIn.ReadyBinList);
            CheckDock(supermarketIn);
        }
        public void CheckAvailability(Storage supermarketIn)
        {
            supermarketIn.UnassignedOrderList = this.Manager.Algorithms.OrderPriorityAlgorithm.Execute(supermarketIn.UnassignedOrderList);
            foreach(Order o in supermarketIn.UnassignedOrderList)
            {
                foreach(Bin b in supermarketIn.UnassignedBinList)
                {
                    if(o.ComponentType == b.ComponentType)
                    {
                        b.Order = o;
                        supermarketIn.UnassignedOrderList.Remove(o);
                        supermarketIn.UnassignedBinList.Remove(b);
                        CheckOrderToWarehouse(b.ComponentType,supermarketIn);
                        supermarketIn.AssignedBinList.Add(b);
                        //statistics update kısmı daigramdaki hali aşağıda ama eksik bence
                        unassignedOrderCount.UpdateWeighted(((SimulationManger)this.Manager).Time, supermarketIn.UnassignedOrderList.Count);
                        unassignedBinCount.UpdateWeighted(((SimulationManger)this.Manager).Time, supermarketIn.UnassignedBinList.Count);
                        assignedBinCount.UpdateWeighted(((SimulationManger)this.Manager).Time, supermarketIn.AssignedBinList.Count);
                        break;
                    }
                }
            }
        }
        public void Picking(Storage supermarketIn)
        {
            supermarketIn.AssignedBinList = this.Manager.Algorithms.BinPriorityAlgorithm.Execute(supermarketIn.AssignedBinList);
            while (supermarketIn.AvailablePicker > 0)
            {
                Bin bin = AssignedBinList[0];
                ScheduleEndPick(bin, ((SimulationManger)this.Manager).Time/* +bin.PickingTime.GenerateValue()*/);
                supermarketIn.AssignedBinList.Remove(bin);
                supermarketIn.AvailablePicker--;
                //buraya utilization istatistik
                if (supermarketIn.AssignedBinList <= 0)
                {
                    break;
                }
            }
        }
        public void CheckDock(Storage supermarketIn)
        {
            foreach(Transporter transporter in supermarketIn.ReadyTransportersAtDock)
            {
                if(transporter.inTransfer == false)
                {
                    if (transporter.AssignedTask[0] == null)
                    {
                        ScheduleEndLoad(((SimulationManger)this.Manager).Time, transporter);
                        transporter.inTransfer == true;
                    }
                }
                else//yeri yanlış bence bunun içteki if'in else'i olması lazım 
                {
                    //ScheduleDepart(); //bknz. Notlar 32
                }

            }
        }
        public void CheckOrderToWarehouse(ComponentType componentTypeIn ,Storage supermarketIn)
        {
            double inventoryPosition = GetComponentLevel(componentTypeIn,supermarketIn);//notlar 27'de bahsettiğim durum
            if(inventoryPosition < supermarketIn.inventoryPolicy[componentTypeIn].S)
            {
                Statistics OutstandingOrderCount = this.Statistics["OutstandingOrderCount"];
                for(int i=0; i< supermarketIn.inventoryPolicy[componentTypeIn].Q; i++)
                {
                    Order order = new Order(supermarketIn, componentTypeIn, ((SimulationManger)this.Manager).Time);
                    supermarketIn.OutstandingOrder.Add(order);
                    this.Manager.OpenOrder(order);
                }
                OutstandingOrderCount.UpdateWeighted(((SimulationManger)this.Manager).Time, supermarketIn.OutstandingOrder.Count);
            }

        }
    }
}

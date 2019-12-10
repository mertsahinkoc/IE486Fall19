using System;
using FLOW.NET.Layout;
using FLOW.NET.Operational;

namespace FLOW.NET.Decision.OrderRelease
{
    public class BlinkImmediate : OrderReleaseAlgorithm
    {
        protected override UnitloadList SelectUnitloads(Station inputStationIn, UnitloadList unitloadsIn)
        {
            UnitloadList selectedUnitloads = new UnitloadList();
            return selectedUnitloads;
        }

        protected override void PostOperation(Station inputStationIn, UnitloadList selectedUnitloadsIn, System.Collections.ArrayList names)
        {
            this.Manager.JobManager.UnitloadsToDecide.AddRange(selectedUnitloadsIn);
            this.Manager.Algorithms.OperationSelection.Execute();
            foreach (Unitload unitload in selectedUnitloadsIn)
            {
                if(unitload.Station.InQueue.IsAvailable == true)
                {
                    unitload.InTransfer = true;
                    unitload.Station.InQueue.Reserved++;
                    double releaseTime = 0;
                }
            }
        }
    }
}

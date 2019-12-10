using System;
using FLOW.NET.Layout;
using FLOW.NET.Operational;

namespace FLOW.NET.Decision.OrderRelease
{
    public class CyclicalImmediate : OrderReleaseAlgorithm
    {
        protected override UnitloadList SelectUnitloads(Station inputStationIn, UnitloadList unitloadsIn)
        {
            UnitloadList selectedUnitloads = new UnitloadList();
            return selectedUnitloads;
        }

        protected override void PostOperation(Station inputStationIn, UnitloadList selectedUnitloadsIn, System.Collections.ArrayList names)
        {
            foreach (Unitload unitload in selectedUnitloadsIn)
            {
                unitload.InTransfer = true;
                double transferTime = this.Manager.Time + inputStationIn.TransferTime.GenerateValue();
                Job job = (Job)unitload.Parent;
                names.Add(job.Name);
                if (job.StartTime == Double.PositiveInfinity)
                {
                    job.StartTime = this.Manager.Time;
                }
            }
        }

        protected override void PreOperation()
        {
            StationList inputStations = this.Manager.LayoutManager.Layout.InputStations;
            foreach (Station inputStation in inputStations)
            {
                this.PerformCyclicalOrder(inputStation);
            }
        }
    }
}
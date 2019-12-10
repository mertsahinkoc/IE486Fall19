using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FLOW.NET.Layout;
using FLOW.NET.Operational;
using FLOW.NET.Decision;
using FLOW.NET.Collections;

//After endprocess event, unitloads try to go inqueue of their next station

namespace FLOW.NET.Decision.Push
{
    public class AlwaysPush : PushAlgorithm
    {
        public override void AlwaysPushing(UnitloadList currentListIn)
            {
                foreach (Unitload unit in currentListIn)
                {
                    Processor ProcessorToRelease = (Processor)unit.Location;
                    if (unit.Station.InQueue.IsAvailable == false)
                    {
                        this.Manager.JobManager.BlockedUnitloads.Add(unit);
                        ProcessorToRelease.ChangeBlocked(this.Manager.Time, true);
                    }//block		
                    else
                    {
                        //double transferTime = this.Manager.LayoutManager.Layout.DistanceMatrix[(Station)ProcessorToRelease.Parent, unit.Station];
                        double transferTime = 1;
                        unit.Station.InQueue.Reserved++;
                        unit.InTransfer = true;
                        this.Manager.LayoutManager.Layout.UnitloadsOnMover.Add(unit);
                        ProcessorToRelease.Release(this.Manager.Time, unit);
                        //operation and processor will be selected when the unitload is in inqueue of its station.
                        this.Manager.TriggerStationControllerAlgorithm((Station)ProcessorToRelease.Parent);
                        this.Manager.EventCalendar.ScheduleEnterQueueEvent(this.Manager.Time + transferTime, unit.Station.InQueue, unit);
                    }//go
                }
                currentListIn.Clear();
        }
    }
}


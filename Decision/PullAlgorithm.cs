using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FLOW.NET.Layout;
using FLOW.NET.Operational;
using FLOW.NET.Decision;
using FLOW.NET.Collections;

namespace FLOW.NET.Decision
{
    public abstract class PullAlgorithm : DecisionAlgorithm
    {
        //ProcessorSelectionAlgorithm
        public void Execute()
        {
            this.PreOperation();
            QueueList queueList = this.Manager.LayoutManager.QueuesToPull;
            UnitloadList unitloadList = this.Manager.JobManager.BlockedUnitloads;

            foreach (Queue queue in queueList)
            {
                while (queue.IsAvailable == true)
                {
                    PullDecision decision = this.PullUnitload(queue, unitloadList);
                    if (decision == null) break;
                    else
                    {
                        Processor ProcessorToRelease = ((Processor)decision.Unitload.Location);
                        Unitload unitloadToGo = decision.Unitload;
                        //int transferTime = this.Manager.LayoutManager.Layout.DistanceMatrix[(Station)ProcessorToRelease.Parent, unitloadToGo.Station]; ie486f18
                        int transferTime = 1;
                        queue.Reserved++;
                        unitloadToGo.InTransfer = true;
                        this.Manager.LayoutManager.Layout.UnitloadsOnMover.Add(unitloadToGo);
                        //geldiği processorün/inqueuecellin queusunun blockedı açılacak. (Release)
                        ProcessorToRelease.Release(this.Manager.Time, unitloadToGo);
                        ProcessorToRelease.ChangeBlocked(this.Manager.Time, false);
                        this.Manager.TriggerStationControllerAlgorithm((Station)ProcessorToRelease.Parent);
                        this.Manager.EventCalendar.ScheduleEnterQueueEvent(this.Manager.Time + transferTime,queue, unitloadToGo);
                        unitloadList.Remove(unitloadToGo);
                    }
                }
            }
            queueList.Clear();
            this.PostOperation();
        }


        protected void PostOperation()
        {
        }

        public abstract PullDecision PullUnitload(Queue inQueue, UnitloadList unitloadlist);

        protected virtual void PreOperation() { }


        //key function to define unitload prioritizing for different implementations


        public static StringList GetAlgorithmNames()
        {
            StringList names = new StringList();
            names.Add("FirstBlockedFirstServed");
            return names;
        }

        public static PullAlgorithm GetAlgorithmByName(string nameIn)
        {
            switch (nameIn)
            {
                case "FirstBlockedFirstServed":
                    return new Pull.FirstBlockedFirstServed();
                default:
                    throw new Exception("Invalid job type finish algorithm name.");
            }
        }

        //make your general function definitions here
    }
}

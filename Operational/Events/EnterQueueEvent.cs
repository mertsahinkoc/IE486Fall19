using System;
using System.Diagnostics;
using System.Xml.Serialization;
using FLOW.NET.Layout;

namespace FLOW.NET.Operational.Events
{
    public class EnterQueueEvent : Event
    {
        private Queue inqueue;
        private Unitload unitload;
        public EnterQueueEvent()
        {

        }

        public EnterQueueEvent(double timeIn, SimulationManager managerIn, Queue queueIn, Unitload unitloadIn)
           : base(timeIn, managerIn)
        {
            this.inqueue = queueIn;
            this.unitload = unitloadIn;
        }

        public override EventState GetEventState()
        {
            throw new NotImplementedException();
        }

        //removing arriving unitload from UnitloadsOnMover list and entering it to related queue
        protected override void Operation()
        {
            if (this.unitload.Location == null)
            {
                this.Manager.LayoutManager.Layout.UnitloadsOnMover.Remove(unitload); 
                unitload.Station.InQueue.Receive(this.Time, unitload);
                this.Manager.TriggerOperationDecisionAlgorithm(unitload);
                this.Manager.TriggerStationControllerAlgorithm(unitload.Station);
            }
        }

        protected override void TraceEvent()
        {
            Debug.WriteLine(String.Format("ENTERQUEUE [{0}, {1}]", this.Time, this.inqueue.Name));
        }
    }
}

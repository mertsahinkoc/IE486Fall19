using System;
using System.Xml.Serialization;
using FLOW.NET;
using FLOW.NET.Layout;
using FLOW.NET.Random;
using FLOW.NET.Decision;
using FLOW.NET.Operational;
using FLOW.NET.Operational.Events;

namespace FLOW.NET.Operational
{
    public class EventCalendar
    {
        private EventListDoubleDictionary events; //state

        private bool isPassive;

        private SimulationManager manager;

        public EventCalendar(SimulationManager managerIn)
        {
            this.manager = managerIn;
            this.events = new EventListDoubleDictionary();
        }

        public EventListDoubleDictionary Events
        {
            get { return this.events; }
            set { this.events = value; }
        }

        public bool IsPassive
        {
            get { return this.isPassive; }
            set { this.isPassive = value; }
        }

        public void Reset()
        {
            for (int i = this.events.Count - 1; i >= 0; i--)
            {
                EventList eventList = this.events.Values[i];
                for (int j = eventList.Count - 1; j >= 0; j--)
                {
                    if (eventList[j].IsPermanent == false)
                    {
                        eventList.RemoveAt(j);
                    }
                }
                if (eventList.Count == 0)
                {
                    this.events.RemoveAt(i);
                }
            }
        }

        public void ScheduleArrivalEvent(double timeIn, JobType jobTypeIn)
        {
            ArrivalEvent arrivalEvent = new ArrivalEvent(timeIn, this.manager, jobTypeIn);
            this.ScheduleEvent(arrivalEvent);
        }

        public void ScheduleEnterQueueEvent(double timeIn,Queue queueIn, Unitload unitloadIn)
        {
            EnterQueueEvent enterQueueEvent = new EnterQueueEvent(timeIn, this.manager,queueIn, unitloadIn);
            this.ScheduleEvent(enterQueueEvent);
        }

        public void ScheduleDepartureEvent(double timeIn, Job jobIn)
        {
            DepartureEvent departureEvent = new DepartureEvent(timeIn, this.manager, jobIn);
            this.ScheduleEvent(departureEvent);
        }

        public void ScheduleJobTypeFinishEvent(double timeIn, JobType jobTypeIn)
        {
            JobTypeFinishEvent jobTypeFinishEvent = new JobTypeFinishEvent(timeIn, this.manager, jobTypeIn);
            this.ScheduleEvent(jobTypeFinishEvent);
        }

        public void ScheduleEndPlanningPeriodEvent(double timeIn)
        {
            EndPlanningPeriodEvent endPlanningPeriodEvent = new EndPlanningPeriodEvent(timeIn, this.manager);
            this.ScheduleEvent(endPlanningPeriodEvent);
        }

        public void ScheduleSeizeNodeEvent(double timeIn, Transporter transporterIn)
        {
            SeizeNodeEvent seizeNodeEvent = new SeizeNodeEvent(timeIn, this.manager, transporterIn);
            this.ScheduleEvent(seizeNodeEvent);
        }
        public void ScheduleEndLoadEvent(double timeIn, Transporter transporterIn)
        {
            EndLoadEvent endLoadEvent = new EndLoadEvent(timeIn, this.manager, transporterIn);
            this.ScheduleEvent(endLoadEvent);
        }
        public void ScheduleEndUnloadEvent(double timeIn, Transporter transporterIn)
        {
            EndUnloadEvent endUnloadEvent = new EndUnloadEvent(timeIn, this.manager, transporterIn);
            this.ScheduleEvent(endUnloadEvent);
        }
        public void ScheduleByPassEvent(double timeIn, Bin binIn)
        {
            BypassEvent byPassEvent = new BypassEvent(timeIn, this.manager, binIn);
            this.ScheduleEvent(byPassEvent);
        }
        public void ScheduleEndPickEvent(double timeIn, Bin binIn)
        {
            EndPickEvent endPickEvent = new EndPickEvent(timeIn, this.manager, binIn);
            this.ScheduleEvent(endPickEvent);
        }

        public void ScheduleEndProcessEvent(double timeIn, Processor processorIn)
        {
            bool breakdown = false;
            foreach (EventList eList in events.Values)
            {
                foreach (Event e in eList)
                {
                    //do not endprocess event if processor was brokendown. 
                    if (e.Time < timeIn)
                    {
                        if (e.GetType().Name == "ProcessorBreakdownEvent")
                        {
                            if (((ProcessorBreakdownEvent)e).Processor == processorIn)
                            {
                                breakdown = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (breakdown == false)
            {
                EndProcessEvent endProcessEvent = new EndProcessEvent(timeIn, this.manager, processorIn);
                this.ScheduleEvent(endProcessEvent);
            }
        }

        public void ScheduleEndSimulationEvent(double timeIn)
        {
            EndSimulationEvent endSimulationEvent = new EndSimulationEvent(timeIn, this.manager);
            this.ScheduleEvent(endSimulationEvent);
        }

        public void ScheduleEndLoadUnloadEvent(double timeIn, BinList binsToUnloadIn, Transporter transporterIn, BinMagazine binMagazineIn)   //  486update 25.12.2018
        {
            EndLoadUnloadEvent endLoadUnloadEvent = new EndLoadUnloadEvent(timeIn, binsToUnloadIn, this.manager, transporterIn, binMagazineIn);
            this.ScheduleEvent(endLoadUnloadEvent);
        }

        public void ScheduleEndWarmupEvent(double timeIn)
        {
            EndWarmupEvent endWarmupEvent = new EndWarmupEvent(timeIn, this.manager);
            this.ScheduleEvent(endWarmupEvent);
        }

        public void ScheduleEvent(Event eventIn)
        {
            if (this.isPassive == false)
            {
                if (eventIn.Time >= this.manager.Time)
                {
                    EventList eventList = null;
                    if (this.events.ContainsKey(eventIn.Time) == true)
                    {
                        eventList = this.events[eventIn.Time];
                    }
                    else
                    {
                        eventList = new EventList();
                        this.events.Add(eventIn.Time, eventList);
                    }
                    eventList.Add(eventIn);
                }
            }
        }


        public void ScheduleProcessorRepairEvent(double timeIn, Processor processorIn)
        {
            ProcessorRepairEvent processorRepairEvent = new ProcessorRepairEvent(timeIn, this.manager, processorIn);
            this.ScheduleEvent(processorRepairEvent);
        }

        public void ScheduleProcessorBreakdownEvent(double timeIn, Processor processorIn)
        {
            ProcessorBreakdownEvent processorBreakdownEvent = new ProcessorBreakdownEvent(timeIn, this.manager, processorIn);
            this.ScheduleEvent(processorBreakdownEvent);
        }

        public void ScheduleStartPlanningPeriodEvent(double timeIn)
        {
            StartPlanningPeriodEvent startPlanningPeriodEvent = new StartPlanningPeriodEvent(timeIn, this.manager);
            this.ScheduleEvent(startPlanningPeriodEvent);
        }

        public void ScheduleStartProcessEvent(double timeIn, Processor processorIn, Unitload unitloadIn, double transfertimeIn)
        {
            StartProcessEvent startProcessEvent = new StartProcessEvent(timeIn, this.manager, processorIn, unitloadIn, transfertimeIn);
            this.ScheduleEvent(startProcessEvent);
        }

    }

    public class EventCalendarState
    {
        private EventStateList events;

        public EventCalendarState()
        {
            this.events = new EventStateList();
        }

        [XmlArray("Events")]
        [XmlArrayItem(typeof(EventState))]
        public EventStateList Events
        {
            get { return this.events; }
            set { this.events = value; }
        }

        public void GetState(EventCalendar eventCalendarIn)
        {
            foreach (EventList eventList in eventCalendarIn.Events.Values)
            {
                foreach (Event @event in eventList)
                {
                    this.events.Add(@event.GetEventState());
                }
            }
        }

        public void SetState(EventCalendar eventCalendarIn, SimulationManager managerIn)
        {
            eventCalendarIn.Events.Clear();
            foreach (EventState eventState in this.events)
            {
                eventCalendarIn.ScheduleEvent(eventState.GetEvent(managerIn));
            }
        }
    }
}
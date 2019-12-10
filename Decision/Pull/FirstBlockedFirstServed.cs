using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FLOW.NET.Operational;
using FLOW.NET.Layout;
using FLOW.NET.Collections;

//If the previous station is blocked, departure of one unit creates a pull signal for previous stations
//The blocked units are in layout's pullunitloadlist

namespace FLOW.NET.Decision.Pull
{
    public class FirstBlockedFirstServed : PullAlgorithm
    {
        public override PullDecision PullUnitload(Queue queueIn, UnitloadList unitloadListIn)
        {
            PullDecision decision = new PullDecision();
            //First blocked first served             
            foreach (Unitload unit in unitloadListIn)
            {
                if (unit.Station.InQueue == queueIn)
                {
                    decision.Unitload = unit;
                    decision.InQueue = unit.Station.InQueue;
                    return decision;
                   //first one added to blocked list is found, break
                }
            }
            return null;
        }
    }
}

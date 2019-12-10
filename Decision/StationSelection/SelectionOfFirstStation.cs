using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FLOW.NET.Operational;
using FLOW.NET.Layout;
using FLOW.NET.Collections;

//Always first alternate is chosen. No comparasion of time is done.

namespace FLOW.NET.Decision.StationSelection
{
    public class SelectionOfFirstStation : StationSelectionAlgorithm
    {
        public override void SelectStation(Unitload unitloadIn)
        {
            unitloadIn.Station = unitloadIn.Alternates[0].Operations[unitloadIn.Completed.Count].Station;
        }
    }
}

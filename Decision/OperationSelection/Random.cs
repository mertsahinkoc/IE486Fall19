using System;
using FLOW.NET.Layout;
using FLOW.NET.Operational;

namespace FLOW.NET.Decision.OperationSelection
{
    public class Random : OperationSelectionAlgorithm
    {
        protected override void SelectOperation(Unitload unitloadIn, OperationList operationsIn, StationList processCellsIn)
        {
            unitloadIn.Operation = unitloadIn.Alternates[0].Operations[unitloadIn.Completed.Count];
        }
    }
}

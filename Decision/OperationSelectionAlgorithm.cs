using System;
using FLOW.NET.Layout;
using FLOW.NET.Operational;

namespace FLOW.NET.Decision
{
    public abstract class OperationSelectionAlgorithm : DecisionAlgorithm
    {
        public void Execute()
        {
			UnitloadList unitloads = this.Manager.JobManager.UnitloadsToDecide;
            this.PreOperation(unitloads);
            
            foreach (Unitload unitload in unitloads)
            {
                OperationList operations = new OperationList();
                StationList processCells = new StationList();
                Station currentStation = unitload.Station;
                this.SelectOperation(unitload, operations, processCells); //IE486f18
            }
            unitloads.Clear();
            this.PostOperation();
        }

        public static OperationSelectionAlgorithm GetAlgorithmByName(string nameIn)
        {
            switch (nameIn)
            {
                case "Random":
                    return new OperationSelection.Random();
                default:
                    throw new Exception("Invalid operation selection algorithm name.");
            }
        }

        public static StringList GetAlgorithmNames()
        {
            StringList names = new StringList();
            names.Add("Random");
            names.Add("SmallestQueueWorkload");
            names.Add("EarliestExpectedFinishTime");
            names.Add("MinimumeEstimatedFlowTime");
            names.Add("AlternativeRouteingsDirectedDynamically");
            return names;
        }

        protected virtual void PostOperation() { }

        protected virtual void PreOperation(UnitloadList unitloadsIn) 
		{
		}

        protected abstract void SelectOperation(Unitload unitloadIn, OperationList operationsIn, StationList processCellsIn);

        protected void SendUnitloadToCentralBuffer(Unitload unitloadIn)
        {
            StaticObject location = null;
            if (unitloadIn.Location.GetType().Name == "Queue" || unitloadIn.Location.GetType().Name == "Processor")
            {
            }
            else
            {
                location = (StaticObject)((MovableObject)unitloadIn.Location).Location;
            }
            unitloadIn.Operation = null;
            //unitloadIn.Station = this.FindNearestCentralBuffer(location);
        }

		protected double waitingTimeAtNextStation(Station cellIn)
		{
			//when calculating waitingTimeNextStation there some assumptions:
			//   1. FCFS
			//   2. No new incomer to inqueue 
			double waitingTimeNextStation = 0;
			double averageProcessingTime = 0;
			int totalNumberOfParts = 0;
			foreach(Unitload unitload in cellIn.InQueue.Content)
			{
				averageProcessingTime += unitload.GetExpectedProcessTime(cellIn) + 2*cellIn.TransferTime.ExpectedValue();
				totalNumberOfParts++;
			}
			foreach(Processor processor in cellIn.Processors)
			{
				Unitload unitload = null;
				if(processor.Content.Count > 0)
					unitload = (Unitload)processor.Content[0];
				//check whether processors are busy or not
				//count only the processor which has work on them (which has unitload on them)
				if (unitload != null)
				{
					averageProcessingTime += Math.Max(0, unitload.GetExpectedProcessTime(cellIn) - (this.Manager.Time - unitload.EntryTime)) + cellIn.TransferTime.ExpectedValue();
					totalNumberOfParts++;
				}
			}
			
			//check division by zero
			if(totalNumberOfParts != 0)
				averageProcessingTime /= totalNumberOfParts;
			//reserved happens during in transfer so their processing time have not included above
			return waitingTimeNextStation;
		}
    }
}
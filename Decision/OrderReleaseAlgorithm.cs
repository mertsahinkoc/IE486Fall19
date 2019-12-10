using System;
using FLOW.NET.Layout;
using FLOW.NET.Operational;

namespace FLOW.NET.Decision
{
    public abstract class OrderReleaseAlgorithm : DecisionAlgorithm
    {
        private static System.Collections.ArrayList names = new System.Collections.ArrayList();

        public void Execute()
        {
            this.PreOperation();
            StationList inputStations = this.FindCandidateCells();
            foreach (Station inputStation in inputStations)
            {
                UnitloadList unitloads = inputStation.FindUnreleasedUnitloads();
                UnitloadList selectedUnitloads = this.SelectUnitloads(inputStation, unitloads);
                this.PostOperation(inputStation, selectedUnitloads,names);
            }
            
        }

        protected StationList FindCandidateCells()
        {
            StationList candidates = new StationList();
            StationList inputStations = this.Manager.LayoutManager.Layout.InputStations;
            return candidates;
        }

        public static OrderReleaseAlgorithm GetAlgorithmByName(string nameIn)
        {
            switch (nameIn)
            {
                case "CyclicalImmediate":
                    return new OrderRelease.CyclicalImmediate();
                case "Immediate":
                    return new OrderRelease.Immediate();
                case "BlinkImmediate":
                    return new OrderRelease.BlinkImmediate();
                default:
                    throw new Exception("Invalid order release algorithm name.");
            }
        }

        public static StringList GetAlgorithmNames()
        {
            StringList names = new StringList();
            names.Add("CyclicalImmediate");
            names.Add("Immediate");
            names.Add("BlinkCyclicalImmediate");
            names.Add("BlinkImmediate");
            return names;
        }

        protected void PerformCyclicalOrder(Station inputStationIn)
        {
            UnitloadListStringDictionary unitloads = new UnitloadListStringDictionary();
            UnitloadList orderedUnitloads = new UnitloadList();
            JobTypeList jobTypes = new JobTypeList();
            int totalRelease = 0;
            MovableObjectList queueContent = inputStationIn.InQueue.Content;
            foreach (Unitload unitload in queueContent)
            {
                if (unitload.EntryTime == this.Manager.Time)
                {
                    Job currentJob = (Job)unitload.Parent;
                    if (unitloads.ContainsKey(currentJob.JobType.Name) == false)
                    {
                        UnitloadList jobList = new UnitloadList();
                        unitloads.Add(currentJob.JobType.Name, jobList);
                        jobList.Add(unitload);
                        jobTypes.Add(currentJob.JobType);
                    }
                    else
                    {
                        UnitloadList jobList = unitloads[currentJob.JobType.Name];
                        jobList.Add(unitload);
                    }
                    totalRelease++;
                }
            }
            int[] frequency = new int[jobTypes.Count];
            int[] remaining = new int[jobTypes.Count];
            int[] minimumPosition = new int[jobTypes.Count];
            int index = 0;
            foreach (JobType jobType in jobTypes)
            {
                UnitloadList jobList = unitloads[jobType.Name];
                frequency[index] = (int)Math.Ceiling((double)totalRelease / jobList.Count);
                remaining[index] = jobList.Count;
                minimumPosition[index++] = 0;
            }
            while (totalRelease != 0)
            {
                int leastPosition = Int32.MaxValue;
                for (int i = 0; i < jobTypes.Count; i++)
                {
                    if (minimumPosition[i] < leastPosition)
                    {
                        leastPosition = minimumPosition[i];
                    }
                }
                int mostRemaining = 0;
                int selectedJobIndex = -1;
                for (int i = 0; i < jobTypes.Count; i++)
                {
                    if (minimumPosition[i] == leastPosition)
                    {
                        if (mostRemaining < remaining[i])
                        {
                            mostRemaining = remaining[i];
                            selectedJobIndex = i;
                        }
                    }
                }
                remaining[selectedJobIndex] = remaining[selectedJobIndex] - 1;
                if (remaining[selectedJobIndex] == 0)
                {
                    minimumPosition[selectedJobIndex] = Int32.MaxValue;
                }
                else
                {
                    if (minimumPosition[selectedJobIndex] <= orderedUnitloads.Count)
                    {
                        minimumPosition[selectedJobIndex] = minimumPosition[selectedJobIndex] + frequency[selectedJobIndex];
                    }
                }
                totalRelease--;
                UnitloadList jobList = unitloads[jobTypes[selectedJobIndex].Name];
                orderedUnitloads.Add(jobList[0]);
                jobList.RemoveAt(0);
            }
            queueContent.RemoveRange(queueContent.Count - orderedUnitloads.Count, orderedUnitloads.Count);
            foreach (Unitload unitload in orderedUnitloads)
            {
                queueContent.Add(unitload);
            }
        }

        protected virtual void PostOperation(Station inputStationIn, UnitloadList selectedUnitloadsIn, System.Collections.ArrayList names) { }

        protected virtual void PreOperation() { }

        protected abstract UnitloadList SelectUnitloads(Station inputStationIn, UnitloadList unitloadsIn);
    }
}
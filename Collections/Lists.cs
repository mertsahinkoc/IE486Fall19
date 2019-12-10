using System.Collections.Generic;
using FLOW.NET.Layout;
using FLOW.NET.Random;
using FLOW.NET.Operational;
using FLOW.NET.Operational.Events;

namespace FLOW.NET
{
    public class BufferCellList : List<BufferCell>
    {
        public BufferCell this[string name]
        {
            get
            {
                foreach (BufferCell bufferCell in this)
                {
                    if (bufferCell.Name == name)
                    {
                        return bufferCell;
                    }
                }
                return null;
            }
        }
    }

    public class BufferCellStateList : List<BufferCellState>
    {
    }

    public class CellList : List<Cell>
    {
        public Cell this[string name]
        {
            get
            {
                foreach (Cell cell in this)
                {
                    if (cell.Name == name)
                    {
                        return cell;
                    }
                }
                return null;
            }
        }
    }

    public class CellStateList : List<CellState>
    {
    }

    public class StationLoadingList : List<StationLoading>
    {
    }

    public class DoubleList : List<double>
    {
    }
    public class StorageList : List<Storage>
    {
        public Storage this[string name]
        {
            get
            {
                foreach (Storage storage in this)
                {
                    if (storage.Name == name)
                    {
                        return storage;
                    }
                }
                return null;
            }
        }
    }

    public class EventList : List<Event>
    {
    }
    public class RequestList : List<Request>
    {
    }
    public class EventStateList : List<EventState>
    {
    }

    public class RVGeneratorList : List<RVGenerator>
    {
    }

    public class RVGeneratorStateList : List<RVGeneratorState>
    {
    }

    public class IntegerList : List<int>
    {
    }

    public class JobList : List<Job>
    {
        public Job this[string name]
        {
            get
            {
                foreach (Job job in this)
                {
                    if (job.Name == name)
                    {
                        return job;
                    }
                }
                return null;
            }
        }
    }

    public class JobStateList : List<JobState>
    {
    }

    public class JobRouteList : List<JobRoute>
    {
        public JobRoute this[string name]
        {
            get
            {
                foreach (JobRoute jobRoute in this)
                {
                    if (jobRoute.Name == name)
                    {
                        return jobRoute;
                    }
                }
                return null;
            }
        }

        public JobRouteList Clone()
        {
            JobRouteList result = new JobRouteList();
            result.AddRange(this);
            return result;
        }
    }

    public class JobTypeList : List<JobType>
    {
        public JobType this[string name]
        {
            get
            {
                foreach (JobType jobType in this)
                {
                    if (jobType.Name == name)
                    {
                        return jobType;
                    }
                }
                return null;
            }
        }
    }

    public class JobTypeStateList : List<JobTypeState>
    {
    }

    //public class LaneList : List<Lane>
    //{
    //    public Lane this[string name]
    //    {
    //        get
    //        {
    //            foreach (Lane lane in this)
    //            {
    //                if (lane.Name == name)
    //                {
    //                    return lane;
    //                }
    //            }
    //            return null;
    //        }
    //    }
    //}

    //public class LaneStateList : List<LaneState>
    //{
    //}

    public class MovableObjectList : List<MovableObject>
    {
        public MovableObject this[string name]
        {
            get
            {
                foreach (MovableObject movableObject in this)
                {
                    if (movableObject.Name == name)
                    {
                        return movableObject;
                    }
                }
                return null;
            }
        }
    }

    public class NodeList : List<Node>
    {
        public Node this[string name]
        {
            get
            {
                foreach (Node node in this)
                {
                    if (node.Name == name)
                    {
                        return node;
                    }
                }
                return null;
            }
        }
    }

    public class NodeStateList : List<NodeState>
    {
    }

    public class OperationList : List<Operation>
    {
        public Operation this[string name]
        {
            get
            {
                foreach (Operation operation in this)
                {
                    if (operation.Name == name)
                    {
                        return operation;
                    }
                }
                return null;
            }
        }
    }

    public class OperationStateList : List<OperationState>
    {
    }

     public class StationList : List<Station>
    {
        public Station this[string name]
        {
            get
            {
                foreach (Station processCell in this)
                {
                    if (processCell.Name == name)
                    {
                        return processCell;
                    }
                }
                return null;
            }
        }
    }

    public class StationStateList : List<StationState>
    {
    }

    public class ProcessorList : List<Processor>
    {
        public Processor this[string name]
        {
            get
            {
                foreach (Processor processor in this)
                {
                    if (processor.Name == name)
                    {
                        return processor;
                    }
                }
                return null;
            }
        }
    }

    public class ProcessorStateList : List<ProcessorState>
    {
    }
    
    //IE486fall18
    //public class ProcessorLoadingList : List<ProcessorLoading>
    //{
    //}

    public class QueueList : List<Queue>
    {
        public Queue this[string name]
        {
            get
            {
                foreach (Queue queue in this)
                {
                    if (queue.Name == name)
                    {
                        return queue;
                    }
                }
                return null;
            }
        }
    }

    public class SimulationParameterList : List<SimulationParameter>
    {
    }

    public class StaticObjectList : List<StaticObject>
    {
        public StaticObject this[string name]
        {
            get
            {
                foreach (StaticObject staticObject in this)
                {
                    if (staticObject.Name == name)
                    {
                        return staticObject;
                    }
                }
                return null;
            }
        }
    }

    public class StatisticsStateList : List<StatisticsState>
    {
    }

    public class StringList : List<string>
    {
    }

    public class BinList : List<Bin>
    {
        public Bin this[string name]
        {
            get
            {
                foreach (Bin bin in this)
                {
                    if (bin.Name == name)
                    {
                        return bin;
                    }
                }
                return null;
            }
        }
    }

    public class BinStateList : List<BinState>
    {
    }

    public class ComponentTypeList : List<ComponentType>
    {
        public ComponentType this[string name]
        {
            get
            {
                foreach (ComponentType toolType in this)
                {
                    if (toolType.Name == name)
                    {
                        return toolType;
                    }
                }
                return null;
            }
        }
    }

    public class ToolTypeStateList : List<ToolTypeState>
    {
    }

    public class UnitloadList : List<Unitload>
    {
        public Unitload this[string name]
        {
            get
            {
                foreach (Unitload unitload in this)
                {
                    if (unitload.Name == name)
                    {
                        return unitload;
                    }
                }
                return null;
            }
        }
    }

    public class UnitloadStateList : List<UnitloadState>
    {
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FLOW.NET.Layout;
using FLOW.NET.Operational;

namespace FLOW.NET.Decision.PartSequencingForProcessor
{
    public class FirstComeMustGo : PartSequencingForProcessorAlgorithm
    {
        protected override void Prioritize(UnitloadList unitloadsIn)
        {
        }
        protected override UnitloadList UnitloadListSelection(UnitloadList unitloads)
        {
            UnitloadList unitloadsinQueue = new UnitloadList();
            UnitloadList consideredunitloads = new UnitloadList();

            foreach (Unitload unit in unitloads)
            {
                if (unit.IsCompleted == false)
                {
                    OperationList consideredOperations = new OperationList();
                    foreach (JobRoute currentRoute in unit.Alternates)
                    {
                        Operation operation = currentRoute.Operations[unit.Completed.Count];
                        if (consideredOperations.Contains(operation) == false && consideredunitloads.Contains(unit) == false)
                        {
                            //operation can be done in identical processors
                            if (operation.Processor == null)  
                            {
                                foreach (Processor processor in unit.Station.Processors)
                                {
                                    if ((processor.IsAvailable == true)&&(processor.Operations.Contains(operation)))
                                    {
                                        unitloadsinQueue.Add(unit);
                                        consideredOperations.Add(operation);
                                        consideredunitloads.Add(unit);
                                        break;
                                    }
                                }                               
                            }
                            else //if the processor of the operation is determined from beginning (not identical)
                            {
                                if (operation.Processor.IsAvailable == true)
                                {
                                    unitloadsinQueue.Add(unit);
                                    consideredOperations.Add(operation);
                                    consideredunitloads.Add(unit);
                                    break;
                                }
                            }
                        }
                        if (consideredunitloads.Contains(unit))
                            break; 
                    }                                               
                }
            }
            return unitloadsinQueue;
        }
    }
}

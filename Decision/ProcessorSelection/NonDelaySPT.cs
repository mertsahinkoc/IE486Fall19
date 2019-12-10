using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FLOW.NET.Operational;
using FLOW.NET.Layout;
using FLOW.NET.Collections;


namespace FLOW.NET.Decision.ProcessorSelection
{
   public class NonDelaySPT : ProcessorSelectionAlgorithm
    {
        public override ProcessorOperationCouple Select(Unitload unit)
        {
            ProcessorOperationCouple decision = new ProcessorOperationCouple();
            Operation selectedOperation = null;
            Processor selectedProcessor = null;
            double shortesttime = double.PositiveInfinity;
            OperationList consideredOperations = new OperationList();

            foreach (JobRoute currentRoute in unit.Alternates)
            {
                Operation operation = currentRoute.Operations[unit.Completed.Count];
                if (consideredOperations.Contains(operation) == false)
                {
                    if (operation.Processor == null)
                    {
                        if (unit.Station.BinMagazine.CheckComponent(operation.ComponentUsages))
                        {
                            foreach (Processor processor in unit.Station.Processors)
                            {
                                if ((processor.IsAvailable == true) && (processor.Operations.Contains(operation)))
                                {
                                    if (shortesttime > operation.GetExpectedProcessTime(unit.Station))
                                    {
                                        selectedOperation = operation;
                                        selectedProcessor = processor;
                                        shortesttime = operation.GetExpectedProcessTime(unit.Station);
                                    }
                                    break;
                                }
                            }
                        }                       
                    }
                    else //operation's processor not null but have a unique processor
                    {
                        if (operation.Processor.IsAvailable == true)
                        {
                            if (unit.Station.BinMagazine.CheckComponent(operation.ComponentUsages))
                            {
                                if (shortesttime > operation.GetExpectedProcessTime(unit.Station))
                                {
                                    selectedOperation = operation;
                                    selectedProcessor = operation.Processor;
                                    shortesttime = operation.GetExpectedProcessTime(unit.Station); //fonksiyon boş, nasıl stationa bağlı process time yazılabilir?
                                }
                            }
                        }
                    }
                    consideredOperations.Add(operation);
                }
            }
            decision.Operation = selectedOperation;
            decision.Processor = selectedProcessor;
            return decision;
        }
    }
}

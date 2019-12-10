using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FLOW.NET.Layout;
using FLOW.NET.Operational;

namespace FLOW.NET.Decision.StationController
{
    public class MixedConfiguration : StationControllerAlgorithm
    {
        protected override void ControlStation(FLOW.NET.Layout.Station stationIn)
        {
            UnitloadList uloadtogo = new UnitloadList();

            if (stationIn.GetFreeProcessors().Count > 0)
            {
                if (stationIn.InQueue.Content != null)
                {
                    foreach (Unitload u in stationIn.InQueue.Content)
                    {
                        uloadtogo.Add(u);
                    }
                    uloadtogo = this.Manager.Algorithms.PartSequencingForProcessor.Execute(uloadtogo); //FirstComeMustGo is an example
                    foreach (Unitload unit in uloadtogo)
                    {
                        Processor selectedprocessor = null;
                        if (unit.IsCompleted == false)
                        {
                            selectedprocessor = this.Manager.Algorithms.ProcessorSelection.Execute(unit);
                        }
                        if (selectedprocessor != null)
                        {
                            unit.InTransfer = true;
                            double transferTime = stationIn.TransferTime.GenerateValue();
                            this.Manager.EventCalendar.ScheduleStartProcessEvent(this.Manager.Time, selectedprocessor, unit, transferTime);
                            selectedprocessor.Reserved++;
                            ((Station)selectedprocessor.Parent).BinMagazine.SpendComponent(unit.Operation.ComponentUsages);
                            ((Station)selectedprocessor.Parent).BinMagazine.CheckComponentRequest(unit.Operation.ComponentUsages);                            
                        }
                        if (stationIn.GetFreeProcessors().Count == 0)
                            break;
                    }
                }
            }            
            stationIn.BlockageUpdate(this.Manager.Time);
            stationIn.StarvationUpdate(uloadtogo, this.Manager.Time);
        }
    }
}

using System;
using System.Xml.Serialization;
using FLOW.NET.Random;
using FLOW.NET.Operational;

namespace FLOW.NET.Layout
{
    [XmlType("BufferCell")]
    public class BufferCell : Cell
    {
        public BufferCell()
        {
        }

        public BufferCell(string nameIn, FLOWObject parentIn, RVGenerator transferTimeIn)
            : base(nameIn, parentIn, 0)
        {
            this.TransferTime = transferTimeIn;
        }
    }

    [XmlType("BufferCellState")]
    public class BufferCellState : CellState
    {
        public BufferCellState()
        {
        }

        public void GetState(BufferCell bufferCellIn)
        {
            base.GetState(bufferCellIn);
        }

        public void GetState(BufferCell bufferCellIn, SimulationManager managerIn)
        {
            base.SetState(bufferCellIn);
        }
    }
}
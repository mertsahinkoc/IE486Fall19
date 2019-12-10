using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FLOW.NET.Layout;
using FLOW.NET.Operational;

public class ProcessorOperationCouple 
{
    private Operation operation;
    private Processor processor;

    public ProcessorOperationCouple()
    {
        this.operation = new Operation();
        this.processor = new Processor();
    }

    public Operation Operation
    {
        get { return this.operation; }
        set { this.operation = value; }
    }

    public Processor Processor
    {
        get { return this.processor; }
        set { this.processor = value; }
    }
}

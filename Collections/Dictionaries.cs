using System;
using System.Collections.Generic;
using FLOW.NET.Operational;
using FLOW.NET.Operational.Events;
using FLOW.NET.Layout;

namespace FLOW.NET
{
    public class ComponentTypeUsageDictionary : Dictionary<ComponentType, int> //IE486fall18
    {
    }

    public class OperationListOperationDictionary : SortedList<Operation, OperationList>
    {
    }

    public class InventoryPolicyDictionary : Dictionary<ComponentType, SQParameter> //IE486fall18
    {
    }

    public class DoubleJobRouteDictionary : SortedList<JobRoute, double>
    {
    }

    public class EventListDoubleDictionary : SortedList<double, EventList>
    {
    }

    public class IntegerStringDictionary : SortedList<string, int>
    {
    }

    public class JobTypeDoubleDictionary : SortedList<JobType, double>
    {
    }

    public class JobTypeIntegerDictionary : SortedList<JobType, int>
    {
    }

    public class StatisticsStringDictionary : SortedList<string, Statistics>
    {
    }

	public class UnitloadListStringDictionary : SortedList<string, UnitloadList>
    {
    }

    public class UnitloadListDoubleDictionary : SortedList<double, UnitloadList>
    {
    }
}
using System;
using System.Xml.Serialization;
using FLOW.NET.Random;
using FLOW.NET.Operational;

namespace FLOW.NET.Layout
{
    [XmlType("BinMagazine")]
    public class BinMagazine : Storage, ICloneable
    {
        private RVGenerator loadunloadTime; //state
        private InventoryPolicyDictionary inventoryPolicy;
        private RequestList outstandingRequests;
        private ComponentTypeList componentTypes;
        private StringList inventoryPolicyNameList;
        
        public BinMagazine()
        {
            this.outstandingRequests = new RequestList();
            this.inventoryPolicy = new InventoryPolicyDictionary();
            this.inventoryPolicyNameList = new StringList();
            this.componentTypes = new ComponentTypeList();
        }

        public BinMagazine(string nameIn, FLOWObject parentIn, int capacityIn, RVGenerator loadunloadTimeIn, Node nodeIn) 
            : base(nameIn, parentIn, capacityIn,nodeIn)
        {
            this.loadunloadTime = loadunloadTimeIn;
            this.outstandingRequests = new RequestList();
            this.inventoryPolicy = new InventoryPolicyDictionary();
            this.inventoryPolicyNameList = new StringList();
            this.componentTypes = new ComponentTypeList();
        }


        [XmlIgnore()]
        public RequestList OutstandingRequests
        {
            get { return this.outstandingRequests; }
            set { this.outstandingRequests= value; }
        }

        [XmlIgnore()]
        public ComponentTypeList ComponentTypes
        {
            get { return this.componentTypes; }
            set { this.componentTypes = value; }
        }

        [XmlElement("LoadUnloadTime", typeof(RVGenerator))]
        public RVGenerator LoadUnloadTime
        {
            get { return this.loadunloadTime; }
            set { this.loadunloadTime = value; }
        }

        [XmlIgnore()]
        public InventoryPolicyDictionary InventoryPolicies
        {
            get { return this.inventoryPolicy; }
            set { this.inventoryPolicy = value; }
        }

        [XmlArray("InventoryPolicyNameList")] //IE486fall18
        [XmlArrayItem(typeof(string))]
        public StringList InventoryPolicyNameList
        {
            get { return this.inventoryPolicyNameList; }
            set { this.inventoryPolicyNameList = value; }
        }

        public Bin GetBinWithMinimumCount(ComponentType componentType)
        {
            Bin minimumbin = new Bin();
            int min = Int32.MaxValue;   //  486update 25.12.2018
            foreach (Bin bin in this.Content)
            {
                if (bin.ComponentType == componentType)
                {
                    if (bin.Count < min)
                    {
                        min = bin.Count;
                        minimumbin = bin;
                    }
                }
            }
            return minimumbin;
        }

        //Finds the bin that is capable of doing the operation
        public Bin FindBin(ComponentType componentType)
        {
            Bin temp = new Bin();
            temp.Count = int.MaxValue;
            foreach(Bin bin in this.Content)
            {
                if(bin.ComponentType == componentType)
                { 
                    if(bin.Count < temp.Count)
                    {
                        if (bin.Count != 0)
                        {
                            temp = bin;
                        }
                    }
                } 
            }
            if (temp.Count == int.MaxValue)
            {
                temp = (Bin)this.Content[0];
            }
                return temp;          
        }

       

        public object Clone()
        {
            BinMagazine clone = new BinMagazine(this.Name, this.Parent, this.Capacity, (RVGenerator)this.loadunloadTime.Clone(), this.Node);
            return clone;
        }

        public void ClearStatistics(double timeIn)  //  486update 25.12.2018 degisikliler yapildi
        {
            Statistics requestLeadTime = this.Statistics["RequestLeadTime"];
            requestLeadTime.Clear(timeIn, 0);
            Statistics outstandingRequestCount = this.Statistics["OutstandingRequestCount"];
            outstandingRequestCount.Clear(timeIn, this.OutstandingRequests.Count);
            foreach (ComponentType ct in componentTypes)
            {
                Statistics componentTypeCount = this.Statistics["ComponentTypeCount-" + ct.Name];
                componentTypeCount.Clear(timeIn, GetNumberOfBins(ct)); // IE486f18
                Statistics binTime = this.Statistics["BinTime-" + ct.Name];
                binTime.Clear(timeIn, 0);  // IE486f18
            }
        }

        public void CreateStatistics()
        {
            this.Statistics.Add("RequestLeadTime", new Statistics(0)); //update average lead time
            this.Statistics.Add("OutstandingRequestCount", new Statistics(0)); //toplam outstanding request sayısı * time ---updateweighted
            foreach(ComponentType ct in componentTypes)
            {
                this.Statistics.Add("ComponentTypeCount-" + ct.Name, new Statistics(0)); //updateweighted
                this.Statistics.Add("BinTime-" + ct.Name, new Statistics(0)); //update average -- binlerin ortalama magazine'de durma zamanı
            }
        }

        public void FinalizeStatistics(double timeIn)
        {
            Statistics outstandingCount = this.Statistics["OutstandingRequestCount"];
            outstandingCount.UpdateWeighted(timeIn, this.outstandingRequests.Count);//  486update 25.12.2018 
            foreach (ComponentType ct in this.componentTypes)
            {
                Statistics componenttypeCount = this.Statistics["ComponentTypeCount-" + ct.Name];
                componenttypeCount.UpdateWeighted(timeIn, GetNumberOfBins(ct));    
            }
        }

        //Checks the binmagazine whether its capable or not to do the operation for component type
        public bool CheckComponent(ComponentTypeUsageDictionary ComponentUsages)
        {
            bool flag = true;

            foreach (ComponentType ct in ComponentUsages.Keys)
            {
                int requirement = ComponentUsages[ct];
                int availablecomponent = 0;

                foreach (Bin bin in this.Content)
                {
                    if (bin.ComponentType == ct)
                    {
                        availablecomponent += (bin.Count);
                    }
                }
                if (!(availablecomponent >= requirement))
                {
                    flag = false;
                }
            }
            return flag;
        }

        //Actually spends the component from chosen bin 
        public void SpendComponent(ComponentTypeUsageDictionary ComponentUsages)
        { 
            foreach (ComponentType ct in ComponentUsages.Keys)
            {
                int requirement = ComponentUsages[ct];
                while (requirement > 0)
                {
                    Bin currentbin = this.FindBin(ct);
                    if (currentbin.Count < requirement)
                    {
                        requirement -= currentbin.Count;
                        currentbin.Count = 0;    
                    }
                    else
                    {
                        currentbin.Count -= requirement;
                        requirement = 0;
                    }
                    SimulationManager simulationManager = (SimulationManager)this.Parent.Parent.Parent.Parent;
                    simulationManager.LayoutManager.Layout.WriteInventoryOnHand((Station)this.Parent, ct, simulationManager.Time, this.GetNumberOfBins(ct));
                    Statistics ComponenttypeCount = this.Statistics["ComponentTypeCount-" + currentbin.ComponentType.Name];
                    ComponenttypeCount.UpdateWeighted(((SimulationManager)this.Parent.Parent.Parent.Parent).Time, this.GetNumberOfBins(ct));
                }
            }
        }

        //Checks current inventory position with safety stock level
        public void CheckComponentRequest(ComponentTypeUsageDictionary ComponentUsages)
        {
            foreach (ComponentType ct in ComponentUsages.Keys)
            {
                double inventoryPosition = GetComponentLevel(ct);
                if (inventoryPosition < this.inventoryPolicy[ct].S)
                {
                    Statistics OutstandingRequestCount = this.Statistics["OutstandingRequestCount"];

                    for (int i = 0; i < this.inventoryPolicy[ct].Q; i++)
                    {
                        Request request = new Request(this, ct, ((SimulationManager) this.Parent.Parent.Parent.Parent).Time); 
                        this.outstandingRequests.Add(request);
                        ((SimulationManager)this.Parent.Parent.Parent.Parent).TriggerRequest(request);               
                    }
                    OutstandingRequestCount.UpdateWeighted(((SimulationManager)this.Parent.Parent.Parent.Parent).Time, this.outstandingRequests.Count);
                }
            }
        }

        public double GetNumberOfBins (ComponentType ct)
        {
            double count = 0;
            foreach (Bin b in this.Content)
            {
                if (b.ComponentType == ct)
                {
                    count += b.Count;
                }
            }
            return count / ct.AmountPerBin;
        }

        private double GetComponentLevel(ComponentType ct) 
        {
            double count = GetNumberOfBins(ct);
            foreach (Request r in this.outstandingRequests) //assumption: each request is one bin 
            {
                if (r.ComponentType == ct)
                {
                    count += 1;
                }
            }
            return count;
        }

        public void LoadBin(double timeIn, Bin binIn, bool sharedIn)
        {
            binIn.LoadingTime = timeIn;
            this.Receive(timeIn, binIn);
            binIn.EntryTime = timeIn;
            binIn.ChangeLocation(timeIn, this);
            binIn.Count = binIn.ComponentType.AmountPerBin;
            Statistics ComponenttypeCount = this.Statistics["ComponentTypeCount-" + binIn.ComponentType.Name];
            ComponenttypeCount.UpdateWeighted(((SimulationManager)this.Parent.Parent.Parent.Parent).Time, GetNumberOfBins(binIn.ComponentType));
        }

        //If the bin is received from the transporter
        public void Receive(double timeIn, Bin binIn)
        {
            this.Content.Add(binIn);            
            this.CloseRequest(binIn.ComponentType, timeIn);
        }

        //If the request has come to bin magazine, deletes it from outstanding requests
        private void CloseRequest(ComponentType componentTypeIn, double timeIn)
        {
            foreach(Request request in this.outstandingRequests)
            {
                if (request.ComponentType == componentTypeIn)
                {
                    double now = ((SimulationManager)this.Parent.Parent.Parent.Parent).Time;
                    this.outstandingRequests.Remove(request);
                    Statistics OutstandingRequestCount = this.Statistics["OutstandingRequestCount"];
                    OutstandingRequestCount.UpdateWeighted(now, this.outstandingRequests.Count);
                    Statistics RequestLeadTime = this.Statistics["RequestLeadTime"];
                    RequestLeadTime.UpdateAverage(now, (now - request.Time));
                    return;
                }
            }
        }

        //If the bin is given to transporter
        public void Release(double timeIn, Bin binIn)
        {
            this.Content.Remove(binIn);
            Statistics binTime = this.Statistics["BinTime-" + binIn.ComponentType.Name];
            binTime.UpdateAverage(timeIn,(timeIn-binIn.EntryTime));
            binIn.Location = null;
        }

        public void Reset(double timeIn)
        {
            foreach (Bin tool in this.Content)
            {
                tool.InTransfer = false;
            }
        }

        public void Unload(double timeIn)
        {
            this.Content.Clear();
        }
    }

    [XmlType("MagazineState")]
    public class BinMagazineState : StaticObjectState
    {
        private RVGeneratorState loadunloadTime;

        public BinMagazineState()
        {
        }

        [XmlElement("LoadUnloadTime")]
        public RVGeneratorState LoadUnloadTime
        {
            get { return this.loadunloadTime; }
            set { this.loadunloadTime = value; }
        }


        public void GetState(BinMagazine magazineIn)
        {
            base.GetState(magazineIn);
            this.loadunloadTime = magazineIn.LoadUnloadTime.GetRVGeneratorState();
        }

        public void SetState(BinMagazine binMagazineIn, SimulationManager managerIn)
        {
            Layout layout = managerIn.LayoutManager.Layout;
            binMagazineIn.Content.Clear();
            foreach (string toolName in this.Content)
            {
                Bin tool = layout.Bins[toolName];
                binMagazineIn.Receive(tool.EntryTime, tool);
            }
            base.SetState(binMagazineIn);
        }
    }

   
	
}			


using System;
using FLOW.NET.Layout;
using FLOW.NET.Random;
using FLOW.NET.Operational;

namespace FLOW.NET.Decision
{
    public abstract class DecisionAlgorithm
    {
        private SimulationManager manager;

        private RngStream stream;

		//Fixed from Wrapper for now
		protected double hCoeffForCovert = 1;

		protected double hForAPT = 1;

        public DecisionAlgorithm()
        {
        }

        public SimulationManager Manager
        {
            get { return this.manager; }
            set { this.manager = value; }
        }

        public RngStream Stream
        {
            get { return this.stream; }
            set { this.stream = value; }
        }

		public double HCoeffForCovert
		{
			get { return this.hCoeffForCovert; }
			set { this.hCoeffForCovert = value; }
		}

		public double HForAPT
		{
			get { return this.hForAPT; }
			set { this.hForAPT = value; }
		}

		public DoubleJobRouteDictionary CalculateRouteStatistics(Unitload unitloadIn)
		{
			DoubleJobRouteDictionary routeStatistics = new DoubleJobRouteDictionary();
			double totalRouteUsage = 0;
			foreach (JobRoute currentRoute in unitloadIn.Alternates)
			{
				totalRouteUsage += currentRoute.Usage.Count;
			}

			foreach (JobRoute currentRoute in unitloadIn.Alternates)
			{
				double routeUsage = totalRouteUsage == 0 ? 1 : currentRoute.Usage.Count / totalRouteUsage;
				routeStatistics.Add(currentRoute, routeUsage);
			}

			return routeStatistics;
		}

        public BufferCell FindNearestCentralBuffer(StaticObject locationIn)
        {
            BufferCell selectedCell = null;
            double selectedDistance = Double.PositiveInfinity;
            BufferCellList bufferCells = this.Manager.LayoutManager.Layout.BufferCells;
            foreach (BufferCell bufferCell in bufferCells)
            {
                double distance = 0;
                if (distance < selectedDistance)
                {
                    selectedCell = bufferCell;
                    selectedDistance = distance;
                }
            }
            return selectedCell;
        }

        public virtual void Initialize(SimulationManager managerIn)
        {
            this.manager = managerIn;
            this.stream = new RngStream();
        }
    }
}
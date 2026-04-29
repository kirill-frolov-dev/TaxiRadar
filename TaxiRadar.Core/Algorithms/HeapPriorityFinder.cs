using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiRad.Core.Models;

namespace TaxiRad.Core.Algorithms
{
    public class HeapPriorityFinder : IDriverFinder
    {
        public string Name 
        { 
            get { return "HeapPriority"; } 
        }

        public IReadOnlyList<Driver> FindNearest(Order order, IReadOnlyList<Driver> drivers, int count, int N, int M)
        {
            if (order == null || drivers == null || count <= 0 || drivers.Count == 0)
                return Array.Empty<Driver>();

            PriorityQueue<Driver, int> priorityQueue = new();
            foreach (Driver driver in drivers)
            {
                priorityQueue.Enqueue(driver, -(Math.Abs(driver.X - order.X) + Math.Abs(driver.Y - order.Y)));
                if (priorityQueue.Count > count)
                {
                    priorityQueue.Dequeue();
                }
            }
            var candidates = new List<Driver>(count);
            while (priorityQueue.TryDequeue(out var driver, out _))
            {
                candidates.Add(driver);
            }
            candidates.Reverse();
            return candidates;
        }
    }
}

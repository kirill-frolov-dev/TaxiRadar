using System;
using System.Collections.Generic;
using System.Linq;
using TaxiRad.Core.Models;

namespace TaxiRad.Core.Algorithms
{
    public class TopKScanFinder : IDriverFinder
    {
        public string Name => "TopKScan";

        public IReadOnlyList<Driver> FindNearest(Order order, IReadOnlyList<Driver> drivers, int count, int N, int M)
        {
            if (order == null || drivers == null || count <= 0 || drivers.Count == 0)
                return Array.Empty<Driver>();

            var topK = new List<(int dist, Driver driver)>(count);
            int maxDist = int.MaxValue;

            foreach (var driver in drivers)
            {
                int distance = Math.Abs(driver.X - order.X) + Math.Abs(driver.Y - order.Y);

                if (topK.Count >= count && distance >= maxDist)
                    continue;

                int insertPos = topK.Count;
                while (insertPos > 0 && topK[insertPos - 1].dist > distance)
                {
                    insertPos--;
                }

                topK.Insert(insertPos, (distance, driver));

                if (topK.Count > count)
                {
                    topK.RemoveAt(topK.Count - 1);
                }
                maxDist = topK[topK.Count - 1].dist;
            }

            return topK.Select(d => d.driver).ToArray();
        }
    }
}
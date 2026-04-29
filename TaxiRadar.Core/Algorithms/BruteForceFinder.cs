using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiRad.Core.Models;

namespace TaxiRad.Core.Algorithms
{
    public class BruteForceFinder : IDriverFinder
    {
        public string Name
        {
            get { return "BruteForce"; }
        }
        public IReadOnlyList<Driver> FindNearest(Order order, IReadOnlyList<Driver> drivers, int count, int N, int M)
        {
            if (order == null || drivers == null || count <= 0 || drivers.Count == 0)
                return Array.Empty<Driver>();

            return drivers
            .OrderBy(d => Math.Abs(d.X - order.X) + Math.Abs(d.Y - order.Y))
            .Take(count)
            .ToList();

        }
    }
}

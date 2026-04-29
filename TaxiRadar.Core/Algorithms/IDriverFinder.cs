using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiRad.Core.Models;

namespace TaxiRad.Core.Algorithms
{
    public interface  IDriverFinder
    {
        string Name { get; }
        IReadOnlyList<Driver> FindNearest(Order order, IReadOnlyList<Driver> drivers, int count, int N, int M);
    }
}

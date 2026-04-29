using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using TaxiRad.Core.Algorithms;
using TaxiRad.Core.Models;

namespace TaxiRad.Benchmarks
{
    [MemoryDiagnoser]
    [RankColumn]
    [Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
    public class DriverFinderBenchmarks
    {
        private const int N = 10_000;
        private const int M = 10_000;
        private const int Count = 5;

        [Params(10_000, 100_000, 1_000_000)]
        public int DriverCount { get; set; }

        private Order _order;
        private List<Driver> _drivers;
        private IDriverFinder[] _finders;

        [GlobalSetup]
        public void Setup()
        {
            var rng = new Random(42);

            _drivers = new List<Driver>(DriverCount);
            for (int i = 0; i < DriverCount; i++)
            {
                _drivers.Add(new Driver(i + 1, rng.Next(0, N), rng.Next(0, M)));
            }

            _order = new Order(rng.Next(0, N), rng.Next(0, M));

            _finders = new IDriverFinder[]
            {
                new BruteForceFinder(),
                new HeapPriorityFinder(),
                new SpatialHashingFinder(),
                new TopKScanFinder()
            };
        }

        [Benchmark]
        public IReadOnlyList<Driver> BruteForce() =>
            _finders[0].FindNearest(_order, _drivers, Count, N, M);

        [Benchmark]
        public IReadOnlyList<Driver> HeapPriority() =>
            _finders[1].FindNearest(_order, _drivers, Count, N, M);

        [Benchmark]
        public IReadOnlyList<Driver> SpatialHashing() =>
            _finders[2].FindNearest(_order, _drivers, Count, N, M);

        [Benchmark]
        public IReadOnlyList<Driver> TopKScan() =>
            _finders[3].FindNearest(_order, _drivers, Count, N, M);
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<DriverFinderBenchmarks>(args: args);
        }
    }
}
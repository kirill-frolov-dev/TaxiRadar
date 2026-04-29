using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TaxiRad.Core.Algorithms;
using TaxiRad.Core.Models;

namespace TaxiRad.Tests
{
    [TestFixture]
    public class DriverFinderTests
    {
        private IDriverFinder[] _finders;
        private const int N = 100, M = 100, Count = 5;

        [SetUp]
        public void SetUp()
        {
            _finders = new IDriverFinder[]
            {
                new BruteForceFinder(),
                new HeapPriorityFinder(),
                new SpatialHashingFinder(),
                new TopKScanFinder()
            };
        }

        [Test]
        public void FindNearest_BasicCase_ReturnsExactlyCountInCorrectOrder()
        {
            var order = new Order(50, 50);
            var drivers = GenerateDriversAround(order, 20);

            foreach (var finder in _finders)
            {
                var result = finder.FindNearest(order, drivers, Count, N, M);
                Assert.AreEqual(Count, result.Count, $"{finder.Name}: неверное количество результатов");
                Assert.IsTrue(IsSortedByDistance(result, order), $"{finder.Name}: результаты не отсортированы");
            }
        }

        [Test]
        public void FindNearest_EmptyList_ReturnsEmpty()
        {
            var order = new Order(10, 10);
            var emptyDrivers = Array.Empty<Driver>();

            foreach (var finder in _finders)
            {
                var result = finder.FindNearest(order, emptyDrivers, Count, N, M);
                Assert.AreEqual(0, result.Count, $"{finder.Name} должен вернуть пустой список");
            }
        }

        [Test]
        public void FindNearest_ZeroOrNegativeCount_ReturnsEmpty()
        {
            var order = new Order(10, 10);
            var drivers = GenerateDriversAround(order, 5);

            foreach (var finder in _finders)
            {
                Assert.AreEqual(0, finder.FindNearest(order, drivers, 0, N, M).Count, $"{finder.Name}: count=0");
                Assert.AreEqual(0, finder.FindNearest(order, drivers, -1, N, M).Count, $"{finder.Name}: count<0");
            }
        }

        [Test]
        public void FindNearest_CountGreaterThanDrivers_ReturnsAllSorted()
        {
            var order = new Order(25, 25);
            var drivers = GenerateDriversAround(order, 3);

            foreach (var finder in _finders)
            {
                var result = finder.FindNearest(order, drivers, 10, N, M);
                Assert.AreEqual(3, result.Count, $"{finder.Name} должен вернуть всех доступных");
                Assert.IsTrue(IsSortedByDistance(result, order));
            }
        }

        [Test]
        public void FindNearest_AllAlgorithms_ReturnIdenticalResults()
        {
            var order = new Order(45, 72);
            var drivers = GenerateDriversAround(order, 50);

            var bruteResult = _finders[0].FindNearest(order, drivers, Count, N, M).ToList();

            for (int i = 1; i < _finders.Length; i++)
            {
                var otherResult = _finders[i].FindNearest(order, drivers, Count, N, M).ToList();
                Assert.AreEqual(bruteResult.Count, otherResult.Count, $"Разное кол-во у {_finders[i].Name}");

                for (int j = 0; j < bruteResult.Count; j++)
                {
                    Assert.AreEqual(bruteResult[j].Id, otherResult[j].Id,
                        $"{_finders[i].Name} вернул другого водителя на позиции {j}");
                }
            }
        }

        private List<Driver> GenerateDriversAround(Order order, int amount)
        {
            var drivers = new List<Driver>();
            var rng = new Random(42);
            for (int i = 0; i < amount; i++)
            {
                int x = Math.Max(0, Math.Min(order.X + rng.Next(-40, 41), N - 1));
                int y = Math.Max(0, Math.Min(order.Y + rng.Next(-40, 41), M - 1));
                drivers.Add(new Driver(i + 1, x, y));
            }
            return drivers;
        }

        private bool IsSortedByDistance(IReadOnlyList<Driver> list, Order order)
        {
            for (int i = 1; i < list.Count; i++)
            {
                int distPrev = Math.Abs(list[i - 1].X - order.X) + Math.Abs(list[i - 1].Y - order.Y);
                int distCurr = Math.Abs(list[i].X - order.X) + Math.Abs(list[i].Y - order.Y);
                if (distCurr < distPrev) return false;
            }
            return true;
        }
    }
}
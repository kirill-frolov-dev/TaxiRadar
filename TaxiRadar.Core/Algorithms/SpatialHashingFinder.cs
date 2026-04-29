using System;
using System.Collections.Generic;
using System.Linq;
using TaxiRad.Core.Models;

namespace TaxiRad.Core.Algorithms
{
    public class SpatialHashingFinder : IDriverFinder
    {
        public string Name => "SpatialHashing";
        private const int cellSize = 100;

        private Dictionary<(int x, int y), List<Driver>> _cachedGrid;
        private IReadOnlyList<Driver> _cachedDrivers;
        private int _cachedN, _cachedM;

        public IReadOnlyList<Driver> FindNearest(Order order, IReadOnlyList<Driver> drivers, int count, int N, int M)
        {
            if (order == null || drivers == null || count <= 0 || drivers.Count == 0)
                return Array.Empty<Driver>();

            if (!ReferenceEquals(_cachedDrivers, drivers) || _cachedN != N || _cachedM != M)
            {
                BuildGrid(drivers, N, M);
                _cachedDrivers = drivers;
                _cachedN = N;
                _cachedM = M;
            }

            return FindNearestInGrid(order, count, N, M);
        }

        private void BuildGrid(IReadOnlyList<Driver> drivers, int N, int M)
        {
            _cachedGrid = new Dictionary<(int, int), List<Driver>>(drivers.Count / 10 + 1);

            foreach (var driver in drivers)
            {
                var bucket = (driver.X / cellSize, driver.Y / cellSize);

                if (!_cachedGrid.TryGetValue(bucket, out var list))
                {
                    list = new List<Driver>(4);
                    _cachedGrid[bucket] = list;
                }
                list.Add(driver);
            }
        }

        private IReadOnlyList<Driver> FindNearestInGrid(Order order, int count, int N, int M)
        {
            int startX = order.X / cellSize;
            int startY = order.Y / cellSize;

            int maxCellX = (N - 1) / cellSize;
            int maxCellY = (M - 1) / cellSize;

            int maxRadius = Math.Max(
                Math.Max(startX, maxCellX - startX),
                Math.Max(startY, maxCellY - startY)
            );

            var candidates = new List<Driver>(count * 4);
            int radiusOfSearch = 0;

            while (candidates.Count < count && radiusOfSearch <= maxRadius)
            {
                for (int x = startX - radiusOfSearch; x <= startX + radiusOfSearch; x++)
                {
                    for (int y = startY - radiusOfSearch; y <= startY + radiusOfSearch; y++)
                    {
                        // 🔥 Проверяем только периметр кольца
                        if (Math.Abs(x - startX) == radiusOfSearch || Math.Abs(y - startY) == radiusOfSearch)
                        {
                            if (x >= 0 && x <= maxCellX && y >= 0 && y <= maxCellY)
                            {
                                if (_cachedGrid.TryGetValue((x, y), out var foundDrivers))
                                {
                                    candidates.AddRange(foundDrivers);
                                }
                            }
                        }
                    }
                }
                radiusOfSearch++;

                // 🔥 Простой ранний выход
                if (candidates.Count >= count * 3 && radiusOfSearch > 5)
                    break;
            }
            var orderX = order.X;
            var orderY = order.Y;

            return candidates
                .Select(d => new
                {
                    Driver = d,
                    Dist = Math.Abs(d.X - orderX) + Math.Abs(d.Y - orderY)
                })
                .OrderBy(x => x.Dist)
                .Take(count)
                .Select(x => x.Driver)
                .ToList();
        }
    }
}
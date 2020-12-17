using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

var input = File.ReadAllText("input.txt");
var rows = input.Split(Environment.NewLine).ToArray();

var run = new Action<bool, string>((hyperDimension, label) =>
{
    var stopwatch = Stopwatch.StartNew();
    var space = new Space(hyperDimension);
    space.Initialize(rows);

    foreach (var _ in Enumerable.Range(1, 6)) space.RunCycle();

    var countAfter = space.CountActive();
    Console.WriteLine($"[{label}] Active {countAfter}");

    stopwatch.Stop();
    Console.WriteLine($"[{label}] Took time: {stopwatch.Elapsed}");
});

var taskOne = new Action(() => run(false, "One"));
var taskTwo = new Action(() => run(true, "Two"));

taskOne();
taskTwo();

internal record Cube(int X, int Y, int Z, int W);

internal class Space
{
    private readonly bool _hyperDimension;
    private readonly HashSet<Cube> _activeCoordinates = new();

    public Space(bool hyperDimension)
    {
        _hyperDimension = hyperDimension;
    }

    public void Initialize(string[] rows)
    {

        foreach (var coordinate in ActiveCoordinates())
        {
            _activeCoordinates.Add(coordinate);
        }

        IEnumerable<Cube> ActiveCoordinates()
        {
            for (var x = 0; x < rows.Length; x++)
            {
                var row = rows[x].ToCharArray();
                for (var y = 0; y < rows.Length; y++)
                {
                    if (row[y] != '#')
                    {
                        continue;
                    }

                    yield return new(x, y, 0, 0);
                }
            }
        }
    }

    public int CountActive()
    {
        return _activeCoordinates.Count;
    }


    public void RunCycle()
    {
        // Important to enumerate before
        var cubesToFlip = CubesToFlip().ToList();

        foreach (var cube in cubesToFlip)
        {
            if (IsCubeActive(cube))
            {
                _activeCoordinates.Remove(cube);
            }
            else
            {
                _activeCoordinates.Add(cube);
            }
        }

        IEnumerable<Cube> CubesToFlip()
        {
            var cubesToCheck = _activeCoordinates
                .SelectMany(Neighbors)
                .Concat(_activeCoordinates)
                .Distinct();

            foreach (var cube in cubesToCheck)
            {
                if (ShouldFlip(cube))
                {
                    yield return cube;
                }
            }
        }

        bool ShouldFlip(Cube cube)
        {
            var isActive = IsCubeActive(cube);
            var activeNeighborsCount = 0;
            foreach (var neighbor in Neighbors(cube))
            {
                if (IsCubeActive(neighbor))
                {
                    activeNeighborsCount += 1;
                }

                if (activeNeighborsCount > 3)
                {
                    return isActive;
                }
            }

            if (isActive)
            {
                return activeNeighborsCount != 2 && activeNeighborsCount != 3;
            }

            return activeNeighborsCount == 3;
        }
    }

    private IEnumerable<Cube> Neighbors(Cube cube)
    {
        for (var dx = -1; dx <= 1; dx++)
        for (var dy = -1; dy <= 1; dy++)
        for (var dz = -1; dz <= 1; dz++)
            foreach (var neighbor in NeighborsInHyperDimension(dx, dy, dz))
                yield return neighbor;

        IEnumerable<Cube> NeighborsInHyperDimension(int dx, int dy, int dz)
        {
            if (!_hyperDimension)
            {
                if (dx == 0 && dy == 0 && dz == 0) yield break;

                yield return new(cube.X + dx, cube.Y + dy, cube.Z + dz, cube.W);
                yield break;
            }

            for (var dw = -1; dw <= 1; dw++)
            {
                if (dx == 0 && dy == 0 && dz == 0 && dw == 0) continue;

                yield return new(cube.X + dx, cube.Y + dy, cube.Z + dz, cube.W + dw);
            }
        }
    }

    private bool IsCubeActive(Cube x)
    {
        return _activeCoordinates.Contains(x);
    }
}
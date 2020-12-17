using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SpaceMap =
    System.Collections.Generic.Dictionary<int, System.Collections.Generic.Dictionary<int,
        System.Collections.Generic.Dictionary<int, System.Collections.Generic.Dictionary<int, Cube>>>>;


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

internal class Cube
{
    public readonly int W;
    public readonly int X;
    public readonly int Y;
    public readonly int Z;

    public Cube(int x, int y, int z, int w, bool active)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
        Active = active;
    }

    public bool Active { get; private set; }

    public bool ShouldFlip(Space space)
    {
        var activeNeighbours = space.Neighbors(this).Count(s => s.Active);

        if (Active) return activeNeighbours != 2 && activeNeighbours != 3;

        return activeNeighbours == 3;
    }

    public void Flip()
    {
        Active = !Active;
    }
}

internal class Space
{
    private readonly bool _hyperDimension;
    private readonly SpaceMap _space = new();

    public Space(bool hyperDimension)
    {
        _hyperDimension = hyperDimension;
    }

    public void Initialize(string[] rows)
    {
        for (var x = 0; x < rows.Length; x++)
        {
            _space[x] = new();

            var row = rows[x].ToCharArray();

            for (var y = 0; y < rows.Length; y++)
                _space[x][y] = new()
                {
                    {
                        0, new()
                        {
                            {0, new Cube(x, y, 0, 0, row[y] == '#')}
                        }
                    }
                };
        }
    }

    public int CountActive()
    {
        return _space
            .SelectMany(x => x.Value.SelectMany(y => y.Value.SelectMany(z => z.Value.Select(w => w.Value))))
            .Count(c => c.Active);
    }


    public void RunCycle()
    {
        var maxX = _space.Keys.Max();
        var minX = _space.Keys.Min();
        var maxY = _space[minX].Keys.Max();
        var minY = _space[minX].Keys.Min();
        var maxZ = _space[minX][minY].Keys.Max();
        var minZ = _space[minX][minY].Keys.Min();
        var maxW = _space[minX][minY][minZ].Keys.Max();
        var minW = _space[minX][minY][minZ].Keys.Min();

        // Important to enumerate before
        var cubesToFlip = CubesToFlip().ToList();

        // Console.WriteLine("Before flips:");
        // Print();
        // Console.WriteLine();
        foreach (var cube in cubesToFlip) cube.Flip();
        // Console.WriteLine("After flips");
        // Print();
        // Console.WriteLine();

        IEnumerable<Cube> CubesToFlip()
        {
            for (var x = minX - 1; x <= maxX + 1; x++)
            for (var y = minY - 1; y <= maxY + 1; y++)
            for (var z = minZ - 1; z <= maxZ + 1; z++)
                foreach (var cube in CubesInHyperDimension(x, y, z))
                    if (cube.ShouldFlip(this))
                        yield return cube;
        }

        IEnumerable<Cube> CubesInHyperDimension(int x, int y, int z)
        {
            if (!_hyperDimension)
            {
                yield return IsCubeActive(x, y, z, 0);
                yield break;
            }

            for (var w = minW - 1; w <= maxW + 1; w++) yield return IsCubeActive(x, y, z, w);
        }
    }

    public IEnumerable<Cube> Neighbors(Cube cube)
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

                yield return IsCubeActive(cube.X + dx, cube.Y + dy, cube.Z + dz, cube.W);
                yield break;
            }

            for (var dw = -1; dw <= 1; dw++)
            {
                if (dx == 0 && dy == 0 && dz == 0 && dw == 0) continue;

                yield return IsCubeActive(cube.X + dx, cube.Y + dy, cube.Z + dz, cube.W + dw);
            }
        }
    }

    private Cube IsCubeActive(int x, int y, int z, int w)
    {
        if (!_space.ContainsKey(x)) _space[x] = new Dictionary<int, Dictionary<int, Dictionary<int, Cube>>>();

        if (!_space[x].ContainsKey(y)) _space[x][y] = new Dictionary<int, Dictionary<int, Cube>>();

        if (!_space[x][y].ContainsKey(z)) _space[x][y][z] = new Dictionary<int, Cube>();

        if (!_space[x][y][z].ContainsKey(w)) _space[x][y][z][w] = new Cube(x, y, z, w, false);

        return _space[x][y][z][w];
    }
}
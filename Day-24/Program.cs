using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

var input = File.ReadAllLines("input.txt");

void One()
{
    var blacks = Run();

    Console.WriteLine($"[One]: Count: {blacks.Count}");
}

void Two()
{
    var blacks = Run();

    for (var i = 1; i <= 100; i++)
    {
        var tilesToCheck = blacks.Concat(blacks.SelectMany(b => b.Neighbors())).Distinct();

        var tilesToFlip = tilesToCheck.Where(t => t.ShouldFlip(blacks)).ToList();

        foreach (var tile in tilesToFlip) tile.Flip(blacks);

        Console.WriteLine($"[Two]: Day {i}, black tiles: {blacks.Count}");
    }
}

ISet<Coordinate> Run()
{
    var blacks = new HashSet<Coordinate>();

    foreach (var row in input)
    {
        var coordinate = new Coordinate(0, 0);
        var directions = GetDirections(row);

        coordinate = directions.Aggregate(coordinate, (c, direction) => c.GetNextCoordinate(direction));

        coordinate.Flip(blacks);
    }

    return blacks;
}

One();
Two();

static string[] GetDirections(string input)
{
    if (input.Length == 0) return Array.Empty<string>();

    var direction = GetNextDirection(input);
    var restOfString = input.Substring(direction.Length);

    return new[] {direction}.Concat(GetDirections(restOfString)).ToArray();
}

static string GetNextDirection(string input)
{
    return input switch
    {
        var i when i.StartsWith("e") => "e",
        var i when i.StartsWith("w") => "w",
        var i when i.StartsWith("se") => "se",
        var i when i.StartsWith("sw") => "sw",
        var i when i.StartsWith("ne") => "ne",
        var i when i.StartsWith("nw") => "nw",
        _ => throw new ArgumentOutOfRangeException(nameof(input), input, null)
    };
}

internal record Coordinate(int X, int Y)
{
    public Coordinate GetNextCoordinate(string direction)
    {
        return direction switch
        {
            "ne" => this with { X = X + 1, Y = Y + 1},
            "e" => this with { X = X + 1},
            "se" => this with { Y = Y - 1},
            "sw" => this with { X = X - 1, Y = Y - 1},
            "w" => this with { X = X - 1},
            "nw" => this with { Y = Y + 1},
            _ => throw new ArgumentException(null, nameof(direction))
        };
    }

    public bool ShouldFlip(ISet<Coordinate> blacks)
    {
        var countBlackNeighbors = Neighbors().Count(blacks.Contains);

        // Any black tile with zero or more than 2 black tiles immediately adjacent to it is flipped to white.
        if (blacks.Contains(this)) return countBlackNeighbors == 0 || countBlackNeighbors > 2;

        // Any white tile with exactly 2 black tiles immediately adjacent to it is flipped to black.
        return countBlackNeighbors == 2;
    }

    public void Flip(ISet<Coordinate> blacks)
    {
        if (blacks.Contains(this))
            blacks.Remove(this);
        else
            blacks.Add(this);
    }

    public IEnumerable<Coordinate> Neighbors()
    {
        yield return this with { X = X + 1, Y = Y + 1};
        yield return this with { X = X + 1};
        yield return this with { Y = Y - 1};
        yield return this with { X = X - 1, Y = Y - 1};
        yield return this with { X = X - 1};
        yield return this with { Y = Y + 1};
    }
}
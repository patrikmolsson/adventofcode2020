#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

var input = File.ReadAllText("input.txt");

var tileStrings = input.Split($"{Environment.NewLine}{Environment.NewLine}").ToArray();

var fullTiles = new Dictionary<int, string[]>();

var tiles = tileStrings.Select(tileString =>
{
    var rows = tileString.Split(Environment.NewLine);
    var id = int.Parse(Regex.Match(rows[0], @"\d+").ToString());

    fullTiles[id] = rows[1..];

    return new Tile(id, rows[1..]);
}).ToArray();

// var tests = new Action(() =>
// {
//     // var t1489 = tiles.Single(t => t.Id == 1489).FlipVertical();
//     // var t2473 = tiles.Single(t => t.Id == 2473).Rotate(3).FlipHorizontal();
//     // var t1171 = tiles.Single(t => t.Id == 1171).FlipHorizontal();
//     var t1489 = tiles.Single(t => t.Id == 1489).Rotate(2).FlipHorizontal();
//     var t2473 = tiles.Single(t => t.Id == 2473).Rotate(1).FlipHorizontal();
//     var t1171 = tiles.Single(t => t.Id == 1171);
//     var t2311 = tiles.Single(t => t.Id == 2311).Rotate(2).FlipHorizontal();
//     var t3079 = tiles.Single(t => t.Id == 3079);
//     var t3079RotatedCorrectly = tiles.Single(t => t.Id == 3079);
//
//     var t1171matchboth = t1171.AllCombinations().Where(c =>
//         c.MatchesDirection(t1489, Direction.Left) && c.MatchesDirection(t2473, Direction.Above)
//     ).ToList();
//     
//     var t3079MatchSingle = t3079.AllCombinations().Where(c => c.MatchesDirection(t2311, Direction.Left)).ToList();
//     var t3079MatchBoth = t3079.AllCombinations().Where(c => c.MatchesDirection(t2311, Direction.Left) && c.MatchesDirection(t2473, Direction.Below)).ToList();
// });
//
// tests();

var one = new Action(() =>
{
    var availableTiles = tiles[1..].ToList();

    var grid = new Dictionary<(int x, int y), Tile?>
    {
        {(0, 0), tiles[0].Rotate(1)} // Trial-and-error
        // {(0, 0), tiles[0].Rotate(3)}
    };

    var queue = new Queue<(int x, int y)>();
    
    foreach (var coord in AdjacentCoordinates((0, 0)))
    {
        queue.Enqueue(coord);
    }

    while (availableTiles.Any() && queue.TryDequeue(out var coordinate))
    {
        if (grid.ContainsKey(coordinate))
        {
            continue;
        }
        
        var adjacentTilesInGrid = TilesAdjacentToCoordinate(coordinate).ToList();

        if (!adjacentTilesInGrid.Any())
        {
            grid[coordinate] = null;
            continue;
        }

        var matches = availableTiles
            .SelectMany(t => t.AllCombinations())
            .Where(t =>
                adjacentTilesInGrid.All(adjacent => t.MatchesDirection(adjacent.tile, adjacent.direction)))
            .ToList();

        if (matches.Count > 1)
        {
            Console.WriteLine(matches);
            continue;
        }

        var match = matches.SingleOrDefault();
        grid[coordinate] = match;

        if (match is not null)
        {
            foreach (var coord in AdjacentCoordinates(coordinate))
            {
                queue.Enqueue(coord);
            }
            
            availableTiles = availableTiles.Where(t => t.Id != match.Id).ToList();
        }
    }

    IEnumerable<(Direction direction, Tile tile)> TilesAdjacentToCoordinate((int x, int y) coord)
    {
        return new (int x, int y, Direction direction)[]
        {
            (coord.x - 1, coord.y, Direction.Left),
            (coord.x + 1, coord.y, Direction.Right),
            (coord.x, coord.y - 1, Direction.Below),
            (coord.x, coord.y + 1, Direction.Above),
        }.Select(c => (c.direction, grid.GetValueOrDefault((c.x, c.y)))).Where(t => t.Item2 != null) as IEnumerable<(Direction, Tile)>;
    }

    static IEnumerable<(int x, int y)> AdjacentCoordinates((int x, int y) coord)
    {

        return new (int x, int y)[]
        {
            (coord.x - 1, coord.y),
            (coord.x + 1, coord.y),
            (coord.x, coord.y - 1),
            (coord.x, coord.y + 1)
        };
    }
            

    static IEnumerable<(int x, int y)> GetCoordinates()
    {
        // (di, dj) is a vector - direction in which we move right now
        var di = 1;
        var dj = 0;
        var segmentLength = 1;

        // current position (i, j) and how much of current segment we passed
        var i = 0;
        var j = 0;
        var segmentPassed = 0;
        for (var k = 0; k < 144 * 4; ++k) {
            // make a step, add 'direction' vector (di, dj) to current position (i, j)
            i += di;
            j += dj;
            ++segmentPassed;

            yield return (i, j);

            if (segmentPassed != segmentLength) continue;
            
            // done with current segment
            segmentPassed = 0;

            // 'rotate' directions
            var buffer = di;
            di = -dj;
            dj = buffer;

            // increase segment length if necessary
            if (dj == 0) {
                ++segmentLength;
            }
        }
    }

    var placedTiles = grid.Where(d => d.Value != null).ToList();
    var maxXCoordinate = placedTiles.Max(s => s.Key.x);
    var maxYCoordinate = placedTiles.Max(s => s.Key.y);
    var minXCoordinate = placedTiles.Min(s => s.Key.x);
    var minYCoordinate = placedTiles.Min(s => s.Key.y);

    var cornerTiles = new[]
    {
        grid[(minXCoordinate, maxYCoordinate)],
        grid[(maxXCoordinate, maxYCoordinate)],
        grid[(maxXCoordinate, minYCoordinate)],
        grid[(minXCoordinate, minYCoordinate)],
    };

    var product = cornerTiles.Aggregate(1L, (l, tile) => l * tile.Id);
    
    Console.WriteLine(product);
    
    // Task two
    var fullGrid = new Dictionary<(int x, int y), char>();
    
    for (var xPos = minXCoordinate; xPos <= maxXCoordinate; xPos++)
    for (var yPos = minYCoordinate; yPos <= maxYCoordinate; yPos++)
    {
        var tile = grid[(xPos, yPos)];
        
        for (var yOff = 1; yOff < tile.Rows.Length - 1; yOff++)
        for (var xOff = 1; xOff < tile.Rows[yOff].Length - 1; xOff++)
        {
            var x = xPos * (tile.Rows[yOff].Length - 2) + xOff - 1;
            var y = yPos * (tile.Rows.Length - 2) - (yOff - 1);

            fullGrid.Add((x, y), tile.Rows[yOff][xOff]);
        }
    }
    

    var coordinatesWithSeaMonster = new HashSet<(int x, int y)>();

    foreach (var coord in fullGrid.Keys)
    {
        if (HasMonsterStartingAt(coord))
        {
            Console.WriteLine($"hello from {coord}");
        }
    }
    
    // PrintGrid();
    // Console.WriteLine();

    foreach (var coord in coordinatesWithSeaMonster)
    {
        fullGrid[coord] = 'O';
    }
    
    // PrintGrid();

    var roughness = fullGrid.Count(s => s.Value == '#');
    
    Console.WriteLine($"Roughness: {roughness}");

    bool HasMonsterStartingAt((int x, int y) coordinate)
    {
        // Monster:
        //                  # 
        //#    ##    ##    ###
        // #  #  #  #  #  #   

        var coords = GenerateCoordinates();
        
        var hasMonster = GenerateCoordinates().All(c => fullGrid.TryGetValue(c, out var character) && !coordinatesWithSeaMonster.Contains(c)  && character == '#');

        if (hasMonster)
        {
            coordinatesWithSeaMonster.UnionWith(coords);
        }

        return hasMonster;
        
        IEnumerable<(int x, int y)> GenerateCoordinates()
        {
            // Top row
            yield return (coordinate.x + 18, coordinate.y);
            
            // Middle row
            var middleRowOffsets = new int[]
            {
                0, 5, 6, 11, 12, 17, 18, 19
            };

            foreach (var offset in middleRowOffsets)
            {
                yield return (coordinate.x + offset, coordinate.y - 1);
            }
            
            var bottomRowOffsets = new int[]
            {
                1, 4, 7, 10, 13, 16
            };

            foreach (var offset in bottomRowOffsets)
            {
                yield return (coordinate.x + offset, coordinate.y - 2);
            }
        }
    }
    

    void PrintGrid()
    {
        var minX = minXCoordinate * 8;
        var minY = minYCoordinate * 8 - 7;

        var maxX = maxXCoordinate * 8 + 7;
        var maxY = maxYCoordinate * 8;

        for (var y = maxY; y >= minY; y--)
        {
            var line = new StringBuilder();
            
            for (var x = minX; x <= maxX; x++)
            {
                line.Append(fullGrid[(x, y)]);
            }
            
            Console.WriteLine(line);
        }
    }
});
one();

static string ReverseString(string input)
{
    return new(input.Reverse().ToArray());
}
record Tile(int Id, string[] Rows)
{
    public bool MatchesDirection(Tile tile, Direction direction)
    {
        var (one, two) = GetPair();

        return one == two;

        (string one, string two) GetPair()
        {
            return direction switch
            {
                Direction.Above => (Rows[0], tile.Rows[^1]),
                Direction.Right => (GetRightColumn(), tile.GetLeftColumn()),
                Direction.Below => (Rows[^1], tile.Rows[0]),
                Direction.Left => (GetLeftColumn(), tile.GetRightColumn()),
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
        }
    }

    private string GetRightColumn() => new(Rows.Select(r => r[^1]).ToArray());
    
    private string GetLeftColumn() => new(Rows.Select(r => r[0]).ToArray());

    public IEnumerable<Tile> AllCombinations()
    {
        return Enumerable.Range(0, 4).SelectMany(i => new[] {Rotate(i), FlipHorizontal().Rotate(i)});
    }

    public Tile FlipHorizontal()
    {
        return new(Id, Rows.Select(ReverseString).ToArray());
    }

    /// <summary>
    /// Rotates 90 deg clock-wise specified number of times
    /// </summary>
    public Tile Rotate(int times)
    {
        
        return Enumerable.Range(0, times).Aggregate(this, (tile, i) => tile.Rotate());
    }

    private Tile Rotate()
    {
        var grid = new char[Rows.Length, Rows.Length];

        for (var y = 0; y < Rows.Length; y++)
        for (var x = 0; x < Rows[y].Length; x++)
        {
            grid[x, y] = Rows[y][x];
        }

        var rot = RotateMatrix(grid, Rows.Length);

        var rows = new string[Rows.Length];
        for (var y = 0; y < Rows.Length; y++)
        {
            var row = new StringBuilder();
            for (var x = 0; x < Rows[y].Length; x++)
            {
                row.Append(rot[x, y]);
            }

            rows[y] = row.ToString();
        }
        
        return new (Id, rows);
    }


    private static string ReverseString(string input)
    {
        return new(input.Reverse().ToArray());
    }

    private static char[,] RotateMatrix(char[,] matrix, int n)
    {
        var ret = new char[n,n];

        for (var i = 0; i < n; ++i) {
            for (var j = 0; j < n; ++j) {
                ret[i, j] = matrix[n - j - 1, i];
            }
        }

        return ret;
    }
};

enum Direction
{
    Above,
    Right,
    Below,
    Left
};
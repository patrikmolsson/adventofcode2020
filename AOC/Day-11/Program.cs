#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Day_11;

var input = File.ReadAllText("input.txt");
var list = input.Split("\r\n").ToArray();
var one = new TaskOne(list);
one.Run();
var two = new TaskTwo(list);
two.Run();

static Tile[][] CreateEmptyBoard(string[] lines)
{
}

namespace Day_11
{
    internal class TaskOne
    {
        private readonly Tile[][] _tiles;

        public TaskOne(string[] rows)
        {
            _tiles = Utils.CreateEmptyBoard(rows, 4);
            for (var row = 0; row < _tiles.Length; row += 1)
            for (var col = 0; col < _tiles[row].Length; col += 1)
                foreach (var adjacentTile in GetAdjacentTiles(row, col))
                    _tiles[row][col].AdjacentSeats.Add(adjacentTile);

            IEnumerable<Tile> GetAdjacentTiles(int startRow, int startCol)
            {
                for (var rowDiff = -1; rowDiff <= 1; rowDiff += 1)
                for (var colDiff = -1; colDiff <= 1; colDiff += 1)
                {
                    if (rowDiff == 0 && colDiff == 0) continue;

                    var row = startRow + rowDiff;
                    var col = startCol + colDiff;

                    if (row < 0 || row > _tiles.Length - 1) continue;
                    if (col < 0 || col > _tiles[row].Length - 1) continue;

                    yield return _tiles[row][col];
                }
            }
        }

        public void Run()
        {
            var allTiles = _tiles.SelectMany(s => s.Select(t => t)).ToList();

            while (true)
            {
                var tilesToFlip = new HashSet<Tile>();

                foreach (var tile in allTiles.Where(tile => tile.ShouldFlip()))
                    tilesToFlip.Add(tile);

                if (tilesToFlip.Count == 0) break;

                foreach (var tile in tilesToFlip) tile.Flip();
            }

            var occupiedSeats = allTiles.Count(t => t.CurrentTile == TileType.Occupied);

            Console.WriteLine($"Occupied seats: {occupiedSeats}");
        }
    }

    internal class TaskTwo
    {
        private readonly Tile[][] _tiles;

        public TaskTwo(string[] rows)
        {
            _tiles = Utils.CreateEmptyBoard(rows, 5);
            
            for (var row = 0; row < _tiles.Length; row += 1)
            for (var col = 0; col < _tiles[row].Length; col += 1)
                foreach (var adjacentTile in GetAdjacentTiles(row, col))
                    _tiles[row][col].AdjacentSeats.Add(adjacentTile);

            IEnumerable<Tile> GetAdjacentTiles(int startRow, int startCol)
            {
                for (var rowDiff = -1; rowDiff <= 1; rowDiff += 1)
                for (var colDiff = -1; colDiff <= 1; colDiff += 1)
                {
                    if (rowDiff == 0 && colDiff == 0) continue;

                    var firstSeat = GetFirstSeatInDirection();

                    if (firstSeat != null) yield return firstSeat;

                    Tile? GetFirstSeatInDirection()
                    {
                        var step = 1;
                        while (true)
                        {
                            var row = startRow + rowDiff * step;
                            var col = startCol + colDiff * step;

                            if (row < 0 || row > _tiles.Length - 1) return null;
                            if (col < 0 || col > _tiles[row].Length - 1) return null;

                            var tile = _tiles[row][col];
                            if (tile.CurrentTile != TileType.Floor) return tile;

                            step += 1;
                        }
                    }
                }
            }
        }

        public void Run()
        {
            var allTiles = _tiles.SelectMany(s => s.Select(t => t)).ToList();

            while (true)
            {
                var tilesToFlip = new HashSet<Tile>();

                foreach (var tile in allTiles.Where(tile => tile.ShouldFlip()))
                    tilesToFlip.Add(tile);

                if (tilesToFlip.Count == 0) break;

                foreach (var tile in tilesToFlip) tile.Flip();
            }

            var occupiedSeats = allTiles.Count(t => t.CurrentTile == TileType.Occupied);

            Console.WriteLine($"Occupied seats: {occupiedSeats}");
        }
    }

    internal class Tile
    {
        private readonly int _col;
        private readonly int _requiredSeatsForOccupiedToFlip;
        private readonly int _row;
        public readonly HashSet<Tile> AdjacentSeats = new();

        public Tile(int col, int row, TileType currentTile, int requiredSeatsForOccupiedToFlip)
        {
            _requiredSeatsForOccupiedToFlip = requiredSeatsForOccupiedToFlip;
            _col = col;
            _row = row;
            CurrentTile = currentTile;
        }

        public TileType CurrentTile { get; private set; }

        public override int GetHashCode()
        {
            return _col.GetHashCode() + _row.GetHashCode();
        }

        public void Flip()
        {
            switch (CurrentTile)
            {
                case TileType.Floor: break;
                case TileType.Empty:
                {
                    CurrentTile = TileType.Occupied;
                    break;
                }
                case TileType.Occupied:
                {
                    CurrentTile = TileType.Empty;
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public bool ShouldFlip()
        {
            return CurrentTile switch
            {
                TileType.Floor => false,
                TileType.Empty => AdjacentSeats.All(s => s.CurrentTile != TileType.Occupied),
                TileType.Occupied => AdjacentSeats.Count(s => s.CurrentTile == TileType.Occupied) >=
                                     _requiredSeatsForOccupiedToFlip,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    internal enum TileType
    {
        Floor = '.',
        Occupied = '#',
        Empty = 'L'
    }

    internal static class Utils
    {
        public static Tile[][] CreateEmptyBoard(string[] lines, int requiredSeatsForOccupiedToFlip)
        {
            return lines
                .Select((line, row) => line
                    .ToCharArray()
                    .Select((tile, col) => new Tile(col, row, (TileType) tile, requiredSeatsForOccupiedToFlip))
                    .ToArray())
                .ToArray();
        }
    }
}
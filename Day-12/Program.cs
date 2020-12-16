using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Day_12;

var input = File.ReadAllText("input.txt");
var list = input.Split("\r\n").ToArray();
var one = new TaskOne(list);
one.Run();
var two = new TaskTwo(list);
two.Run();

namespace Day_12
{
    internal class TaskOne
    {
        private readonly Ferry _ferry = new((1, 0), true);
        private readonly (char, int)[] _instructions;

        public TaskOne(string[] rows)
        {
            _instructions = rows.Select(row => (row[0], int.Parse(row.Substring(1)))).ToArray();
        }

        public void Run()
        {
            _ferry.Run(_instructions);
        }
    }

    internal class TaskTwo
    {
        private readonly Ferry _ferry = new((10, 1), false);
        private readonly (char, int)[] _instructions;

        public TaskTwo(string[] rows)
        {
            _instructions = rows.Select(row => (row[0], int.Parse(row.Substring(1)))).ToArray();
        }

        public void Run()
        {
            _ferry.Run(_instructions);
        }
    }

    internal class Ferry
    {
        private readonly bool _moveShipOnDirection;
        private (int x, int y) _position = (0, 0);
        private (int x, int y) _waypoint;

        public Ferry((int x, int y) waypoint, bool moveShipOnDirection)
        {
            _waypoint = waypoint;
            _moveShipOnDirection = moveShipOnDirection;
        }

        public void Run(IEnumerable<(char, int)> instructions)
        {
            foreach (var (operation, magnitude) in instructions) Act(operation, magnitude);

            Console.WriteLine(
                $"{_position.x} {_position.y}: {Math.Abs(_position.x) + Math.Abs(_position.y)}");
        }

        private void Act(char operation, int magnitude)
        {
            switch (operation)
            {
                case 'N':
                case 'S':
                case 'E':
                case 'W':
                {
                    Move(magnitude, operation);
                    break;
                }
                case 'F':
                {
                    MoveShipForward(magnitude);
                    break;
                }
                case 'R':
                case 'L':
                {
                    var clockwise = operation == 'R';
                    Turn(clockwise, magnitude);
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException(nameof(operation), operation, null);
                }
            }
        }

        private void Move(int steps, char dirChar)
        {
            var direction = DirectionFromChar();

            if (_moveShipOnDirection)
                MoveShip(steps, direction);
            else
                MoveWaypoint(steps, direction);


            (int x, int y) DirectionFromChar()
            {
                return dirChar switch
                {
                    'N' => (0, 1),
                    'S' => (0, -1),
                    'E' => (1, 0),
                    'W' => (-1, 0),
                    _ => throw new ArgumentOutOfRangeException(nameof(dirChar), dirChar, null)
                };
            }
        }

        private void MoveWaypoint(int steps, (int x, int y) direction)
        {
            MoveEntity(ref _waypoint, steps, direction);
        }

        private void MoveShipForward(int steps)
        {
            MoveShip(steps, _waypoint);
        }

        private void MoveShip(int steps, (int x, int y) waypoint)
        {
            MoveEntity(ref _position, steps, waypoint);
        }

        private static void MoveEntity(ref (int x, int y) entity, int steps, (int x, int y) waypoint)
        {
            entity.x += waypoint.x * steps;
            entity.y += waypoint.y * steps;
        }

        private void Turn(bool clockWise, int absoluteDegrees)
        {
            var sign = clockWise ? -1 : 1;
            var degrees = sign * absoluteDegrees;
            var angle = Math.PI * degrees / 180.0;

            var previousX = _waypoint.x;
            _waypoint.x = (int) (_waypoint.x * Math.Cos(angle) - _waypoint.y * Math.Sin(angle));
            _waypoint.y = (int) (_waypoint.y * Math.Cos(angle) + previousX * Math.Sin(angle));
        }
    }
}
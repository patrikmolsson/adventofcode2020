using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

var input = File.ReadAllText("input.txt");
var list = input.Split("\r\n").ToArray();
var one = new TaskOne(list[0], list[1]);
one.Run();
var two = new TaskTwo(list[1]);
two.Run();

internal class TaskOne
{
    private readonly Bus[] _buses;
    private readonly int _startTime;

    public TaskOne(string first, string buses)
    {
        _startTime = int.Parse(first);
        _buses = buses.Split(',').Where(x => x != "x").Select(x => new Bus(int.Parse(x), 0)).ToArray();
    }

    public void Run()
    {
        var currentTime = _startTime;
        while (true)
        {
            var bus = _buses.SingleOrDefault(b => b.DepartsAt(currentTime));
            if (bus != null)
            {
                Console.WriteLine($"#1 Found bus {bus.BusId} at {currentTime}: {(currentTime - _startTime) * bus.BusId}");
                break;
            }

            currentTime += 1;
        }
    }
}

internal class TaskTwo
{
    private readonly Bus[] _buses;

    public TaskTwo(string buses)
    {
        _buses = buses
            .Split(',')
            .Select((s, i) =>
                s != "x" ? new Bus(int.Parse(s), i) : null
            )
            .Where(bus => bus != null)
            .ToArray();
    }

    public void Run()
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        long stepSize = 1;
        long startTime = 1;
        for (var i = 0; i < _buses.Length; i++)
        {
            (startTime, stepSize) = GetSteps(startTime, stepSize, i + 1);
        }

        stopwatch.Stop();
        Console.WriteLine($"#2 Time with first sequence: {startTime}");
        Console.WriteLine($"#2 Time elapsed: {stopwatch.Elapsed}");
    }

    private (long startTime, long stepSize) GetSteps(long startTime, long stepSize, int busCount)
    {
        var time = startTime;
        var buses = _buses.Take(busCount).ToList();

        while (true)
        {
            if (buses.All(bus => bus.DepartsAsSequence(time)))
            {
                var frequency = buses.Aggregate(1L, (l, bus) => l * bus.BusId);

                return (time, frequency);
            }

            time += stepSize;
        }
    }
}

internal record Bus(int BusId, int Offset)
{
    public bool DepartsAsSequence(long time)
    {
        return DepartsAt(time + Offset);
    }

    public bool DepartsAt(long time)
    {
        return time % BusId == 0;
    }
}
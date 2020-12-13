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
                Console.WriteLine($"Found bus {bus.BusId} at {currentTime}: {(currentTime - _startTime) * bus.BusId}");
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
        _buses = buses.Split(',').Select((x, i) => new Bus(x != "x" ? int.Parse(x) : null, i)).ToArray();
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

    private (long startTime, long stepSize) GetSteps(long startTime, long stepSize, int buses)
    {
        var time = startTime;

        long? firstMatch = null;

        while (true)
        {
            if (_buses.Take(buses).All(bus => bus.BusId == null || bus.DepartsAsSequence(time)))
            {
                if (firstMatch != null)
                {
                    return (firstMatch.Value, time - firstMatch.Value);
                }

                firstMatch = time;
            }

            time += stepSize;
        }
    }
}

internal record Bus(int? BusId, int Offset)
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

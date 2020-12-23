using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

void One()
{
    var (cups, activeCup) = BuildGraph(0);

    var moves = 100;
    Run(cups, activeCup, moves);

    var str = new StringBuilder();
    var c = cups[1].Next;
    while (c != cups[1])
    {
        str.Append(c.Number);
        c = c.Next;
    }
    
    Console.WriteLine($"[One] String: {str}");
}

void Two()
{
    var stopwatch = Stopwatch.StartNew();

    var (cups, activeCup) = BuildGraph(999_991);

    var moves = 10_000_000;
    Run(cups, activeCup, moves);

    var first = cups[1].Next.Number;
    var second = cups[1].Next.Next.Number;

    Console.WriteLine($"[Two] First: {first} Second: {second} Product: {(long) first * second}. El: {stopwatch.Elapsed}");
}

(IDictionary<int, Cup> cups, Cup activeCup) BuildGraph(int additionalCups)
{
    var input = "469217538";
    var cups = new Dictionary<int, Cup>();

    // Build graph
    Cup lastCup = null;
    var cupNumbers = input.ToCharArray().Select(s => int.Parse(s.ToString()))
        .Concat(Enumerable.Range(input.Length + 1, additionalCups))
        .ToArray();

    foreach (var cupNumber in cupNumbers)
    {
        var current = new Cup(cupNumber);
        cups.Add(cupNumber, current);

        if (lastCup != null) lastCup.Next = current;

        lastCup = current;
    }

    var activeCup = cups[cupNumbers[0]];
    lastCup.Next = activeCup;

    return (cups, activeCup);
}

void Run(IDictionary<int, Cup> cups, Cup activeCup, int moves)
{
    for (var i = 1; i <= moves; i++)
    {
        // Get slice
        var startPickedUp = activeCup.Next;
        var endPickedUp = activeCup.Next.Next.Next;

        // Remove slice
        activeCup.Next = endPickedUp.Next;
        endPickedUp.Next = null;

        // Where to place it?
        var destinationCupNumber = activeCup.Number - 1;
        while (!cups.ContainsKey(destinationCupNumber) || startPickedUp.ChainContains(destinationCupNumber))
            destinationCupNumber = destinationCupNumber == 0 ? cups.Count : destinationCupNumber - 1;

        // Insert slice
        var destinationCup = cups[destinationCupNumber];
        endPickedUp.Next = destinationCup.Next;
        destinationCup.Next = startPickedUp;

        // New active cup
        activeCup = activeCup.Next;
    }
}

One();

Two();

internal class Cup
{
    public readonly int Number;

    public Cup(int number)
    {
        Number = number;
    }

    public Cup Next { get; set; }

    public bool ChainContains(int number)
    {
        var c = this;

        while (c != null)
        {
            if (c.Number == number) return true;

            c = c.Next;
        }

        return false;
    }
}
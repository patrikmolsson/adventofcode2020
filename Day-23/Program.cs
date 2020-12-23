using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

var one = new Action(() =>
{
    var input = "469217538";

    var cups = input.ToCharArray().Select(s => int.Parse(s.ToString())).ToList();

    var activeCup = cups[0];
    var moves = 100;

    for (var i = 1; i <= moves; i++)
    {
        var activeIndex = cups.IndexOf(activeCup);

        // Figure out which cups
        var pickedUpCups = cups.Concat(cups).Skip(activeIndex + 1).Take(3).ToArray();

        // Remove cups
        cups = cups.Except(pickedUpCups).ToList();

        // Where to place it?
        var destinationCup = activeCup - 1;

        while (!cups.Contains(destinationCup)) destinationCup = destinationCup == 0 ? cups.Max() : destinationCup - 1;

        var indexOfDestinationCup = cups.IndexOf(destinationCup);

        cups.InsertRange(indexOfDestinationCup + 1, pickedUpCups);

        // Calculate new active cup
        activeIndex = cups.IndexOf(activeCup);

        activeCup = cups.Concat(cups).ElementAt(activeIndex + 1);
    }

    var indexOfOne = cups.IndexOf(1);

    var str = string.Join("", cups.Concat(cups).Skip(indexOfOne + 1).TakeWhile(s => s != 1).Select(s => s.ToString()));

    Console.WriteLine($"[One] String: {str}");
});
var two = new Action(() =>
{
    var stopwatch = Stopwatch.StartNew();
    var input = "469217538";

    var cups = new Dictionary<int, Cup>();

    // Build graph
    Cup lastCup = null;
    var cupNumbers = input.ToCharArray().Select(s => int.Parse(s.ToString()))
        .Concat(Enumerable.Range(10, 999_991))
        .ToArray();

    foreach (var cupNumber in cupNumbers)
    {
        var current = new Cup(cupNumber);
        cups.Add(cupNumber, current);

        if (lastCup != null) lastCup.Next = current;

        lastCup = current;
    }

    lastCup.Next = cups[cupNumbers[0]];

    var activeCupNumber = cupNumbers[0];
    var moves = 10_000_000;
    for (var i = 1; i <= moves; i++)
    {
        var activeCup = cups[activeCupNumber];

        var startPickedUp = activeCup.Next;
        var endPickedUp = activeCup.Next.Next.Next;

        // Remove slice
        activeCup.Next = endPickedUp.Next;
        endPickedUp.Next = null;

        // Where to place it?
        var destinationCupNumber = activeCupNumber - 1;
        while (!cups.ContainsKey(destinationCupNumber) || startPickedUp.ChainContains(destinationCupNumber))
            destinationCupNumber = destinationCupNumber == 0 ? cups.Count : destinationCupNumber - 1;

        // Insert slice
        var destinationCup = cups[destinationCupNumber];
        var destNext = destinationCup.Next;

        destinationCup.Next = startPickedUp;
        endPickedUp.Next = destNext;

        // New active cup number
        activeCupNumber = activeCup.Next.Number;
    }

    var first = cups[1].Next.Number;
    var second = cups[1].Next.Next.Number;

    Console.WriteLine($"[Two] First: {first} Second: {second} Product: {(long) first * second}. El: {stopwatch.Elapsed}");
});
one();
two();

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

        do
        {
            if (c.Number == number) return true;

            c = c.Next;
        } while (c != null);

        return false;
    }
}
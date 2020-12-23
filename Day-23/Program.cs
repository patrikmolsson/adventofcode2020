using System;
using System.Linq;

// var input = "389125467";
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

    while (!cups.Contains(destinationCup))
    {
        destinationCup = destinationCup == 0 ? cups.Max() : destinationCup - 1;
    }

    var indexOfDestinationCup = cups.IndexOf(destinationCup);
    
    cups.InsertRange(indexOfDestinationCup + 1, pickedUpCups);
    
    // Calculate new active cup
    activeIndex = cups.IndexOf(activeCup);

    activeCup = cups.Concat(cups).ElementAt(activeIndex + 1);
}

var indexOfOne = cups.IndexOf(1);

var str = string.Join("", cups.Concat(cups).Skip(indexOfOne + 1).TakeWhile(s => s != 1).Select(s => s.ToString()));

Console.WriteLine($"String: {str}");
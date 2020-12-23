using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

var one = new Action(() =>
{
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
});


var two = new Action(() =>
{    
    // var input = "389125467";
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

         if (lastCup != null)
         {
             lastCup.Next = current;
         }
         
         lastCup = current;
     }

     lastCup.Next = cups[cupNumbers[0]];
     
     var activeCupNumber = cupNumbers[0];
     var moves = 10_000_000;
     for (var i = 1; i <= moves; i++)
     {
         var activeCup = cups[activeCupNumber];
         
         var startPickedUp = activeCup.Next;
         var endPickedUp = activeCup.Third;

         // Remove slice
         activeCup.Next = endPickedUp.Next;
         endPickedUp.Next = null;
 
         // Where to place it?
         var pickedUpCupNumbers = startPickedUp.ChainedNumbers;
         
         var destinationCupNumber = activeCupNumber - 1;
         while (!cups.ContainsKey(destinationCupNumber) || pickedUpCupNumbers.Contains(destinationCupNumber))
         {
             destinationCupNumber = destinationCupNumber == 0 ? cups.Count : destinationCupNumber - 1;
         }

         // Insert slice
         var destinationCup = cups[destinationCupNumber];
         var destNext = destinationCup.Next;

         destinationCup.Next = startPickedUp;
         endPickedUp.Next = destNext;
         
         // New active cup
         activeCupNumber = activeCup.Next.Number;
     }

     var first = cups[1].Next.Number;
     var second = cups[1].Next.Next.Number;
 
     Console.WriteLine($"String: {first} {second}: {(long) first * second}");

});

two();

internal class Cup
{
    public readonly int Number;

    public Cup(int number)
    {
        Number = number;
    }
    public Cup Next { get; set; }

    public Cup Third => Next!.Next!.Next!;

    public HashSet<int> ChainedNumbers 
    {
        get
        {
            var set = new HashSet<int>
            {
                Number
            };
            
            var c = this;
            while (c.Next != null)
            {
                c = c.Next;
                set.Add(c.Number);
            }

            return set;
        }
    }
}
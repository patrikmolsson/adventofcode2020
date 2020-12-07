using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day_07
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt");

            var list = input.Split("\r\n").ToArray();

            new TaskOne(list);
            new TaskTwo(list);
        }
    }
    
    internal class TaskOne
    {
        private readonly IDictionary<string, Bag> _map = new Dictionary<string, Bag>();

        public TaskOne(string[] list)
        {
            foreach (var row in list)
            {
                var bag = new Bag(row);
                _map.Add(bag.Id, bag);
            }

            var count = _map.Count(pair => BagContains(pair.Value, "shiny gold"));

            Console.WriteLine(count);
        }

        private bool BagContains(Bag bag, string bagToCheck)
        {
            if (bag.NestedBags.Count == 0) return false;

            if (bag.NestedBags.Any(b => b.Key == bagToCheck)) return true;

            return bag.NestedBags.Any(b => BagContains(_map[b.Key], bagToCheck));
        }
    }

    internal class TaskTwo
    {
        private readonly IDictionary<string, Bag> _map = new Dictionary<string, Bag>();

        public TaskTwo(string[] list)
        {
            foreach (var row in list)
            {
                var bag = new Bag(row);
                _map.Add(bag.Id, bag);
            }

            var count = CountBags(_map["shiny gold"]);

            // Minus the shiny gold bag itself
            Console.WriteLine(count - 1);
        }

        private int CountBags(Bag bag)
        {
            return bag.NestedBags.Sum(b => b.Value * CountBags(_map[b.Key])) + 1;
        }
    }

    internal class Bag
    {
        public readonly string Id;

        public readonly Dictionary<string, int> NestedBags = new Dictionary<string, int>();

        public Bag(string row)
        {
            var matches = Regex.Matches(row, @"\w+");

            Id = $"{matches[0]} {matches[1]}";

            // 5,6
            // 9,10
            // 13,14
            for (var i = 5; i < matches.Count; i += 4)
            {
                if (matches[i].Value == "other") continue;
                
                NestedBags.Add($"{matches[i]} {matches[i + 1]}", int.Parse(matches[i - 1].ToString()));
            }
        }
    }
}
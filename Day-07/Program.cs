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

            var one = new TaskOne(list);
            one.Run();
            var two = new TaskTwo(list);
            two.Run();
        }

        private static IDictionary<string, Bag> CreateMap(string[] list)
        {
            var map = new Dictionary<string, Bag>();
            foreach (var row in list)
            {
                var matches = Regex.Matches(row, @"\w+");
                var id = $"{matches[0]} {matches[1]}";

                var bag = GetOrAdd(id);

                // 5,6
                // 9,10
                // 13,14
                for (var i = 5; i < matches.Count; i += 4)
                {
                    if (matches[i].Value == "other") continue;

                    var nestedBagId = $"{matches[i]} {matches[i + 1]}";
                    var nestedBag = GetOrAdd(nestedBagId);
                    var nestedBagCount = int.Parse(matches[i - 1].ToString());

                    bag.NestedBags.Add(nestedBag, nestedBagCount);
                }

                map.TryAdd(bag.Id, bag);
            }


            return map;

            Bag GetOrAdd(string id)
            {
                if (map.TryGetValue(id, out var bag)) return bag;

                var newBag = new Bag(id);

                map.Add(id, newBag);

                return newBag;
            }
        }

        private class TaskOne
        {
            private readonly IDictionary<string, Bag> _map;

            public TaskOne(string[] list)
            {
                _map = CreateMap(list);
            }

            public void Run()
            {
                var count = _map.Count(pair => pair.Value.Contains(_map["shiny gold"]));

                Console.WriteLine(count);
            }
        }

        private class TaskTwo
        {
            private readonly IDictionary<string, Bag> _map;

            public TaskTwo(string[] list)
            {
                _map = CreateMap(list);
            }

            public void Run()
            {
                var count = _map["shiny gold"].CountBags();

                // Minus the shiny gold bag itself
                Console.WriteLine(count - 1);
            }
        }

        internal class Bag
        {
            public readonly string Id;

            public readonly Dictionary<Bag, int> NestedBags = new Dictionary<Bag, int>();

            public Bag(string id)
            {
                Id = id;
            }

            public override string ToString()
            {
                return Id;
            }

            public override int GetHashCode()
            {
                return Id.GetHashCode();
            }

            public bool Contains(Bag bag)
            {
                return NestedBags.Any(nested => nested.Key.Id == bag.Id)
                       || NestedBags.Any(nested => nested.Key.Contains(bag));
            }

            public int CountBags()
            {
                return NestedBags.Sum(b => b.Value * b.Key.CountBags()) + 1;
            }
        }
    }
}
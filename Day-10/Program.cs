using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_10
{
    class Program
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
    }

    internal class TaskOne
    {
        private IList<int> _list;
        
        public TaskOne(string[] list)
        {
            _list = new List<int>
            {
                0
            };
            
            _list = _list.Concat(list.Select(int.Parse).OrderBy(s => s)).ToList();

            // Add the device itself
            _list.Add(_list.Max() + 3);
        }

        public void Run()
        {
            var differences = new Dictionary<int, int>
            {
                {1, 0},
                {2, 0},
                {3, 0},
            };

            for (var i = 0; i < _list.Count - 1; i += 1)
            {
                var adapter = _list[i];
                var nextAdapter = _list[i + 1];

                differences[nextAdapter - adapter] += 1;
            }

            Console.Out.WriteLine($"Product is {differences[1] * differences[3]}");
        }
    }

    internal class TaskTwo
    {
        private IList<int> _list;
        
        public TaskTwo(string[] list)
        {
            _list = new List<int>
            {
                0
            };
            
            _list = _list.Concat(list.Select(int.Parse).OrderBy(s => s)).ToList();

            // Add the device itself
            _list.Add(_list.Max() + 3);
        }
        
        public void Run()
        {
            var leafNodeCount = _list.ToDictionary(adapter => adapter, _ => (long) 0);
            const int adapterRange = 3;

            leafNodeCount[0] = 1;
            long distinctWays = 1;
            
            for (var i = 0; i < _list.Count - 1; i += 1)
            {
                var adapter = _list[i];

                var possibleNextAdaptersCount = 0;
                for (var j = i + 1; j < _list.Count; j += 1)
                {
                    var nextAdapter = _list[j];
                    if (nextAdapter - adapter > adapterRange)
                    {
                        break;
                    }

                    possibleNextAdaptersCount += 1;
                    leafNodeCount[nextAdapter] += leafNodeCount[adapter];
                }

                distinctWays += leafNodeCount[adapter] * (possibleNextAdaptersCount - 1);
            }
            
            Console.Out.WriteLine($"Distinct ways: {distinctWays}");
        }
    }
}
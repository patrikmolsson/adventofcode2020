using System;
using System.IO;
using System.Linq;

namespace Day_09
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt");

            var list = input.Split("\r\n").ToArray();

            var one = new TaskOne(list);
            var invalidItem = one.Run(25);
            
            var two = new TaskTwo(list);
            two.Run(invalidItem);
            
        }

        private class TaskOne
        {
            private readonly long[] _list;

            public TaskOne(string[] list)
            {
                _list = list.Select(long.Parse).ToArray();
            }

            public long Run(int preambleLength)
            {
                for (var i = preambleLength; i < _list.Length; i += 1)
                {
                    if (IsValid(i))
                    {
                        continue;
                    }
                    
                    Console.Out.WriteLine($"Invalid item! idx:{i} item:{_list[i]}");
                    
                    return _list[i];
                }
                
                throw new InvalidOperationException("No invalid item found");

                bool IsValid(int index)
                {
                    var previousItems = _list.Skip(index - preambleLength).Take(preambleLength).ToList();

                    var previousSums = previousItems.SelectMany(term1 =>
                        previousItems.Where(term2 => term1 != term2).Select(term2 => term1 + term2));

                    return previousSums.Contains(_list[index]);
                }
            }
        }
        
        private class TaskTwo
        {
            private readonly long[] _list;

            private (int startIndex, int length)? _contiguous;

            public TaskTwo(string[] list)
            {
                _list = list.Select(long.Parse).ToArray();
            }

            public void Run(long invalidNumber)
            {
                 for (var i = 0; i < _list.Length; i += 1)
                 {
                     var contiguousLength = GetContiguousLength(i);

                     if (contiguousLength == null)
                     {
                         continue;
                     }

                     if (_contiguous == null || _contiguous?.length < contiguousLength)
                     {
                         _contiguous = (i, contiguousLength.Value);
                     }
                 }

                 Console.WriteLine($"Weakness: {EncryptionWeakness()}");

                 long EncryptionWeakness()
                 {
                     if (_contiguous == null)
                     {
                         throw new InvalidOperationException("No contiguous found!");
                     }

                     var range = _list.Skip(_contiguous.Value.startIndex).Take(_contiguous.Value.length).ToArray();

                     return range.Min() + range.Max();
                 }
 
                 int? GetContiguousLength(int startIndex)
                 {
                     long sum = 0;

                     var length = 0;
                     for (var i = startIndex; i < _list.Length; i += 1)
                     {
                         if (sum >= invalidNumber)
                         {
                             break;
                         }

                         sum += _list[i];
                         length += 1;
                     }

                     return sum == invalidNumber ? length : (int?) null;
                 }               
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

var input = File.ReadAllText("input.txt");
var list = input.Split("\r\n").ToArray();
var taskOne = new Action(() =>
{
    var memory = new Dictionary<int, long>();
    Bitmask mask = null;

    foreach (var row in list)
    {
        var matches = Regex.Matches(row, "\\w+");

        var operation = matches[0].Value;
        switch (operation)
        {
            case "mask":
                mask = new Bitmask(matches[1].Value);
                break;
            case "mem":
            {
                if (mask is null) throw new InvalidOperationException("Mask is null");

                var memoryAddress = int.Parse(matches[1].Value);
                var memoryValue = int.Parse(matches[2].Value);

                memory[memoryAddress] = mask.MaskValue(memoryValue);
                break;
            }
            default:
            {
                throw new InvalidOperationException($"Invalid operation '{operation}'");
            }
        }
    }

    var sum = memory.Aggregate(0L, (l, pair) => l + pair.Value);

    Console.WriteLine($"#1: Sum {sum}");
});

var taskTwo = new Action(() =>
{
    var memory = new Dictionary<long, long>();
    Bitmask mask = null;

    foreach (var row in list)
    {
        var matches = Regex.Matches(row, "\\w+");

        var operation = matches[0].Value;
        switch (operation)
        {
            case "mask":
                mask = new Bitmask(matches[1].Value);
                break;
            case "mem":
            {
                if (mask is null) throw new InvalidOperationException("Mask is null");

                var rawAddress = int.Parse(matches[1].Value);
                var memoryValue = int.Parse(matches[2].Value);

                var addresses = mask.MaskAddress(rawAddress);

                foreach (var address in addresses) memory[address] = memoryValue;

                break;
            }
            default:
            {
                throw new InvalidOperationException($"Invalid operation '{operation}'");
            }
        }
    }

    var sum = memory.Aggregate(0L, (l, pair) => l + pair.Value);
    Console.WriteLine($"#2: Sum {sum}");
});

taskOne();
taskTwo();

internal record Bitmask(string Mask)
{
    private const char Floating = 'X';

    public long MaskValue(long input)
    {
        var maskedString = MaskInputAsString(input, Floating);

        return Convert.ToInt64(maskedString, 2);
    }

    public IEnumerable<long> MaskAddress(long address)
    {
        var floatingAddress = MaskInputAsString(address, '0');

        return MaskAddressInternal(floatingAddress);

        static IEnumerable<long> MaskAddressInternal(string floatingAddress)
        {
            if (!floatingAddress.Contains(Floating)) return new[] {Convert.ToInt64(floatingAddress, 2)};

            var first = MaskAddressInternal(ReplaceFirstChar(floatingAddress, '1'));
            var second = MaskAddressInternal(ReplaceFirstChar(floatingAddress, '0'));

            return first.Concat(second);
        }

        static string ReplaceFirstChar(string input, char newChar)
        {
            var charArray = input.ToCharArray();

            var index = input.IndexOf(Floating);
            charArray[index] = newChar;

            return string.Join("", charArray);
        }
    }

    private string MaskInputAsString(long input, char ignoredChar)
    {
        var asString = Convert.ToString(input, 2);
        asString = asString.PadLeft(Mask.Length, '0');

        var charArray = asString.ToCharArray();

        for (var i = 0; i < Mask.Length; i++)
            if (Mask[i] != ignoredChar)
                charArray[i] = Mask[i];

        return string.Join("", charArray);
    }
}
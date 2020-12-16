using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Validator = System.Func<int, bool>;

var validators = new Dictionary<string, Validator>();
var input = File.ReadAllText("input.txt");
var inputGroups = input.Split($"{Environment.NewLine}{Environment.NewLine}").ToArray();
var validatorRows = inputGroups[0].Split(Environment.NewLine);
var myTicket = RowToTicket(inputGroups[1].Split(Environment.NewLine).ElementAt(1));
var nearbyTickets = inputGroups[2].Split(Environment.NewLine).Skip(1).Select(RowToTicket).ToList();
foreach (var row in validatorRows)
{
    var category = Regex.Match(row, @"\w+\s?\w*");
    var inputs = Regex.Matches(row, @"\d+");

    var categoryValidators = new List<Validator>();

    for (var i = 0; i < inputs.Count - 1; i += 2)
    {
        var lower = int.Parse(inputs[i].ToString());
        var higher = int.Parse(inputs[i + 1].ToString());

        categoryValidators.Add(RangeValidatorFactory(lower, higher));
    }

    validators.Add(category.ToString(), OrValidatorFactory(categoryValidators));
}

var taskOne = new Action(() =>
{
    var sumInvalid = nearbyTickets
        .Sum(ticket => ticket.Where(i => !IsNumberValid(i)).Sum());

    Console.WriteLine($"[#1] Error rate: {sumInvalid}");
});
var taskTwo = new Action(() =>
{
    var validTickets = nearbyTickets
        .Where(IsTicketValid)
        .Concat(new []{myTicket})
        .ToList();

    var indexToColumnValues = Enumerable.Range(0, validTickets[0].Count)
        .ToDictionary(i => i, index => validTickets.Select(ticket => ticket[index]));

    var validatorsToValidIndexes = validators
        .ToDictionary(
            f => f.Key,
            f => indexToColumnValues.Where(col => col.Value.All(num => f.Value(num))).Select(col => col.Key).ToList());

    var indexToValidator = new Dictionary<int, string>();
    foreach (var (validatorName, validColumns) in validatorsToValidIndexes.OrderBy(v => v.Value.Count))
    {
        var remainingColumn = validColumns.Single(v => !indexToValidator.ContainsKey(v));

        indexToValidator[remainingColumn] = validatorName;
    }

    var product = indexToValidator.Where(f => f.Value.Contains("departure", StringComparison.OrdinalIgnoreCase))
        .Aggregate(1L, (l, pair) => l * myTicket[pair.Key]);

    Console.WriteLine($"[#2] Product: {product}");
});
taskOne();
taskTwo();

bool IsTicketValid(IList<int> ticket)
{
    return ticket.All(IsNumberValid);
}

bool IsNumberValid(int number)
{
    return validators.Values.Any(f => f(number));
}

static Validator RangeValidatorFactory(int lower, int higher)
{
    return input => lower <= input && input <= higher;
}

static Validator OrValidatorFactory(IEnumerable<Validator> validators)
{
    return input => validators.Any(f => f(input));
}

static IList<int> RowToTicket(string row)
{
    return row.Split(',').Select(int.Parse).ToList();
}
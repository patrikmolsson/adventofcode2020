using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

var rows = File.ReadAllLines("input.txt");

var allergenToCandidates = new Dictionary<string, HashSet<string>>();

var foodCount = new Dictionary<string, int>();

foreach (var row in rows)
{
    var parts = row.Split(" (");

    var candidates = Regex.Matches(parts[0], @"\w+");

    foreach (var candidateMatch in candidates)
    {
        var candidate = candidateMatch.ToString();
        if (foodCount.TryGetValue(candidate, out var count))
        {
            foodCount[candidate] = count + 1;
        }
        else
        {
            foodCount[candidate] = 1;
        }
        
    }
    
    var allergens = Regex.Matches(parts[1], @"\w+").ToArray()[1..];
    
    foreach (var allergenMatch in allergens)
    {
        var allergen = allergenMatch.ToString();
        if (!allergenToCandidates.ContainsKey(allergen))
        {
            allergenToCandidates[allergen] = candidates.Select(s => s.ToString()).ToHashSet();
        }
        
        allergenToCandidates[allergen].IntersectWith(candidates.Select(s => s.ToString()));
    }
}

var foodNotContainsAllergens = foodCount.Keys.Where(k => !allergenToCandidates.Values.Any(c => c.Contains(k)));

var occurrenceCount = foodNotContainsAllergens.Aggregate(0, (i, s) => i + foodCount[s]);

Console.WriteLine(occurrenceCount.ToString());

var allergenToFood = new Dictionary<string, string>();

while (allergenToFood.Count != allergenToCandidates.Count)
{
    var singles = allergenToCandidates.Where(s => s.Value.Count == 1);

    foreach (var pair in singles)
    {
        var food = pair.Value.Single();
        allergenToFood[pair.Key] = food;

        foreach (var allergenToCandidate in allergenToCandidates)
        {
            allergenToCandidate.Value.Remove(food);
        }
    }
}

var output = string.Join(",", allergenToFood.OrderBy(s => s.Key).Select(s => s.Value));

Console.WriteLine(output);



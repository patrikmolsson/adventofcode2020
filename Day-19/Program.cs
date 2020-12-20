using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;


var input = File.ReadAllText("input.txt");
var groups = input.Split($"{Environment.NewLine}{Environment.NewLine}").ToArray();


var rulesRows = groups[0].Split(Environment.NewLine).ToArray();
var messages = groups[1].Split(Environment.NewLine).ToArray();

    var rawRules = rulesRows.ToDictionary(f => int.Parse(Regex.Match(f, @"^\d+").Value), f => f.Split(": ")[1]);

    var regexRules = new Dictionary<int, string>();

    string GetRegexRule(int index)
    {
        if (regexRules.ContainsKey(index))
        {
            return regexRules[index];
        }
    
        var rawRule = rawRules[index];
        if (rawRule == @"""a""")
        {
            return AddAndReturn("a");
        }
        if (rawRule == @"""b""")
        {
            return AddAndReturn("b");
        }
    
        rawRule = rawRule.Replace("|", ")|(");
        
        // Interpolate
        var matches = Regex.Matches(rawRule, @"\d+");
        foreach (var match in matches)
        {
            var nestedRule = match.ToString();
    
            if (nestedRule is null)
            {
                throw new InvalidOperationException();
            }

            var regex = new Regex(nestedRule);
            rawRule = regex.Replace(rawRule, $"({GetRegexRule(int.Parse(nestedRule))})", 1);
        }
    
        rawRule = Regex.Replace(rawRule, @"\s+", string.Empty);
        rawRule = $"({rawRule})";

        return AddAndReturn(rawRule);

        string AddAndReturn(string r)
        {
            regexRules.Add(index, r);
            return r;
        }
    }


var t = new Action(() =>
{
    var rule = GetRegexRule(0);
    var count = messages.Count(s => Regex.IsMatch(s, $"^({rule})$"));

    Console.WriteLine($"[One] Count: {count}");
});
var t2 = new Action(() =>
{
    var rule42 = GetRegexRule(42);
    var rule31 = GetRegexRule(31);

    var matchedMessages = new HashSet<string>();
    
    // Just trial-and-error, could hypothetically hit another one higher up
    var i = 1;
    var foundNewMessages = true;
    while (foundNewMessages)
    {
        var reg = new Regex($@"^(({rule42})+({rule42}){{{i}}}({rule31}){{{i}}})$");
        var m = messages.Where(s => reg.IsMatch(s)).ToHashSet();
        
        matchedMessages.UnionWith(m);
        
        foundNewMessages = m.Count > 0;
        i++;
    }
    
    Console.WriteLine($"[Two] Count: {matchedMessages.Count}");
});

t();
t2();


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

var i = File.ReadAllText("input.txt");
var groups = i.Split($"{Environment.NewLine}{Environment.NewLine}");
var decks = BuildDecks();

void One()
{
    Run(false);
}

void Two()
{
    Run(true);
}

void Run(bool recursive)
{
    var results = StartGame(decks.Select(d => new Queue<int>(d)).ToArray(), recursive);

    LogFinal(results.decks);
    var score = CalculateScore(results.decks[results.lastWinner]);

    Console.WriteLine(score);
}

One();
Two();

Queue<int>[] BuildDecks()
{
    var tempDecks = new Queue<int>[]
    {
        new(),
        new()
    };

    for (var group = 0; group < groups.Length; group++)
        foreach (var number in groups[group].Split(Environment.NewLine)[1..])
            tempDecks[group].Enqueue(int.Parse(number));

    return tempDecks;
}

static (int lastWinner, Queue<int>[] decks) StartGame(Queue<int>[] decks, bool supportsRecursive)
{
    var previousRounds = new HashSet<string>();
    var lastWinner = 1;
    while (decks.All(deck => deck.Any()))
    {
        var hash = CalculateHash(decks);
        if (previousRounds.Contains(hash)) return (0, decks);

        previousRounds.Add(hash);
        var cardOne = decks[0].Dequeue();
        var cardTwo = decks[1].Dequeue();

        // Should start recursive?
        if (supportsRecursive && decks[0].Count >= cardOne && decks[1].Count >= cardTwo)
            lastWinner = StartGame(new Queue<int>[]
            {
                new(decks[0].Take(cardOne)),
                new(decks[1].Take(cardTwo))
            }, true).lastWinner;
        // Fall-back to higher card wins
        else
            lastWinner = cardOne > cardTwo ? 0 : 1;

        if (lastWinner == 0)
        {
            decks[0].Enqueue(cardOne);
            decks[0].Enqueue(cardTwo);
        }
        else
        {
            decks[1].Enqueue(cardTwo);
            decks[1].Enqueue(cardOne);
        }
    }

    return (lastWinner, decks);
}

static string CalculateHash(Queue<int>[] decks)
{
    return string.Join("|", decks.Select(d => string.Join(",", d)));
}

static int CalculateScore(Queue<int> deck)
{
    var s = 0;

    var multiplier = deck.Count;
    foreach (var card in deck)
    {
        s += multiplier * card;
        multiplier--;
    }

    return s;
}

static void LogFinal(Queue<int>[] decks)
{
    foreach (var deck in decks)
    {
        var line = string.Join(",", deck);

        Console.WriteLine($"XX: {line}");
    }
}
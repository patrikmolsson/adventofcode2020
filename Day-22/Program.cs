using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

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
    var s = Stopwatch.StartNew();
    
    var results = StartGame(decks.Select(d => new Queue<int>(d)).ToArray(), recursive);

    LogFinal(results.decks);
    var score = CalculateScore(results.decks[results.lastWinner]);

    var elapsed = s.Elapsed;
    Console.WriteLine(score);
    Console.WriteLine($"Ran for {elapsed}");
}

One();
Two();

static Queue<int>[] BuildDecks()
{
    var i = File.ReadAllText("input.txt");
    var groups = i.Split($"{Environment.NewLine}{Environment.NewLine}");

    return groups
        .Select(group => new Queue<int>(group.Split(Environment.NewLine)[1..].Select(int.Parse)))
        .ToArray();
}

static (int lastWinner, Queue<int>[] decks) StartGame(Queue<int>[] decks, bool supportsRecursive)
{
    var previousRounds = new HashSet<string>();
    var lastWinner = -1;
    while (decks.All(deck => deck.Any()))
    {
        var hash = CalculateHash(decks);
        if (previousRounds.Contains(hash)) return (0, decks);

        previousRounds.Add(hash);
        var cardOne = decks[0].Dequeue();
        var cardTwo = decks[1].Dequeue();

        // Should start recursive?
        if (supportsRecursive && decks[0].Count >= cardOne && decks[1].Count >= cardTwo)
        {
            var results = StartGame(new Queue<int>[]
            {
                new(decks[0].Take(cardOne)),
                new(decks[1].Take(cardTwo))
            }, true);

            lastWinner = results.lastWinner;
        }
        // Fall back to higher card wins
        else
        {
            lastWinner = cardOne > cardTwo ? 0 : 1;
        }

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
    return deck.Reverse().Select((num, i) => num * (i + 1)).Sum();
}

static void LogFinal(Queue<int>[] decks)
{
    Console.WriteLine();

    var player = 1;
    foreach (var deck in decks)
    {
        var line = string.Join(",", deck);

        Console.WriteLine($"Player {player}: {line}");

        player++;
    }
}
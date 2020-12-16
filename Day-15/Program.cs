using System;
using System.Collections.Generic;
using System.Diagnostics;

var run = new Action<int>(turns =>
{
    var stopwatch = Stopwatch.StartNew();

    var numbersLastSpoken = new Dictionary<int, int>();
    var input = new[] {9, 6, 0, 10, 18, 2, 1};

    var lastSpokenNumber = 0;
    var nextNumberToSpeak = input[0];

    for (var turn = 1; turn <= turns; turn++)
    {
        var numberToSpeak = nextNumberToSpeak;

        lastSpokenNumber = numberToSpeak;
        nextNumberToSpeak = GetNumberToSpeak(turn + 1);

        numbersLastSpoken[lastSpokenNumber] = turn;
    }

    stopwatch.Stop();

    Console.WriteLine($"#[{turns:n0}]: {lastSpokenNumber}. Run time: {stopwatch.Elapsed}");

    int GetNumberToSpeak(int turn)
    {
        if (turn <= input.Length) return input[turn - 1];

        var prior = numbersLastSpoken.GetValueOrDefault(lastSpokenNumber);

        return prior == default ? 0 : turn - 1 - prior;
    }
});

// Task one 
run(2020);

// Task two
run(30_000_000);
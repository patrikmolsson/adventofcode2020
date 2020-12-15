using System;
using System.Collections.Generic;
using System.Diagnostics;

var run = new Action<int>(turns =>
{
    var stopwatch = Stopwatch.StartNew();

    var numbersLastSpoken = new Dictionary<int, int>();
    var input = new[] {9, 6, 0, 10, 18, 2, 1};

    var lastSpokenNumber = 0;
    var lastSpokenNumberLastSpokenPrior = 0;
    for (var turn = 1; turn <= turns; turn++)
    {
        var numberToSpeak = GetNumberToSpeak(turn);

        lastSpokenNumber = numberToSpeak;
        lastSpokenNumberLastSpokenPrior = numbersLastSpoken.GetValueOrDefault(lastSpokenNumber);

        numbersLastSpoken[lastSpokenNumber] = turn;
    }

    stopwatch.Stop();

    Console.WriteLine($"#[{turns:n0}]: {lastSpokenNumber}. Run time: {stopwatch.Elapsed}");

    int GetNumberToSpeak(int turn)
    {
        if (turn <= input.Length) return input[turn - 1];

        var last = numbersLastSpoken[lastSpokenNumber];

        return lastSpokenNumberLastSpokenPrior == 0 ? 0 : last - lastSpokenNumberLastSpokenPrior;
    }
});

// Task one 
run(2020);

// Task two
run(30_000_000);
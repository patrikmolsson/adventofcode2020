using System;

var publicKeyDoor = "3469259";
var publicKeyCard = "13170438";

var loopSizeDoor = GetLoopSize(publicKeyDoor);
var loopSizeCard = GetLoopSize(publicKeyCard);

Console.WriteLine($"[One]: Loop size card {loopSizeCard}");
Console.WriteLine($"[One]: Loop size door {loopSizeDoor}");

var encKeyOne = GetEncryptionKey(int.Parse(publicKeyDoor), loopSizeCard);
var encKeyTwo = GetEncryptionKey(int.Parse(publicKeyCard), loopSizeDoor);

Console.WriteLine(encKeyOne == encKeyTwo
    ? $"[One] Encryption keys match: {encKeyOne}!"
    : $"[One] Encryption keys does not match: {encKeyOne}, {encKeyTwo}!");

static long GetEncryptionKey(int subjectNumber, int loopSize)
{
    var value = 1L;
    
    for (var i = 0; i < loopSize; i++)
    {
        value *= subjectNumber;
        value %= 20201227;
    }

    return value;
}

static int GetLoopSize(string publicKey)
{
    const int maxLoopSize = 100_000_000;
    const int subjectNumber = 7;
    
    var publicKeyAsNumber = int.Parse(publicKey);
    var value = 1;
    for (var i = 1; i <= maxLoopSize; i++)
    {
        value *= subjectNumber;
        value %= 20201227;

        if (value == publicKeyAsNumber)
        {
            return i;
        }
    }

    throw new InvalidOperationException("Could not find loop size");
}


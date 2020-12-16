using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day_08
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt");

            var list = input.Split("\r\n").ToArray();

            var one = new TaskOne(list);
            one.Run();

            var two = new TaskTwo(list);
            two.Run();

            Console.WriteLine("Finished");
        }


        private class TaskOne
        {
            private readonly Instruction[] _instructions;

            public TaskOne(string[] lines)
            {
                _instructions = lines.Select(line => new Instruction(line)).ToArray();
            }

            public void Run()
            {
                var instructionsExecuted = new HashSet<int>();

                var acc = 0;
                var instructionIndex = 0;
                var processing = true;

                while (processing) ProcessInstruction();

                Console.Out.WriteLine($"Stopped processing at acc: {acc}");

                void ProcessInstruction()
                {
                    if (instructionsExecuted.Contains(instructionIndex))
                    {
                        processing = false;
                        return;
                    }

                    instructionsExecuted.Add(instructionIndex);

                    (instructionIndex, acc) = _instructions[instructionIndex].RunInstruction(instructionIndex, acc);
                }
            }
        }

        private class TaskTwo
        {
            private readonly Instruction[] _instructions;

            public TaskTwo(string[] lines)
            {
                _instructions = lines.Select(line => new Instruction(line)).ToArray();
            }

            public void Run()
            {
                var instructionsToChange = new[] {"nop", "jmp"};

                for (var i = 0; i < _instructions.Length; i += 1)
                {
                    if (!instructionsToChange.Contains(_instructions[i].Operation)) continue;

                    if (TryRun(i, out var acc))
                    {
                        Console.WriteLine($"Ran successfully: {acc}");
                        return;
                    }
                }
            }

            private bool TryRun(int indexOfOperationToChange, out int outAcc)
            {
                var instructionsExecuted = new HashSet<int>();
                var localInstructions = new Instruction[_instructions.Length];
                _instructions.CopyTo(localInstructions, 0);

                localInstructions[indexOfOperationToChange] =
                    localInstructions[indexOfOperationToChange].SwapOperation();

                var acc = 0;
                var instructionIndex = 0;
                var processing = true;
                var terminatedSuccessfully = false;

                while (processing && !terminatedSuccessfully) ProcessInstruction();

                outAcc = acc;
                return terminatedSuccessfully;

                void ProcessInstruction()
                {
                    if (instructionsExecuted.Contains(instructionIndex))
                    {
                        processing = false;
                        return;
                    }

                    if (instructionIndex == localInstructions.Length)
                    {
                        terminatedSuccessfully = true;
                        return;
                    }

                    instructionsExecuted.Add(instructionIndex);

                    var instruction = localInstructions[instructionIndex];

                    (instructionIndex, acc) = instruction.RunInstruction(instructionIndex, acc);
                }
            }
        }

        private class Instruction
        {
            private readonly int _argument;
            public readonly string Operation;

            private Instruction(string operation, int argument)
            {
                Operation = operation;
                _argument = argument;
            }

            public Instruction(string line)
            {
                var matches = Regex.Matches(line, @"\w+");
                var operation = matches[0].Value;
                var arg = int.Parse(matches[1].Value);
                var sign = line.Contains('+') ? 1 : -1;

                Operation = operation;
                _argument = arg * sign;
            }

            public Instruction SwapOperation()
            {
                return new Instruction(Operation == "jmp" ? "nop" : "jmp", _argument);
            }

            public (int instructionIndex, int acc) RunInstruction(int instructionIndex, int acc)
            {
                return Operation switch
                {
                    "nop" => (instructionIndex + 1, acc),
                    "acc" => (instructionIndex + 1, acc + _argument),
                    "jmp" => (instructionIndex + _argument, acc),
                    _ => throw new InvalidOperationException($"Unknown instruction {Operation}, {_argument}")
                };
            }
        }
    }
}
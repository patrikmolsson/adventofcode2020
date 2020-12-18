using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

var input = File.ReadAllText("input.txt");
var rows = input.Split(Environment.NewLine).ToArray();
var run = new Action<bool, string>((plusStrongerPreference, label) =>
{
    var stopwatch = Stopwatch.StartNew();

    var expressions = new List<ExpressionPart>();
    foreach (var row in rows.Select(r => Regex.Replace(r, @"\s+", "")))
    {
        var chars = row.ToCharArray();
        var pointer = -1;

        expressions.Add(GetNestedExpression());

        ExpressionPart GetNestedExpression()
        {
            var operators = new List<char>();
            var terms = new List<Part>();
            pointer += 1;

            while (pointer < chars.Length)
            {
                var op = chars[pointer];

                switch (op)
                {
                    case '+':
                    case '*':
                        operators.Add(op);
                        break;
                    case '(':
                        terms.Add(GetNestedExpression());
                        break;
                    case ')':
                        return new ExpressionPart(terms, operators, plusStrongerPreference);
                    default:
                        terms.Add(new NumberPart(int.Parse(op.ToString())));
                        break;
                }

                pointer += 1;
            }

            return new ExpressionPart(terms, operators, plusStrongerPreference);
        }
    }

    var sum = expressions.Aggregate(0L, (l, expression) => l + expression.Evaluate());
    stopwatch.Stop();

    Console.WriteLine($"[{label}] Sum: {sum}");
    Console.WriteLine($"[{label}] Time: {stopwatch.Elapsed}");
    Console.WriteLine();
});
var taskOne = new Action(() => run(false, "One"));
var taskTwo = new Action(() => run(true, "Two"));
taskOne();
taskTwo();

internal abstract record Part
{
    public abstract long Evaluate();
}

internal record NumberPart(long Number) : Part
{
    public override long Evaluate()
    {
        return Number;
    }
}

internal record ExpressionPart(IList<Part> Parts, IList<char> Operators, bool PlusStrongerPreference) : Part
{
    public override long Evaluate()
    {
        if (PlusStrongerPreference)
            EvaluatePluses();

        var value = Parts[0].Evaluate();

        for (var i = 1; i < Parts.Count; i += 1) value = NewValue(Operators[i - 1], Parts[i]);

        return value;

        long NewValue(char mOperator, Part nextTerm)
        {
            return mOperator switch
            {
                '+' => value + nextTerm.Evaluate(),
                '*' => value * nextTerm.Evaluate(),
                _ => throw new ArgumentOutOfRangeException(nameof(mOperator), mOperator, null)
            };
        }

        void EvaluatePluses()
        {
            while (Operators.Contains('+'))
            {
                var idx = Operators.IndexOf('+');

                Parts[idx] = new NumberPart(Parts[idx].Evaluate() + Parts[idx + 1].Evaluate());

                Parts.RemoveAt(idx + 1);
                Operators.RemoveAt(idx);
            }
        }
    }
}
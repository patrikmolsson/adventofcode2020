using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;


var input = File.ReadAllText("input.txt");

var rows = input.Split(Environment.NewLine).ToArray();

var expressions = new List<Expression>();

foreach (var row in rows.Select(r => Regex.Replace(r, @"\s+", "")))
{
    var chars = row.ToCharArray();
    var pointer = -1;

    expressions.Add(GetNestedExpression());

    Expression GetNestedExpression()
    {
        var operators = new List<char>();
        var terms = new List<Term>();

        while (pointer < chars.Length - 1)
        {
            pointer += 1;

            switch (chars[pointer])
            {
                case '(':
                    terms.Add(GetNestedExpression());
                    break;
                case ')':
                    return new Expression(terms.ToArray(), operators.ToArray());
                case '+':
                case '*':
                    operators.Add(chars[pointer]);
                    break;
                default:
                    terms.Add(new NumberTerm(int.Parse(chars[pointer].ToString())));
                    break;
            }
        }
        
        return new Expression(terms.ToArray(), operators.ToArray());
    }
}

var sum = expressions.Aggregate(0L, (l, expression) => l + expression.Evaluate());

Console.WriteLine($"{sum}");

internal abstract record Term
{
    public abstract long Evaluate();
}

record NumberTerm(int Number): Term
{
    public override long Evaluate() => Number;
}

record Expression(Term[] Terms, char[] Operators): Term
{
    public override long Evaluate()
    {
        var value = Terms[0].Evaluate();
        
        for (var i = 1; i < Terms.Length; i += 1)
        {
            value = NewValue(Operators[i - 1], Terms[i]);
        }

        return value;

        long NewValue(char mOperator, Term nextTerm)
        {
            return mOperator switch
            {
                '+' => value + nextTerm.Evaluate(),
                '*' => value * nextTerm.Evaluate(),
                _ => throw new ArgumentOutOfRangeException(nameof(mOperator), mOperator, null)
            };
        }
    }
}





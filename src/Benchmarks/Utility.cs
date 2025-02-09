using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmarks;

internal static class Utility
{
    internal static string CreateCalculationCsv(int count) 
        => CalculationWriter.WritePerformance(CreateCalculations(count));

    internal static Calculation[] CreateCalculations(int count)
    {
        var calcs = new Calculation[count];
        ReadOnlySpan<char> operators = "+-*/%".AsSpan();
        var rand = new Random(42);

        Span<Calculation> span = calcs.AsSpan();

        for (int i = 0; i < span.Length; i++)
        {
            char op = operators[rand.Next(0, operators.Length)];
            int first = rand.Next(-10_000, 10_001);
            int second = rand.Next(-10_000, 10_001);
            double result;

            switch (op)
            {
                case '+':
                    result = first + second;
                    break;
                case '-':
                    result = first - second;
                    break;
                case '*':
                    result = first * second;
                    break;
                case '/':
                    second = second == 0 ? 42 : second;
                    result = (double)first / second;
                    break;
                case '%':
                    second = second == 0 ? 42 : second;
                    result = first % second;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            span[i] = new Calculation(first, op, second, result);
        }

        return calcs;
    }
}

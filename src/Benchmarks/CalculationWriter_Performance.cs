using FolkerKinzel.CsvTools;
using FolkerKinzel.CsvTools.Mappings;
using FolkerKinzel.CsvTools.Mappings.TypeConverters;
using System.Globalization;

namespace Benchmarks;

internal static partial class CalculationWriter
{
    internal static string WritePerformance(Calculation[] data)
    {
        CultureInfo culture = CultureInfo.InvariantCulture;
        const string doubleFormat = "G17";

        using var stringWriter = new StringWriter();
        using var csvWriter = new CsvWriter(stringWriter, ["First", "Operator", "Second", "Result"]);

        ReadOnlySpan<Calculation> dataSpan = data;
        Span<ReadOnlyMemory<char>> recordSpan = csvWriter.Record.Values;

        for (int i = 0; i < dataSpan.Length; i++)
        {
            Calculation calculation = dataSpan[i];

            recordSpan[0] = calculation.First.ToString(doubleFormat, culture).AsMemory();
            recordSpan[1] = GetOperator(calculation.Operator).AsMemory();
            recordSpan[2] = calculation.Second.ToString(doubleFormat, culture).AsMemory();
            recordSpan[3] = calculation.Result.ToString(doubleFormat, culture).AsMemory();

            csvWriter.WriteRecord();
        }

        return stringWriter.ToString();

        static string GetOperator(char op)
        {
            return op switch
            {
                '+' => "+",
                '-' => "-",
                '*' => "*",
                '/' => "/",
                '%' => "%",
                _ => "",
            };
        }
    }
}

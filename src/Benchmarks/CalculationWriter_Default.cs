using FolkerKinzel.CsvTools.Mappings;
using FolkerKinzel.CsvTools.Mappings.TypeConverters;

namespace Benchmarks;

internal static partial class CalculationWriter
{
    internal static string WriteDefault(Calculation[] data)
    {
        var doubleConverter = new DoubleConverter();

        CsvRecordMapping mapping = CsvRecordMappingBuilder
            .Create()
            .AddProperty("First", doubleConverter)
            .AddProperty("Operator", new CharConverter())
            .AddProperty("Second", doubleConverter)
            .AddProperty("Result", doubleConverter)
            .Build();

        return CsvConverter.ToCsv(data, ["First", "Operator", "Second", "Result"], mapping,
            (calc, dyn) =>
            {
                dyn.First = calc.First;
                dyn.Operator = calc.Operator;
                dyn.Second = calc.Second;
                dyn.Result = calc.Result;
            });
    }
}

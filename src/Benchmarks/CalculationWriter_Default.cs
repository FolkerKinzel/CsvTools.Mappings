using FolkerKinzel.CsvTools;
using FolkerKinzel.CsvTools.Mappings;
using FolkerKinzel.CsvTools.Mappings.TypeConverters;

namespace Benchmarks;

internal static partial class CalculationWriter
{
    internal static string WriteDefault(Calculation[] data)
    {
        var doubleConverter = new DoubleConverter();

        CsvMapping mapping = CsvMappingBuilder
            .Create()
            .AddProperty("First", doubleConverter)
            .AddProperty("Operator", new CharConverter())
            .AddProperty("Second", doubleConverter)
            .AddProperty("Result", doubleConverter)
            .Build();

        return data.ToCsv(mapping, (calculation, dyn) =>
                                   {
                                       dyn.First = calculation.First;
                                       dyn.Operator = calculation.Operator;
                                       dyn.Second = calculation.Second;
                                       dyn.Result = calculation.Result;
                                   });
    }
}

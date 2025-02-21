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

        return data.ToCsv(
            mapping, 
            static (calculation, mapping) =>
                   {
                       mapping.First = calculation.First;
                       mapping.Operator = calculation.Operator;
                       mapping.Second = calculation.Second;
                       mapping.Result = calculation.Result;
                   });
    }
}

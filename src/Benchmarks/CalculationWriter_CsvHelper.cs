using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace Benchmarks;

public class CalculationMap : ClassMap<Calculation>
{
    public CalculationMap()
    {
        Map(c => c.First).Index(0).Name("First");
        Map(c => c.Operator).Index(1).Name("Operator");
        Map(c => c.Second).Index(2).Name("Second");
        Map(c => c.Result).Index(3).Name("Result");
    }
}

internal static partial class CalculationWriter
{
    internal static string WriteCsvHelper(Calculation[] data)
    {
        using var writer = new StringWriter();
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap<CalculationMap>();
        csv.WriteRecords(data);
        csv.Flush();
        return writer.ToString();
    }
}

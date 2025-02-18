using CsvHelper;
using System.Globalization;

namespace Benchmarks;

internal static partial class CalculationReader
{
    internal static IList<Calculation> ReadCsvHelper(string csv)
    {
        var list = new List<Calculation>();

        using var reader = new StringReader(csv);
        using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

        csvReader.Context.RegisterClassMap<CalculationMap>();
        CultureInfo culture = CultureInfo.InvariantCulture;

        foreach(dynamic dyn  in csvReader.GetRecords<dynamic>())
        {
            double first = double.Parse(dyn.First, culture);
            char op = dyn.Operator[0];
            double second = double.Parse(dyn.Second, culture);
            double result = double.Parse(dyn.Result, culture);

            list.Add(new(first, op, second, result));
        }

        return list;
    }
}
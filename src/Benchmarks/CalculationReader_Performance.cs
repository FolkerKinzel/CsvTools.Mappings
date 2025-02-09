using FolkerKinzel.CsvTools;
using FolkerKinzel.CsvTools.Mappings;
using FolkerKinzel.CsvTools.Mappings.TypeConverters;
using System.Buffers;
using System.Globalization;

namespace Benchmarks;

internal static partial class CalculationReader
{
    internal static IList<Calculation> ReadPerformance(string csv)
    {
        var doubleConverter = new DoubleConverter(styles: NumberStyles.Integer | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint);
        CsvRecordMapping mapping = CsvRecordMappingBuilder
            .Create()
            .AddProperty("First", doubleConverter)
            .AddProperty("Operator", new CharConverter())
            .AddProperty("Second", doubleConverter)
            .AddProperty("Result", doubleConverter)
            .Build();

        using var stringReader = new StringReader(csv);
        using var csvReader = new CsvReader(stringReader, options: CsvOpts.Default | CsvOpts.DisableCaching);

        var list = new List<Calculation>(64);

        ITypedProperty<double> first = mapping[0].AsITypedProperty<double>();
        ITypedProperty<char> op = mapping[1].AsITypedProperty<char>();
        ITypedProperty<double> second = mapping[2].AsITypedProperty<double>();
        ITypedProperty<double> result = mapping[3].AsITypedProperty<double>();

        CsvRecord? record;
        while((record = csvReader.Read()) != null)
        {
            mapping.Record = record;
            list.Add(new Calculation(first.Value, op.Value, second.Value, result.Value));
        }

        return list;
    }
}
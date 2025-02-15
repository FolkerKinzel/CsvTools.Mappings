using FolkerKinzel.CsvTools;
using FolkerKinzel.CsvTools.Mappings;
using FolkerKinzel.CsvTools.Mappings.TypeConverters;
using System.Globalization;

namespace Benchmarks;

internal static partial class CalculationReader
{
    internal static IList<Calculation> ReadPerformance(string csv)
    {
        // Strict parsers are faster but less flexible:
        var doubleConverter = new DoubleConverter(styles: NumberStyles.Integer
                                                        | NumberStyles.AllowLeadingSign
                                                        | NumberStyles.AllowDecimalPoint);

        CsvMapping mapping = CsvMappingBuilder
            .Create()
            .AddProperty("First", doubleConverter)
            .AddProperty("Operator", new CharConverter())
            .AddProperty("Second", doubleConverter)
            .AddProperty("Result", doubleConverter)
            .Build();

        using var stringReader = new StringReader(csv);
        using var csvReader = new CsvReader(stringReader, options: CsvOpts.Default
        /* Using the DisableCaching option avoids cloning the */ | CsvOpts.DisableCaching);
        /* the CsvRecord instance with each Read().           */

        var list = new List<Calculation>(64); // (This data storage allows further optimization.)

        // Using ITypedProperty<T> instances avoids boxing and unboxing of value types.
        // The AsITypedProperty<T>() extension method is "syntactic sugar" that can be
        // replaced with a simple cast to save a few nanoseconds.
        ITypedProperty<double> first = mapping[0].AsITypedProperty<double>();
        ITypedProperty<char> op = mapping[1].AsITypedProperty<char>();
        ITypedProperty<double> second = mapping[2].AsITypedProperty<double>();
        ITypedProperty<double> result = mapping[3].AsITypedProperty<double>();

        CsvRecord? record;

        while ((record = csvReader.Read()) != null)
        {
            mapping.Record = record;
            list.Add(new Calculation(first.Value, op.Value, second.Value, result.Value));
        }

        return list;
    }
}
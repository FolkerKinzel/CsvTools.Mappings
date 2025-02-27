﻿using FolkerKinzel.CsvTools.Mappings;
using FolkerKinzel.CsvTools.Mappings.TypeConverters;

namespace Benchmarks;

internal static partial class CalculationReader
{
    internal static IList<Calculation> ReadDefault(string csv)
    {
        var doubleConverter = new DoubleConverter();

        CsvMapping mapping = CsvMappingBuilder
            .Create()
            .AddProperty("First", doubleConverter)
            .AddProperty("Operator", new CharConverter())
            .AddProperty("Second", doubleConverter)
            .AddProperty("Result", doubleConverter)
            .Build();

        return CsvConverter.Parse(
            csv,
            mapping,
            static mapping => new Calculation(mapping.First,
                                              mapping.Operator,
                                              mapping.Second,
                                              mapping.Result));
    }
}
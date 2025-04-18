﻿using FolkerKinzel.CsvTools.Mappings.TypeConverters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Mappings.Intls.DynamicProperties.Tests;

[TestClass]
public class ColumnNamePropertyTests
{
    [TestMethod]
    public void NoTargetTest1()
    {
        TypeConverter<int?> conv = new Int32Converter().ToNullableConverter();

        CsvMapping mapping = CsvMappingBuilder
            .Create()
            .AddProperty("NotInCsv", conv)
            .Build();

        int?[] results = CsvConverter.Parse<int?>("""
            A
            42
            """, mapping, dyn => dyn.NotInCsv);
        Assert.AreEqual(1, results.Length);
        Assert.IsFalse(results[0].HasValue);
    }

    [TestMethod]
    public void NoTargetTest2()
    {
        TypeConverter<int?> conv = new Int32Converter().ToNullableConverter();

        CsvMapping mapping = CsvMappingBuilder
            .Create()
            .AddProperty("NotInCsv", ["NotInCsv*"], conv)
            .Build();

        int?[] results = CsvConverter.Parse<int?>("""
            A
            42
            """, mapping, dyn => dyn.NotInCsv);
        Assert.AreEqual(1, results.Length);
        Assert.IsFalse(results[0].HasValue);
    }
}

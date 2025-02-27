﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace FolkerKinzel.CsvTools.Mappings.TypeConverters.Tests;

[TestClass]
public class DecimalConverterTests
{
    private readonly DecimalConverter _conv = new();

    [DataTestMethod]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow("bla")]
    public void TryParseValueTest2(string input)
    {
        Assert.IsFalse(_conv.AcceptsNull);
        Assert.IsFalse(_conv.TryParse(input.AsSpan(), out _));
    }

    [DataTestMethod]
    [DataRow(1.5)]
    [DataRow(-3.8)]
    public void ConvertToStringTest1(Double input)
    {
        Assert.IsFalse(_conv.AcceptsNull);
        Assert.IsNotNull(_conv.ConvertToString((Decimal)input));
    }

    [DataTestMethod]
    [DataRow("D")]
    [DataRow("R")]
    [DataRow("X")]
    [ExpectedException(typeof(ArgumentException))]
    public void ValidateFormatTest(string? format) => new DecimalConverter(format: format);
}

﻿namespace FolkerKinzel.CsvTools.Mappings.TypeConverters.Tests;

[TestClass]
public class DoubleConverterTests
{
    private readonly DoubleConverter _conv = new();

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
        Assert.IsNotNull(_conv.ConvertToString(input));
    }

    [TestMethod]
    public void RoundtripTest1()
    {
        double d = 72.81;

        var conv = new DoubleConverter();

        string? s = conv.ConvertToString(d);
        Assert.IsNotNull(s);

        double? d2 = (double?)conv.Parse(s.AsSpan());
        Assert.AreEqual(d, d2);
    }

    [DataTestMethod]
    [DataRow("D")]
    //[DataRow("R")]
    [DataRow("X")]
    [ExpectedException(typeof(ArgumentException))]
    public void ValidateFormatTest1(string? format) => _ = new DoubleConverter(format: format);

    [DataTestMethod]
    [DataRow("DD")]
    [DataRow("XX")]
    public void ValidateFormatTest2(string? format) => _ = new DoubleConverter(format: format);
}

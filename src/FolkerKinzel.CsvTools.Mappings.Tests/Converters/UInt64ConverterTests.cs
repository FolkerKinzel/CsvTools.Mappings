﻿namespace FolkerKinzel.CsvTools.Mappings.Converters.Tests;

[TestClass]
public class UInt64ConverterTests
{
    private readonly UInt64Converter _conv = new();

    [DataTestMethod]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow("bla")]
    public void TryParseValueTest2(string input)
    {
        Assert.IsFalse(_conv.AllowsNull);
        Assert.IsFalse(_conv.TryParseValue(input.AsSpan(), out _));
    }

    [TestMethod]
    public void ConvertToStringTest1()
    {
        Assert.IsFalse(_conv.AllowsNull);
        Assert.IsNotNull(_conv.ConvertToString(42));
    }

    [TestMethod]
    public void ToHexConverterTest()
    {
        var conv = new UInt64Converter().ToHexConverter();
        Assert.AreEqual("2A", conv.ConvertToString(42));
        Assert.AreEqual(42u, conv.Parse("2A".AsSpan()));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void ValidateFormatTest() => new UInt64Converter(format: "R");
}

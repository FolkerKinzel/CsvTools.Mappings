﻿namespace FolkerKinzel.CsvTools.Mappings.TypeConverters.Tests;

[TestClass]
public class UInt32ConverterTests
{
    private readonly UInt32Converter _conv = new();

    [DataTestMethod]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow("bla")]
    public void TryParseValueTest2(string input)
    {
        Assert.IsFalse(_conv.AcceptsNull);
        Assert.IsFalse(_conv.TryParse(input.AsSpan(), out _));
    }

    [TestMethod]
    public void ConvertToStringTest1()
    {
        Assert.IsFalse(_conv.AcceptsNull);
        Assert.IsNotNull(_conv.ConvertToString(42));
    }

    [TestMethod]
    public void ToHexConverterTest()
    {
        TypeConverter<uint> conv = new UInt32Converter().ToHexConverter();
        Assert.AreEqual("2A", conv.ConvertToString(42));
        Assert.AreEqual(42u, conv.Parse("2A".AsSpan()));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void ValidateFormatTest() => new UInt32Converter(format: "R");
}

﻿namespace FolkerKinzel.CsvTools.Mappings.Converters.Tests;

[TestClass]
public class CharConverterTests
{
    private readonly CharConverter _conv = new();

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
        Assert.IsNotNull(_conv.ConvertToString('a'));
    }

    [TestMethod]
    public void RoundtripTest1()
    {
        const char a = 'a';
        string? csv = _conv.ConvertToString(a);
        Assert.AreEqual(a, _conv.Parse(csv.AsSpan()));
    }

    [TestMethod]
    public void ToHexConverterTest1()
    {
        var conv = new ByteConverter().ToHexConverter();
        Assert.AreEqual("2A", conv.ConvertToString(42));
        Assert.AreEqual(42, conv.Parse("2A".AsSpan()));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void ValidateFormatTest() => new ByteConverter(format: "R");
    
}

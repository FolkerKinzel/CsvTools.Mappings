﻿namespace FolkerKinzel.CsvTools.Mappings.Converters.Tests;

[TestClass]
public class ByteConverterTests
{
    private readonly ByteConverter _conv = new();

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
}

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
}

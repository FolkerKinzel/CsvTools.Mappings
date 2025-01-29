﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Mappings.Converters.Tests;

[TestClass]
public class Int16ConverterTests
{
    private readonly Int16Converter _conv = new();

    [DataTestMethod]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow("bla")]
    public void TryParseValueTest2(string input)
    {
        Assert.IsFalse(_conv.AllowsNull);
        Assert.IsFalse(_conv.TryParseValue(input.AsSpan(), out _));
    }

    [DataTestMethod]
    [DataRow(4)]
    [DataRow(-3)]
    public void ConvertToStringTest1(int input)
    {
        Assert.IsFalse(_conv.AllowsNull);
        Assert.IsNotNull(_conv.ConvertToString((short)input));
    }
}
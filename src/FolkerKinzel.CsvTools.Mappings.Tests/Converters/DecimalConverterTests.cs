using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace FolkerKinzel.CsvTools.Mappings.Converters.Tests;

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
        Assert.IsFalse(_conv.AllowsNull);
        Assert.IsFalse(_conv.TryParseValue(input.AsSpan(), out _));
    }

    [DataTestMethod]
    [DataRow(1.5)]
    [DataRow(-3.8)]
    public void ConvertToStringTest1(Double input)
    {
        Assert.IsFalse(_conv.AllowsNull);
        Assert.IsNotNull(_conv.ConvertToString((Decimal)input));
    }
}

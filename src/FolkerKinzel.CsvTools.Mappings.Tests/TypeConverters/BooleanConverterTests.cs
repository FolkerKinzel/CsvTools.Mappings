using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace FolkerKinzel.CsvTools.Mappings.TypeConverters.Tests;

[TestClass]
public class BooleanConverterTests
{
    private readonly BooleanConverter _conv = new();

    [TestMethod]
    public void TryParseValueTest1()
    {
        string blString = true.ToString(CultureInfo.CreateSpecificCulture("de-DE"));
        Assert.IsTrue(_conv.TryParseValue(blString.AsSpan(), out bool result));
        Assert.IsTrue(result);
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow("bla")]
    public void TryParseValueTest2(string input)
    {
        Assert.IsFalse(_conv.AcceptsNull);
        Assert.IsFalse(_conv.TryParseValue(input.AsSpan(), out _));
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void ConvertToStringTest1(bool input)
    {
        Assert.IsFalse(_conv.AcceptsNull);
        Assert.IsNotNull(_conv.ConvertToString(input));
    }
}

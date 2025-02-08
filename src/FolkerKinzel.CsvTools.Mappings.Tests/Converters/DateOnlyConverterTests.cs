using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace FolkerKinzel.CsvTools.Mappings.TypeConverters.Tests;

[TestClass()]
public class DateOnlyConverterTests
{
    private readonly DateOnlyConverter _conv = new();

    [TestMethod()]
    public void DateOnlyConverterTest1()
    {
        Assert.IsNotNull(_conv);
        Assert.IsFalse(_conv.AcceptsNull);
        Assert.IsFalse(_conv.ParseExact);
        Assert.AreEqual(_conv.FormatProvider, CultureInfo.InvariantCulture);
        Assert.AreEqual("d", _conv.Format);
        Assert.AreEqual(DateTimeStyles.AllowWhiteSpaces, _conv.Styles);
    }

    [TestMethod]
    public void DateOnlyConverterTest2() => _ = new DateOnlyConverter(format: null);

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void FormatNullTest1() => new DateOnlyConverter(format: null, parseExact: true);

    [TestMethod]
    public void ParseExactTest1()
    {
        var conv = new DateOnlyConverter(parseExact: true);
        Assert.IsTrue(conv.ParseExact);
        var dateOnly = new DateOnly(2005, 12, 24);
        string? csv = conv.ConvertToString(dateOnly);
        Assert.IsNotNull(csv);
        Assert.IsTrue(conv.TryParseValue(csv.AsSpan(), out DateOnly parsed));
        Assert.AreEqual(dateOnly, parsed);
    }

    [TestMethod]
    public void ParseTest1()
    {
        Assert.IsFalse(_conv.ParseExact);
        var dateOnly = new DateOnly(2005, 12, 24);
        string? csv = _conv.ConvertToString(dateOnly);
        Assert.IsNotNull(csv);
        Assert.IsTrue(_conv.TryParseValue(csv.AsSpan(), out DateOnly parsed));
        Assert.AreEqual(dateOnly, parsed);
    }


}

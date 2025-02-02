using System.Globalization;

namespace FolkerKinzel.CsvTools.Mappings.Converters.Tests;

[TestClass()]
public class TimeOnlyConverterTests
{
    private readonly TimeOnlyConverter _conv = new();

    [TestMethod()]
    public void TimeOnlyConverterTest1()
    {
        Assert.IsNotNull(_conv);
        Assert.IsFalse(_conv.AcceptsNull);
        Assert.IsFalse(_conv.ParseExact);
        Assert.AreEqual(_conv.FormatProvider, CultureInfo.InvariantCulture);
        Assert.AreEqual("T", _conv.Format);
        Assert.AreEqual(DateTimeStyles.AllowWhiteSpaces, _conv.Styles);
    }

    [TestMethod]
    public void TimeOnlyConverterTest2() => _ = new TimeOnlyConverter(format: null);

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void TimeOnlyConverterTest3() => _ = new TimeOnlyConverter(styles: DateTimeStyles.AdjustToUniversal);

    [TestMethod]
    public void FormatNullTest1() => _ = new TimeOnlyConverter(format: null, parseExact: true);

    [TestMethod]
    public void ParseExactTest1()
    {
        var conv = new TimeOnlyConverter(parseExact: true);
        Assert.IsTrue(conv.ParseExact);
        var timeOnly = new TimeOnly(14, 25, 17);
        string? csv = conv.ConvertToString(timeOnly);
        Assert.IsNotNull(csv);
        Assert.IsTrue(conv.TryParseValue(csv.AsSpan(), out TimeOnly parsed));
        Assert.AreEqual(timeOnly, parsed);
    }

    [TestMethod]
    public void ParseTest1()
    {
        Assert.IsFalse(_conv.ParseExact);
        var timeOnly = new TimeOnly(14, 25, 17);
        string? csv = _conv.ConvertToString(timeOnly);
        Assert.IsNotNull(csv);
        Assert.IsTrue(_conv.TryParseValue(csv.AsSpan(), out TimeOnly parsed));
        Assert.AreEqual(timeOnly, parsed);
    }
}
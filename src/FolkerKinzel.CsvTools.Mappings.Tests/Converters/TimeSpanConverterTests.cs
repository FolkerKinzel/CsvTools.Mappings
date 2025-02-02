using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Mappings.Converters.Tests;

[TestClass()]
public class TimeSpanConverterTests
{
    private readonly TimeSpanConverter _conv = new ();

    [TestMethod()]
    public void TimeSpanConverterTest1()
    {
        Assert.IsNotNull(_conv);
        Assert.IsFalse(_conv.AcceptsNull);
        Assert.IsFalse(_conv.ParseExact);
        Assert.AreEqual(_conv.FormatProvider, CultureInfo.InvariantCulture);
        Assert.AreEqual("g", _conv.Format);
        Assert.AreEqual(TimeSpanStyles.None, _conv.Styles);
    }

    [TestMethod()]
    public void TimeSpanConverterTest2()
    {
        var conv = new TimeSpanConverter(format: "");
        Assert.IsNotNull(conv);
    }

    [TestMethod()]
    public void TimeSpanConverterTest5()
    {
        var conv = new TimeSpanConverter(format: "G");
        Assert.IsNotNull(conv);
        Assert.AreEqual("G", conv.Format);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TimeSpanConverterTest6() => _ = new TimeSpanConverter(format: null!, parseExact: true);

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void TimeSpanConverterTest7() => _ = new TimeSpanConverter(parseExact: true, styles: (TimeSpanStyles)4711);

    [TestMethod]
    public void TimeSpanConverterTest8() => _ = new TimeSpanConverter(parseExact: true, styles: TimeSpanStyles.AssumeNegative);

    [TestMethod()]
    public void Roundtrip1()
    {
        TimeSpan now = DateTime.UtcNow.TimeOfDay;

        string? tmp = _conv.ConvertToString(now);

        Assert.IsNotNull(tmp);

        TimeSpan now2 = _conv.Parse(tmp.AsSpan());

        Assert.AreEqual(now, now2);
    }

    [TestMethod()]
    public void Roundtrip2()
    {
        TimeSpan now = DateTime.UtcNow.TimeOfDay;

        var conv = new TimeSpanConverter(format: "G");

        string? tmp = conv.ConvertToString(now);

        Assert.IsNotNull(tmp);

        TimeSpan now2 = conv.Parse(tmp.AsSpan());

        Assert.AreEqual(now, now2);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void FormatNullTest1() => new TimeSpanConverter(format: null, parseExact: true);

    [TestMethod]
    public void ParseExactTest1()
    {
        var conv = new TimeSpanConverter(parseExact: true);
        Assert.IsTrue(conv.ParseExact);
    }
}
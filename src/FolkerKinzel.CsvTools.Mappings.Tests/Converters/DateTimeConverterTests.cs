using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FolkerKinzel.CsvTools.Mappings.Converters.Tests;

[TestClass()]
public class DateTimeConverterTests
{
    private readonly DateTimeConverter _conv = new();

    [TestMethod()]
    public void DateTimeConverterTest1()
    {
        Assert.IsNotNull(_conv);
        Assert.IsFalse(_conv.AcceptsNull);
        Assert.IsFalse(_conv.ParseExact);
    }

    [TestMethod]
    public void DateTimeConverterTest7() => _ = new DateTimeConverter(format: null);

    [TestMethod()]
    public void Roundtrip1()
    {
        var now = new DateTime(2021, 3, 1, 17, 25, 38, DateTimeKind.Unspecified);

        string? tmp = _conv.ConvertToString(now);

        Assert.IsNotNull(tmp);

        var now2 = (DateTime?)_conv.Parse(tmp.AsSpan());

        Assert.AreEqual(now, now2);
    }

    [TestMethod()]
    public void Roundtrip2()
    {
        DateTime now = DateTime.UtcNow;

        now = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, DateTimeKind.Utc);

        var conv = new DateTimeConverter(format: "F");

        string? tmp = conv.ConvertToString(now);

        Assert.IsNotNull(tmp);

        var now2 = (DateTime?)conv.Parse(tmp.AsSpan());

        Assert.AreEqual(now, now2);
    }

    [DataTestMethod()]
    [DataRow("1974-02-16")]
    [DataRow("1974/02/16")]
    public void ParseTest(string s)
    {
        var conv = new DateTimeConverter();

        object? dt = conv.Parse(s.AsSpan());

        Assert.AreEqual(new DateTime(1974, 02, 16), dt);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void FormatNullTest1() => new DateTimeConverter(format: null, parseExact: true);

    [TestMethod]
    public void ParseExactTest1()
    {
        var conv = new DateTimeConverter(parseExact: true);
        Assert.IsTrue(conv.ParseExact);
        DateTime dateTime = DateTime.Now;
        string? csv = conv.ConvertToString(dateTime);
        Assert.IsNotNull(csv);
        Assert.IsTrue(conv.TryParseValue(csv.AsSpan(), out DateTime parsed));
        CultureInfo culture = CultureInfo.InvariantCulture;
        Assert.AreEqual(dateTime.ToString(culture), parsed.ToString(culture));
    }

    [TestMethod]
    public void ParseTest1()
    {
        Assert.IsFalse(_conv.ParseExact);
        DateTime dateTime = DateTime.Now;
        string? csv = _conv.ConvertToString(dateTime);
        Assert.IsNotNull(csv);
        Assert.IsTrue(_conv.TryParseValue(csv.AsSpan(), out DateTime parsed));
        CultureInfo culture = CultureInfo.InvariantCulture;
        Assert.AreEqual(dateTime.ToString(culture), parsed.ToString(culture));
    }
}

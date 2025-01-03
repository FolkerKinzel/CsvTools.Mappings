using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace FolkerKinzel.CsvTools.Mappings.Converters.Tests;

[TestClass()]
public class DateTimeConverterTests
{
    [TestMethod()]
    public void DateTimeConverterTest1()
    {
        var conv = new DateTimeConverter();
        Assert.IsNotNull(conv);
    }

    //[TestMethod()]
    //public void DateTimeConverterTest2()
    //{
    //    var conv = DateTimeConverter.CreateDateConverter(formatProvider: CultureInfo.InvariantCulture);
    //    Assert.IsNotNull(conv);
    //}

    [TestMethod()]
    public void DateTimeConverterTest3()
    {
        var conv = new DateTimeConverter(format: "");
        Assert.IsNotNull(conv);
    }

    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    public void DateTimeConverterTest4() => _ = new DateTimeConverter(format: "");

    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    public void DateTimeConverterTest5()
        => _ = new DateTimeConverter(format: "Ä");

    [TestMethod()]
    public void DateTimeConverterTest6()
    {
        var conv = new DateTimeConverter(format: "D");
        Assert.IsNotNull(conv);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void DateTimeConverterTest7() => _ = new DateTimeConverter(format: null!);

    [TestMethod()]
    public void Roundtrip1()
    {
        var now = new DateTime(2021, 3, 1, 17, 25, 38, DateTimeKind.Unspecified);

        var conv = new DateTimeConverter();

        string? tmp = conv.ConvertToString(now);

        Assert.IsNotNull(tmp);

        var now2 = (DateTime?)conv.Parse(tmp.AsSpan());


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

    //[TestMethod()]
    //public void ConvertToStringTest1()
    //{
    //    var dt = new DateTime(1985, 11, 17);
    //    var conv = DateTimeConverter.CreateDateConverter(formatProvider: CultureInfo.InvariantCulture);

    //    //dt = dt.ToLocalTime();

    //    string? s = conv.ConvertToString(dt);

    //    Assert.IsNotNull(s);
    //}

    [TestMethod()]
    public void ConvertToStringTest2()
    {
        var dt = new DateTime(2001, 03, 31);
        var conv = new DateTimeConverter();

        //dt = dt.ToLocalTime();

        string? s = conv.ConvertToString(dt);

        Assert.IsNotNull(s);
    }
}
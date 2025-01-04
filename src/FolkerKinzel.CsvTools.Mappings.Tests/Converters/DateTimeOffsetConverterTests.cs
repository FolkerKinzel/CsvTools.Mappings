using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;

namespace FolkerKinzel.CsvTools.Mappings.Converters.Tests;

[TestClass()]
public class DateTimeOffsetConverterTests
{
    [TestMethod()]
    public void DateTimeOffsetConverterTest0()
    {
        var conv = new DateTimeOffsetConverter();

        Assert.IsNotNull(conv);

        var dt = new DateTime(1975, 07, 14);

        string? s = conv.ConvertToString(new DateTimeOffset(dt));

        Assert.IsNotNull(s);

        var dto = (DateTimeOffset)conv.Parse(s.AsSpan())!;

        Assert.AreEqual(dt, dto.DateTime);
    }


    [TestMethod()]
    public void DateTimeOffsetConverterTest3()
    {
        var conv = new DateTimeOffsetConverter(format: "");
        Assert.IsNotNull(conv);
    }

    //[TestMethod()]
    //[ExpectedException(typeof(ArgumentException))]
    //public void DateTimeOffsetConverterTest4() => _ = new DateTimeOffsetConverter(format: "", parseExact: true);

    //[TestMethod()]
    //[ExpectedException(typeof(ArgumentException))]
    //public void DateTimeOffsetConverterTest5()
    //    => _ = new DateTimeOffsetConverter(format: "Ä");

    [TestMethod()]
    public void DateTimeOffsetConverterTest6()
    {
        var conv = new DateTimeOffsetConverter(format: "D");
        Assert.IsNotNull(conv);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void DateTimeOffsetConverterTest7() => _ = new DateTimeOffsetConverter(format: null!, parseExact: true);


    [TestMethod()]
    public void Roundtrip1()
    {
        DateTimeOffset now = new DateTime(2021, 3, 1, 17, 25, 38, DateTimeKind.Local);

        var conv = new DateTimeOffsetConverter();

        string? tmp = conv.ConvertToString(now);

        Assert.IsNotNull(tmp);

        var now2 = (DateTimeOffset?)conv.Parse(tmp.AsSpan());


        Assert.AreEqual(now, now2);
    }


    [TestMethod()]
    public void Roundtrip2()
    {
        DateTime tmp = DateTime.UtcNow;
        DateTimeOffset now = new DateTime(tmp.Year, tmp.Month, tmp.Day, tmp.Hour, tmp.Minute, tmp.Second, DateTimeKind.Utc);

        var conv = new DateTimeOffsetConverter(format: "R");

        string? tmp1 = conv.ConvertToString(now);

        Assert.IsNotNull(tmp1);

        var now2 = (DateTimeOffset?)conv.Parse(tmp1.AsSpan());

        Assert.AreEqual(now, now2);
    }
}
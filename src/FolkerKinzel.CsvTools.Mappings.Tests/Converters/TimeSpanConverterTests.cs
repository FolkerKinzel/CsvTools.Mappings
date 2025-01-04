using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Mappings.Converters.Tests;

[TestClass()]
public class TimeSpanConverterTests
{
    [TestMethod()]
    public void TimeSpanConverterTest1()
    {
        var conv = new TimeSpanConverter();
        Assert.IsNotNull(conv);
    }

    [TestMethod()]
    public void TimeSpanConverterTest2()
    {
        var conv = new TimeSpanConverter(format: "");
        Assert.IsNotNull(conv);
    }

    //[TestMethod()]
    //[ExpectedException(typeof(ArgumentException))]
    //public void TimeSpanConverterTest3() => _ = new TimeSpanConverter(format: "", parseExact: true);

    //[TestMethod()]
    //[ExpectedException(typeof(ArgumentException))]
    //public void TimeSpanConverterTest4() => _ = new TimeSpanConverter(format: "bla");

    [TestMethod()]
    public void TimeSpanConverterTest5()
    {
        var conv = new TimeSpanConverter(format: "G");
        Assert.IsNotNull(conv);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TimeSpanConverterTest6() => _ = new TimeSpanConverter(format: null!, parseExact: true);

    [TestMethod()]
    public void Roundtrip1()
    {
        TimeSpan now = DateTime.UtcNow.TimeOfDay;

        var conv = new TimeSpanConverter();

        string? tmp = conv.ConvertToString(now);

        Assert.IsNotNull(tmp);

        TimeSpan now2 = conv.Parse(tmp.AsSpan());

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
}
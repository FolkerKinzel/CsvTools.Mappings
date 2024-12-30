using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters.Tests;

[TestClass()]
public class GuidConverterTests
{
    [TestMethod()]
    public void GuidConverterTest1()
    {
        var conv = new GuidConverter();
        Assert.IsNotNull(conv);
    }

    [DataTestMethod()]
    [DataRow(null)]
    [DataRow("")]
    public void GuidConverterTest2(string? format)
    {
        var conv = new GuidConverter(format);
        Assert.IsNotNull(conv);
    }

    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    public void GuidConverterTest4() => _ = new GuidConverter("bla");


    [TestMethod()]
    public void GuidConverterTest5()
    {
        var conv = new GuidConverter("B");
        Assert.IsNotNull(conv);
    }

    [TestMethod()]
    public void Roundtrip1()
    {
        var guid = Guid.NewGuid();

        var conv = new GuidConverter();

        string? tmp = conv.ConvertToString(guid);

        Assert.IsNotNull(tmp);

        var now2 = (Guid?)conv.Parse(tmp.AsSpan());

        Assert.AreEqual(guid, now2);
    }

    [TestMethod()]
    public void Roundtrip2()
    {
        var guid = Guid.NewGuid();

        var conv = new GuidConverter("B");

        string? tmp = conv.ConvertToString(guid);

        Assert.IsNotNull(tmp);

        var now2 = (Guid?)conv.Parse(tmp.AsSpan());

        Assert.AreEqual(guid, now2);
    }
}
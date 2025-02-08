using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Mappings.TypeConverters.Tests;

[TestClass()]
public class GuidConverterTests
{
    private readonly GuidConverter _conv = new();

    [TestMethod()]
    public void GuidConverterTest1()
    {
        Assert.IsNotNull(_conv);
        Assert.IsFalse(_conv.AcceptsNull);
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

        string? tmp = _conv.ConvertToString(guid);

        Assert.IsNotNull(tmp);

        var now2 = (Guid?)_conv.Parse(tmp.AsSpan());

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
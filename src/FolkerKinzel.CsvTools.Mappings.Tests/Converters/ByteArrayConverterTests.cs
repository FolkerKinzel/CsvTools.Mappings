using System;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Mappings.Converters.Tests;

[TestClass()]
public class ByteArrayConverterTests
{
    [TestMethod()]
    public void Base64ConverterTest1()
    {
        var conv = new ByteArrayConverter();
        Assert.IsNotNull(conv);
    }

    [TestMethod]
    public void ParseTest1()
    {
        var conv = new ByteArrayConverter();
        Assert.IsNotNull(conv);

        Assert.IsNull(conv.Parse(null));
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public void ParseTest2()
    {
        var conv = new ByteArrayConverter();
        Assert.IsNotNull(conv);

        Assert.IsNull(conv.Parse(default));
        _ = conv.Parse("blabla".AsSpan());
    }

    [TestMethod]
    public void ParseTest3()
    {
        var conv = new ByteArrayConverter(false);
        Assert.IsNotNull(conv);

        Assert.IsNull(conv.Parse(default));
        Assert.IsNull(conv.Parse("blabla".AsSpan()));
    }

    [TestMethod]
    public void RoundtripTest1()
    {
        byte[] bytes = new byte[7];
        using var rnd = RandomNumberGenerator.Create();
        rnd.GetBytes(bytes);

        var conv = new ByteArrayConverter();

        string? s = conv.ConvertToString(bytes);
        Assert.IsNotNull(s);

        byte[]? bytes2 = (byte[]?)conv.Parse(s.AsSpan());

        CollectionAssert.AreEqual(bytes, bytes2);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidCastException))]
    public void MyTestMethod() => new IndexProperty<byte[]?>("prop", 0, new ByteArrayConverter()).SetValue(4711);

}
using System;
using System.Security.Cryptography;
using FolkerKinzel.CsvTools.Mappings.Intls.DynamicProperties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Mappings.TypeConverters.Tests;

[TestClass()]
public class ByteArrayConverterTests
{
    [TestMethod()]
    public void Base64ConverterTest1()
    {
        TypeConverter<byte[]?> conv = ByteArrayConverter.CreateNullable();
        Assert.IsNotNull(conv);
    }

    [TestMethod]
    public void ParseTest1()
    {
        TypeConverter<byte[]?> conv = ByteArrayConverter.CreateNullable();
        Assert.IsNotNull(conv);

        Assert.IsNull(conv.Parse(null));
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public void ParseTest2()
    {
        TypeConverter<byte[]?> conv = ByteArrayConverter.CreateNullable();
        Assert.IsNotNull(conv);

        Assert.IsNull(conv.Parse(default));
        _ = conv.Parse("bl@!*abla".AsSpan());
    }

    [TestMethod]
    public void ParseTest3()
    {
        TypeConverter<byte[]?> conv = ByteArrayConverter.CreateNullable(false);
        Assert.IsNotNull(conv);

        Assert.IsNull(conv.Parse(default));
        Assert.IsNull(conv.Parse("bl@!*abla".AsSpan()));
    }

    [TestMethod]
    public void RoundtripTest1()
    {
        byte[] bytes = new byte[7];
        using var rnd = RandomNumberGenerator.Create();
        rnd.GetBytes(bytes);

        TypeConverter<byte[]?> conv = ByteArrayConverter.CreateNullable();

        string? s = conv.ConvertToString(bytes);
        Assert.IsNotNull(s);

        byte[]? bytes2 = (byte[]?)conv.Parse(s.AsSpan());

        CollectionAssert.AreEqual(bytes, bytes2);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidCastException))]
    public void InvalidCastTest() => new IndexProperty<byte[]?>("prop", 0, ByteArrayConverter.CreateNullable()).Value = 4711;

    [TestMethod]
    public void CreateNonNullableTest1()
    {
        TypeConverter<byte[]> conv = ByteArrayConverter.CreateNonNullable();
        Assert.IsNotNull(conv);
        Assert.IsInstanceOfType<byte[]>(conv.DefaultValue);
    }

    [TestMethod]
    public void CreateNullableTest2()
    {
        TypeConverter<byte[]?> conv = ByteArrayConverter.CreateNullable();
        Assert.IsNotNull(conv);
        Assert.IsNull(conv.DefaultValue);
    }

}
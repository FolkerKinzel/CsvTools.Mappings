﻿using System;
using System.Security.Cryptography;
using FolkerKinzel.CsvTools.Mappings.Intls.DynamicProperties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Mappings.TypeConverters.Tests;

[TestClass()]
public class Base64ConverterTests
{
    [TestMethod()]
    public void ByteArrayConverterTest1()
    {
        TypeConverter<byte[]?> conv = Base64Converter.CreateNullable();
        Assert.IsNotNull(conv);
        Assert.IsTrue(conv.AcceptsNull);
        Assert.IsNull(conv.Parse(null));
        Assert.IsNull(conv.Parse("   ".AsSpan()));
        Assert.IsNull(conv.ConvertToString(null));
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public void ParseTest2()
    {
        TypeConverter<byte[]?> conv = Base64Converter.CreateNullable();
        Assert.IsNotNull(conv);

        Assert.IsNull(conv.Parse(default));
        _ = conv.Parse("bl@!*abla".AsSpan());
    }

    [TestMethod]
    public void ParseTest3()
    {
        TypeConverter<byte[]?> conv = Base64Converter.CreateNullable(false);
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

        TypeConverter<byte[]?> conv = Base64Converter.CreateNullable();

        string? s = conv.ConvertToString(bytes);
        Assert.IsNotNull(s);

        byte[]? bytes2 = (byte[]?)conv.Parse(s.AsSpan());

        CollectionAssert.AreEqual(bytes, bytes2);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidCastException))]
    public void InvalidCastTest() => new IndexProperty<byte[]?>("prop", 0, Base64Converter.CreateNullable()).Value = 4711;

    [TestMethod]
    public void CreateNonNullableTest1()
    {
        TypeConverter<byte[]> conv = Base64Converter.CreateNonNullable();
        Assert.IsNotNull(conv);
        Assert.IsInstanceOfType<byte[]>(conv.DefaultValue);
        Assert.IsTrue(conv.AcceptsNull);
        Assert.IsNotNull(conv.Parse(null));
        Assert.IsNotNull(conv.Parse("   ".AsSpan()));
        Assert.IsNull(conv.ConvertToString(null!));
    }

    [TestMethod]
    public void CreateNullableTest2()
    {
        TypeConverter<byte[]?> conv = Base64Converter.CreateNullable();
        Assert.IsNotNull(conv);
        Assert.IsNull(conv.DefaultValue);
    }

}
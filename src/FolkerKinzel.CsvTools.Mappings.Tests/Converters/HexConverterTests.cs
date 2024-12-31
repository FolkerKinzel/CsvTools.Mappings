using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Mappings.Converters.Tests;

[TestClass()]
public class HexConverterTests
{
    [TestMethod()]
    public void HexConverterTest1()
    {
        TypeConverter<int> conv = new Int32Converter().ToHexConverter();
        Assert.IsNotNull(conv);
    }

    //[TestMethod()]
    //[ExpectedException(typeof(ArgumentOutOfRangeException))]
    //public void HexConverterTest2() => _ = CsvConverterFactory.CreateHexConverter(CsvTypeCode.Double);


    [TestMethod]
    public void RoundtripTest1()
    {
        int i = 123456789;

        TypeConverter<int> conv = new Int32Converter().ToHexConverter();

        string? s = conv.ConvertToString(i);
        Assert.IsNotNull(s);

        int i2 = conv.Parse(s.AsSpan());

        Assert.AreEqual(i, i2);
    }


    [TestMethod]
    public void RoundtripTest2()
    {
        int i = 123456789;

        TypeConverter<int> conv = new Int32Converter().ToHexConverter();

        string? s = conv.ConvertToString(i)?.ToLowerInvariant();
        Assert.IsNotNull(s);

        int i2 = conv.Parse(s.AsSpan());

        Assert.AreEqual(i, i2);
    }
}
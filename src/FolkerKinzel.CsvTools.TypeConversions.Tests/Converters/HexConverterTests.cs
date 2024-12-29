using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters.Intls.Tests;

[TestClass()]
public class HexConverterTests
{
    [TestMethod()]
    public void HexConverterTest1()
    {
        ICsvTypeConverter conv = new Int32Converter().AsHexConverter();
        Assert.IsNotNull(conv);
    }

    //[TestMethod()]
    //[ExpectedException(typeof(ArgumentOutOfRangeException))]
    //public void HexConverterTest2() => _ = CsvConverterFactory.CreateHexConverter(CsvTypeCode.Double);


    [TestMethod]
    public void RoundtripTest1()
    {
        int i = 123456789;

        ICsvTypeConverter conv = new Int32Converter().AsHexConverter();

        string? s = conv.ConvertToString(i);
        Assert.IsNotNull(s);

        int? i2 = (int?)conv.Parse(s.AsSpan());

        Assert.AreEqual(i, i2);

    }


    [TestMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "<Pending>")]
    public void RoundtripTest2()
    {
        int i = 123456789;

        ICsvTypeConverter conv = new Int32Converter().AsHexConverter();

        string? s = conv.ConvertToString(i)?.ToLowerInvariant();
        Assert.IsNotNull(s);

        int? i2 = (int?)conv.Parse(s.AsSpan());

        Assert.AreEqual(i, i2);
    }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace FolkerKinzel.CsvTools.Mappings.TypeConverters.Tests;

[TestClass]
public class ByteConverterTests
{
    private readonly ByteConverter _conv = new();

    [DataTestMethod]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow("bla")]
    public void TryParseValueTest2(string input)
    {
        Assert.IsFalse(_conv.AcceptsNull);
        Assert.IsFalse(_conv.TryParse(input.AsSpan(), out _));
    }

    [TestMethod]
    public void ConvertToStringTest1()
    {
        Assert.IsFalse(_conv.AcceptsNull);
        Assert.IsNotNull(_conv.ConvertToString(42));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void ValidateFormatTest() => new ByteConverter(format: "R");

    [TestMethod]
    public void ToHexConverterTest1()
    {
        var byteConv = new ByteConverter(format: "X", styles: NumberStyles.HexNumber);
        Assert.AreEqual("X", byteConv.Format);

        var hexConv1 = byteConv.ToHexConverter();
        Assert.AreSame(hexConv1, byteConv);
    }
}

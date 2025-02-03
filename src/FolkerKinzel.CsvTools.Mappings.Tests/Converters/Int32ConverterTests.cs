using FolkerKinzel.CsvTools.Mappings.Converters.Interfaces;
using System.Globalization;

namespace FolkerKinzel.CsvTools.Mappings.Converters.Tests;

[TestClass]
public class Int32ConverterTests
{
    private readonly Int32Converter _conv = new();

    [DataTestMethod]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow("bla")]
    public void TryParseValueTest2(string input)
    {
        Assert.IsFalse(_conv.AcceptsNull);
        Assert.IsFalse(_conv.TryParseValue(input.AsSpan(), out _));
    }

    [DataTestMethod]
    [DataRow(4)]
    [DataRow(-3)]
    public void ConvertToStringTest1(int input)
    {
        Assert.IsFalse(_conv.AcceptsNull);
        Assert.IsNotNull(_conv.ConvertToString(input));
    }

    //[TestMethod]
    //public void ToHexConverterTest()
    //{
    //    var conv = new Int32Converter().ToHexConverter();
    //    Assert.AreEqual("2A", conv.ConvertToString(42));
    //    Assert.AreEqual(42, conv.Parse("2A".AsSpan()));
    //}

    [TestMethod()]
    public void HexConverterTest1()
    {
        var intConverter = new Int32Converter();
        TypeConverter<int> conv = intConverter.ToHexConverter();
        Assert.IsNotNull(conv);
        Assert.AreNotSame(conv, intConverter);
        Assert.AreEqual("X", ((Int32Converter)conv).Format);
        Assert.IsTrue(((Int32Converter)conv).Styles.HasFlag(NumberStyles.AllowHexSpecifier));

        Assert.AreSame<object>(conv, ((IHexConverter<int>)conv));
    }

    [TestMethod]
    public void HexConverterTest2()
    {
        int i = 123456789;

        TypeConverter<int> conv = new Int32Converter().ToHexConverter();

        string? s = conv.ConvertToString(i);
        Assert.IsNotNull(s);

        int i2 = conv.Parse(s.AsSpan());

        Assert.AreEqual(i, i2);
    }

    [TestMethod]
    public void HexConverterTest3()
    {
        int i = 123456789;

        TypeConverter<int> conv = new Int32Converter().ToHexConverter();

        string? s = conv.ConvertToString(i)?.ToLowerInvariant();
        Assert.IsNotNull(s);

        int i2 = conv.Parse(s.AsSpan());

        Assert.AreEqual(i, i2);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void ValidateFormatTest() => new Int32Converter(format: "R");
}

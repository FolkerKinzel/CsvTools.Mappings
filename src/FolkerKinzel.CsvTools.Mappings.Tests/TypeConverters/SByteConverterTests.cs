namespace FolkerKinzel.CsvTools.Mappings.TypeConverters.Tests;

[TestClass]
public class SByteConverterTests
{
    private readonly SByteConverter _conv = new();

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
        Assert.IsNotNull(_conv.ConvertToString((sbyte)input));
    }

    [TestMethod]
    public void ToHexConverterTest()
    {
        TypeConverter<sbyte> conv = new SByteConverter().ToHexConverter();
        Assert.AreEqual("2A", conv.ConvertToString(42));
        Assert.AreEqual(42, conv.Parse("2A".AsSpan()));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void ValidateFormatTest() => new SByteConverter(format: "R");
}

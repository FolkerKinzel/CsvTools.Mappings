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

    [TestMethod]
    public void ToHexConverterTest()
    {
        var conv = new Int32Converter().ToHexConverter();
        Assert.AreEqual("2A", conv.ConvertToString(42));
        Assert.AreEqual(42, conv.Parse("2A".AsSpan()));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void ValidateFormatTest() => new Int32Converter(format: "R");
}

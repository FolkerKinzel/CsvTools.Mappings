namespace FolkerKinzel.CsvTools.Mappings.Converters.Tests;

[TestClass]
public class DoubleConverterTests
{
    private readonly DoubleConverter _conv = new();

    [DataTestMethod]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow("bla")]
    public void TryParseValueTest2(string input)
    {
        Assert.IsFalse(_conv.AllowsNull);
        Assert.IsFalse(_conv.TryParseValue(input.AsSpan(), out _));
    }

    [DataTestMethod]
    [DataRow(1.5)]
    [DataRow(-3.8)]
    public void ConvertToStringTest1(Double input)
    {
        Assert.IsFalse(_conv.AllowsNull);
        Assert.IsNotNull(_conv.ConvertToString(input));
    }

    [TestMethod]
    public void RoundtripTest1()
    {
        double d = 72.81;

        var conv = new DoubleConverter();

        string? s = conv.ConvertToString(d);
        Assert.IsNotNull(s);

        double? d2 = (double?)conv.Parse(s.AsSpan());
        Assert.AreEqual(d, d2);
    }

    [DataTestMethod]
    [DataRow("D")]
    //[DataRow("R")]
    [DataRow("X")]
    [ExpectedException(typeof(ArgumentException))]
    public void ValidateFormatTest(string? format) => new DoubleConverter(format: format);
}

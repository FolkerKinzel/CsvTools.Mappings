namespace FolkerKinzel.CsvTools.Mappings.Converters.Tests;

[TestClass]
public class SingleConverterTests
{
    private readonly SingleConverter _conv = new();

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
    [DataRow(1.5f)]
    [DataRow(-3.8f)]
    public void ConvertToStringTest1(float input)
    {
        Assert.IsFalse(_conv.AllowsNull);
        Assert.IsNotNull(_conv.ConvertToString(input));
    }

    [DataTestMethod]
    [DataRow("D")]
    //[DataRow("R")]
    [DataRow("X")]
    [ExpectedException(typeof(ArgumentException))]
    public void ValidateFormatTest(string? format) => new SingleConverter(format: format);
}

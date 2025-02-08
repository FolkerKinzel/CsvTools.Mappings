namespace FolkerKinzel.CsvTools.Mappings.TypeConverters.Tests;

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
        Assert.IsFalse(_conv.AcceptsNull);
        Assert.IsFalse(_conv.TryParse(input.AsSpan(), out _));
    }

    [DataTestMethod]
    [DataRow(1.5f)]
    [DataRow(-3.8f)]
    public void ConvertToStringTest1(float input)
    {
        Assert.IsFalse(_conv.AcceptsNull);
        Assert.IsNotNull(_conv.ConvertToString(input));
    }

    [DataTestMethod]
    [DataRow("D")]
    //[DataRow("R")]
    [DataRow("X")]
    [ExpectedException(typeof(ArgumentException))]
    public void ValidateFormatTest(string? format) => new SingleConverter(format: format);
}

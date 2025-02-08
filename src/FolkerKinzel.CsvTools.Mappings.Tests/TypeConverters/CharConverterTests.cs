namespace FolkerKinzel.CsvTools.Mappings.TypeConverters.Tests;

[TestClass]
public class CharConverterTests
{
    private readonly CharConverter _conv = new();

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
        Assert.IsNotNull(_conv.ConvertToString('a'));
    }

    [TestMethod]
    public void RoundtripTest1()
    {
        const char a = 'a';
        string? csv = _conv.ConvertToString(a);
        Assert.AreEqual(a, _conv.Parse(csv.AsSpan()));
    }

    [TestMethod]
    public void ToHexConverterTest1()
    {
        TypeConverter<byte> conv = new ByteConverter().ToHexConverter();
        Assert.AreEqual("2A", conv.ConvertToString(42));
        Assert.AreEqual(42, conv.Parse("2A".AsSpan()));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void ValidateFormatTest() => new ByteConverter(format: "R");
    
}

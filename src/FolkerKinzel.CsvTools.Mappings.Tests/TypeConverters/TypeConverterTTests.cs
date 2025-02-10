using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Mappings.TypeConverters.Tests;

[TestClass]
public class TypeConverterTTests
{
    [TestMethod]
    public void ParseTest1()
    {
        var conv = new Int32Converter(throwing: false);
        Assert.AreEqual(conv.DefaultValue, conv.Parse(default));
        Assert.AreEqual(conv.DefaultValue, conv.Parse("blabla".AsSpan()));
        Assert.AreEqual(42, conv.Parse("42".AsSpan()));
    }

    [TestMethod]
    public void ParseTest2()
    {
        var conv = new Int32Converter();
        Assert.AreEqual(conv.DefaultValue, conv.Parse(default));
        Assert.AreEqual(42, conv.Parse("42".AsSpan()));
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public void ParseTest3()
    {
        var conv = new Int32Converter();
        Assert.AreEqual(conv.DefaultValue, conv.Parse("blabla".AsSpan()));
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public void ParseTest4()
    {
        var conv = new Int32Converter();
        Assert.AreEqual(conv.DefaultValue, conv.Parse("blablaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa".AsSpan()));
    }
}

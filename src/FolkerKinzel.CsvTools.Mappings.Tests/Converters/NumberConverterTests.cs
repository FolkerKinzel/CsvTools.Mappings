using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Mappings.Converters.Tests;

[TestClass()]
public class NumberConverterTests
{
    [TestMethod]
    public void NumberConverterTest1()
    {
        var conv = new DoubleConverter();
        Assert.IsNotNull(conv);
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
}
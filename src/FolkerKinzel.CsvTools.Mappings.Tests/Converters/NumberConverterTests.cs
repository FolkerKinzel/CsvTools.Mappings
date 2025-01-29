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

    
}
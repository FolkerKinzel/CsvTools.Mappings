using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace FolkerKinzel.CsvTools.Mappings.Converters.Tests;

[TestClass]
public class BooleanConverterTests
{
    [TestMethod]
    public void BooleanConverterTest1()
    {
        string blString = true.ToString(CultureInfo.CreateSpecificCulture("de-DE"));
        var conv = new BooleanConverter();
        Assert.IsTrue(conv.TryParseValue(blString.AsSpan(), out bool result));
        Assert.IsTrue(result);
    }
}

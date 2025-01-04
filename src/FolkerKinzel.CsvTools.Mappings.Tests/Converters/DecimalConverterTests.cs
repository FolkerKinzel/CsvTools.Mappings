using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace FolkerKinzel.CsvTools.Mappings.Converters.Tests;

[TestClass]
public class DecimalConverterTests
{
    [TestMethod]
    public void DecimalConverterTest1()
    {
        decimal d = 1234.567M;

        string s = d.ToString("x", CultureInfo.CreateSpecificCulture("de-DE"));
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Text;

namespace FolkerKinzel.CsvTools.Mappings.Tests;

[TestClass]
public class CsvReaderTests
{
    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void ResetTest()
    {
        using var stringReader = new StringReader("");
        using CsvReader<dynamic> objectReader = CsvConverter.OpenRead(stringReader, CsvMappingBuilder.Create().Build(), dyn => dyn);
        ((IEnumerator)objectReader).Reset();
    }

    [TestMethod]
    public void IEnumerableTest()
    {
        using var stringReader = new StringReader("Hi");
        using CsvReader<dynamic> objectReader = CsvConverter.OpenRead(stringReader,
                                                                      CsvMappingBuilder.Create().Build(),
                                                                      dyn => dyn,
                                                                      isHeaderPresent: false);
        Assert.AreEqual(1, objectReader.AsWeakEnumerable().Count());
    }

    [TestMethod]
    public void CsvReaderTest1()
    {
        using var stringReader = new StringReader("Hi");
        using CsvReader csvReader = Csv.OpenRead(stringReader, isHeaderPresent: false);
        using var reader = new CsvReader<string>(csvReader, CsvMappingBuilder.Create().Build(), dyn => "Hi");

        Assert.IsNotNull(reader);
    }
}

using System.Collections;

namespace FolkerKinzel.CsvTools.Mappings.Tests;

[TestClass]
public class CsvReaderTests
{
    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void ResetTest()
    {
        using var stringReader = new StringReader("");
        using CsvReader<dynamic> objectReader = CsvConverter.OpenRead(stringReader, CsvRecordMappingBuilder.Create().Build(), dyn => dyn);
        ((IEnumerator)objectReader).Reset();
    }

    [TestMethod]
    public void IEnumerableTest()
    {
        using var stringReader = new StringReader("Hi");
        using CsvReader<dynamic> objectReader = CsvConverter.OpenRead(stringReader, 
                                                                      CsvRecordMappingBuilder.Create().Build(), 
                                                                      dyn => dyn, 
                                                                      isHeaderPresent: false);
        Assert.AreEqual(1, objectReader.AsWeakEnumerable().Count());
    }
}

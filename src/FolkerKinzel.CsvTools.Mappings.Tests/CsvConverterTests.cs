using FolkerKinzel.CsvTools.Mappings.TypeConverters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Mappings.Tests;

[TestClass]
public class CsvConverterTests
{
    [TestMethod]
    public void OpenReadTest1()
    {
        var conv = new Int32Converter();

        CsvMapping mapping = CsvMappingBuilder.Create().AddProperty("A", conv).AddProperty("B", conv).Build();

        using var stringReader = new StringReader("""
            A,B
            1,2
            3,4
            """);

        using CsvReader<(int A, int B)> tupleReader =
            CsvConverter.OpenRead<(int A, int B)>(stringReader, 
                                                  mapping, 
                                                  dyn => (dyn.A, dyn.B), 
                                                  options: CsvOpts.Default | CsvOpts.DisableCaching);
        (int A, int B)[] results = [.. tupleReader];

        CollectionAssert.AreEqual(new (int A, int B)[] { (1, 2), (3, 4) }, results);
    }

    [TestMethod]
    public void OpenReadTest2()
    {
        var conv = new Int32Converter();

        CsvMapping mapping = CsvMappingBuilder.Create().AddProperty("A", conv).AddProperty("B", conv).Build();

        using var stringReader = new StringReader("""
            A,B
            1,2
            3,4
            """);

        using CsvReader<CsvMapping> tupleReader = CsvConverter.OpenRead<CsvMapping>(stringReader,
                                                                            mapping, 
                                                                            dyn => dyn);
        CsvMapping[] results = [.. tupleReader];

        var resultVals = new (int A, int B)[] { (results[0]["A"].AsITypedProperty<int>().Value, results[0]["B"].AsITypedProperty<int>().Value),
                                                (results[1]["A"].AsITypedProperty<int>().Value, results[1]["B"].AsITypedProperty<int>().Value)
                                              };

        CollectionAssert.AreEqual(new (int A, int B)[] { (1, 2), (3, 4) }, resultVals);
    }
}

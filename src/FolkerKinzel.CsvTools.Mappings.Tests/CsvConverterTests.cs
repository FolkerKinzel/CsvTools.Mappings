using FolkerKinzel.CsvTools.Mappings.Intls;
using FolkerKinzel.CsvTools.Mappings.TypeConverters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;

namespace FolkerKinzel.CsvTools.Mappings.Tests;

[TestClass]
public class CsvConverterTests
{
    [NotNull]
    public TestContext? TestContext { get; set; }

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

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OpenReadTest3()
    {
        using var stringReader = new StringReader("a,b,c");
        using CsvReader<string> reader2 
            = CsvConverter.OpenRead<string>(stringReader, CsvMappingBuilder.Create().Build(), null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void WriteTest1()
    {
        using var stringWriter = new StringWriter();
        using CsvWriter writer = Csv.OpenWrite(stringWriter, 1);
        CsvConverter.Write(["Hi"], writer, CsvMappingBuilder.Create().Build(), null!);
    }

    [TestMethod]
    public void ToCsvTest1()
    {
        var converter = new Int32Converter();
        (int A, int B)[] values = [(7, -1), (42, 4711)];

        CsvMapping mapping = CsvMappingBuilder.Create().AddProperty("A", 0, converter).AddProperty("B", 1, converter).Build();

        string csv = CsvConverter.ToCsv(values, 2, mapping, (tuple, dyn) => { dyn.A = tuple.A; dyn.B = tuple.B; });

        (int A, int B)[] results = CsvConverter.Parse<(int A, int B)>(csv, mapping, dyn => (dyn.A, dyn.B), isHeaderPresent: false);

        CollectionAssert.AreEqual(values, results);
    }


    [TestMethod]
    public void SaveTest1()
    {
        string filePath = Path.Combine(TestContext!.TestRunResultsDirectory!, "SaveTest1.csv");

        var converter = new Int32Converter();
        (int A, int B)[] values = [(7, -1), (42, 4711)];

        CsvMapping mapping = CsvMappingBuilder.Create().AddProperty("A", 0, converter).AddProperty("B", 1, converter).Build();

        CsvConverter.Save(values, filePath, 2, mapping, (tuple, dyn) => { dyn.A = tuple.A; dyn.B = tuple.B; });

        using CsvReader<(int A, int B)> reader = CsvConverter.OpenRead<(int A, int B)>(filePath, mapping, dyn => (dyn.A, dyn.B), isHeaderPresent: false);
        (int A, int B)[] results = [.. reader];

        CollectionAssert.AreEqual(values, results);
    }
}

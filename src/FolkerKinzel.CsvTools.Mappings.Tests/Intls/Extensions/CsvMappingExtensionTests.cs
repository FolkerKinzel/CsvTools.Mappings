using FolkerKinzel.CsvTools.Mappings.TypeConverters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Mappings.Intls.Extensions.Tests;

[TestClass]
public class CsvMappingExtensionTests
{
    public TestContext? TestContext { get; set; }

    [TestMethod]
    public void ToCsvTest1()
    {
        var converter = new Int32Converter();
        (int A, int B)[] values = [(7, -1), (42, 4711)];

        CsvMapping mapping = CsvMappingBuilder.Create().AddProperty("A", converter).AddProperty("B", converter).Build();

        string csv = values.ToCsv(mapping, (tuple, dyn) => { dyn.A = tuple.A; dyn.B = tuple.B; });

        (int A, int B)[] results = CsvConverter.Parse<(int A, int B)>(csv, mapping, dyn => (dyn.A, dyn.B));

        CollectionAssert.AreEqual(values, results);
    }

    [TestMethod]
    public void ToCsvTest2()
    {
        var converter = new Int32Converter();
        (int A, int B)[] values = [(7, -1), (42, 4711)];

        CsvMapping mapping = CsvMappingBuilder.Create().AddProperty("A", 0, converter).AddProperty("B", 1, converter).Build();

        string csv = values.ToCsv(2, mapping, (tuple, dyn) => { dyn.A = tuple.A; dyn.B = tuple.B; });

        (int A, int B)[] results = CsvConverter.Parse<(int A, int B)>(csv, mapping, dyn => (dyn.A, dyn.B), isHeaderPresent: false);

        CollectionAssert.AreEqual(values, results);
    }

    [TestMethod]
    public void ToCsvTest3()
    {
        var converter = new Int32Converter();
        (int A, int B)[] values = [(7, -1), (42, 4711)];

        CsvMapping mapping = CsvMappingBuilder.Create().AddProperty("A", converter).AddProperty("B", converter).Build();

        string csv = values.ToCsv(mapping, (tuple, dyn) => { dyn.A = tuple.A; dyn.B = tuple.B; });

        (int A, int B)[] results = CsvConverter.ParseAnalyzed<(int A, int B)>(csv, mapping, dyn => (dyn.A, dyn.B));

        CollectionAssert.AreEqual(values, results);
    }

    [TestMethod]
    public void SaveCsvTest1()
    {
        string filePath = Path.Combine(TestContext!.TestRunResultsDirectory!, "SaveCsvTest1.csv");

        var converter = new Int32Converter();
        (int A, int B)[] values = [(7, -1), (42, 4711)];

        CsvMapping mapping = CsvMappingBuilder.Create().AddProperty("A", converter).AddProperty("B", converter).Build();

        values.SaveCsv(filePath, mapping, (tuple, dyn) => { dyn.A = tuple.A; dyn.B = tuple.B; });

        using CsvReader<(int A, int B)> reader = CsvConverter.OpenRead<(int A, int B)>(filePath, mapping, dyn => (dyn.A, dyn.B));

        (int A, int B)[] results = [.. reader];

        CollectionAssert.AreEqual(values, results);
    }

    [TestMethod]
    public void SaveCsvTest2()
    {
        string filePath = Path.Combine(TestContext!.TestRunResultsDirectory!, "SaveCsvTest2.csv");

        var converter = new Int32Converter();
        (int A, int B)[] values = [(7, -1), (42, 4711)];

        CsvMapping mapping = CsvMappingBuilder.Create().AddProperty("A", 0, converter).AddProperty("B", 1, converter).Build();

        values.SaveCsv(filePath, 2, mapping, (tuple, dyn) => { dyn.A = tuple.A; dyn.B = tuple.B; });

        using CsvReader<(int A, int B)> reader = CsvConverter.OpenRead<(int A, int B)>(filePath, mapping, dyn => (dyn.A, dyn.B), isHeaderPresent: false);
        (int A, int B)[] results = [.. reader];

        CollectionAssert.AreEqual(values, results);
    }

    [TestMethod]
    public void SaveCsvTest3()
    {
        string filePath = Path.Combine(TestContext!.TestRunResultsDirectory!, "SaveCsvTest1.csv");

        var converter = new Int32Converter();
        (int A, int B)[] values = [(7, -1), (42, 4711)];

        CsvMapping mapping = CsvMappingBuilder.Create().AddProperty("A", converter).AddProperty("B", converter).Build();

        values.SaveCsv(filePath, mapping, (tuple, dyn) => { dyn.A = tuple.A; dyn.B = tuple.B; });

        using CsvReader<(int A, int B)> reader = CsvConverter.OpenReadAnalyzed<(int A, int B)>(filePath, mapping, dyn => (dyn.A, dyn.B));

        (int A, int B)[] results = [.. reader];

        CollectionAssert.AreEqual(values, results);
    }
}

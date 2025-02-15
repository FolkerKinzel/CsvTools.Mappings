using FolkerKinzel.CsvTools.Mappings.TypeConverters;
using System;
using System.Data;

namespace FolkerKinzel.CsvTools.Mappings.Tests;

[TestClass]
public class DataTableExtensionTests
{
    public TestContext? TestContext { get; set; }

    [TestMethod]
    public void DataTableTest1()
    {
        string filePath = Path.Combine(TestContext!.TestRunResultsDirectory!, "DataTableTest1.csv");

        var converter = new Int32Converter();

        using var table = new DataTable();
        table.Columns.Add("A", typeof(int));
        table.Columns.Add("B", typeof(int));

        table.Rows.Add(7, -1);
        table.Rows.Add(42, 4711);
        table.Rows.Add(55, 44);
        table.AcceptChanges();
        table.Rows[2].Delete();

        CsvMapping mapping = CsvMappingBuilder.Create().AddProperty("A", converter).AddProperty("B", converter).Build();

        table.WriteCsv(filePath, mapping);

        table.Clear();

        table.ReadCsv(filePath, mapping);

        CollectionAssert.AreEqual(new object[] { 7, -1 }, table.Rows[0].ItemArray);
        CollectionAssert.AreEqual(new object[] { 42, 4711 }, table.Rows[1].ItemArray);
    }

    [TestMethod]
    public void DataTableTest2()
    {
        string filePath = Path.Combine(TestContext!.TestRunResultsDirectory!, "DataTableTest1.csv");

        var converter = new Int32Converter();

        using var table = new DataTable();
        table.Columns.Add("A", typeof(int));
        table.Columns.Add("B", typeof(int));

        table.Rows.Add(7, -1);
        table.Rows.Add(42, 4711);

        CsvMapping mapping = CsvMappingBuilder.Create().AddProperty("A", converter).AddProperty("B", converter).Build();

        table.WriteCsv(filePath, mapping);

        table.Clear();

        table.ReadCsvAnalyzed(filePath, mapping);

        CollectionAssert.AreEqual(new object[] { 7, -1 }, table.Rows[0].ItemArray);
        CollectionAssert.AreEqual(new object[] { 42, 4711 }, table.Rows[1].ItemArray);
    }

    [TestMethod]
    public void DataTableTest3()
    {
        var converter = new Int32Converter();

        using var table = new DataTable();
        table.Columns.Add("A", typeof(int));
        table.Columns.Add("B", typeof(int));

        table.Rows.Add(7, -1);
        table.Rows.Add(42, 4711);

        CsvMapping mapping = CsvMappingBuilder.Create().AddProperty("A", converter).AddProperty("B", converter).Build();

        string csv;

        using var stringWriter = new StringWriter();
        using var csvWriter = new CsvWriter(stringWriter, ["A", "B"]);

        table.WriteCsv(csvWriter, mapping);
        csv = stringWriter.ToString();

        table.Clear();
        using var stringReader = new StringReader(csv);
        using var csvReader = new CsvReader(stringReader);
        table.ReadCsv(csvReader, mapping);

        CollectionAssert.AreEqual(new object[] { 7, -1 }, table.Rows[0].ItemArray);
        CollectionAssert.AreEqual(new object[] { 42, 4711 }, table.Rows[1].ItemArray);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void ReadWithWrongMappingTest1()
    {
        var converter = new Int32Converter();
        CsvMapping mapping = CsvMappingBuilder.Create().AddProperty("NotInTable", converter).Build();

        using var table = new DataTable();
        table.Columns.Add("A", typeof(int));
        table.Columns.Add("B", typeof(int));

        using var stringReader = new StringReader("""
            A,B
            1,2
            """);
        using var csvReader = new CsvReader(stringReader);

        table.ReadCsv(csvReader, mapping);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void WriteWithWrongMappingTest1()
    {
        var converter = new Int32Converter();
        CsvMapping mapping = CsvMappingBuilder.Create().AddProperty("NotInTable", converter).Build();

        using var table = new DataTable();
        table.Columns.Add("A", typeof(int));
        table.Columns.Add("B", typeof(int));

        table.Rows.Add(42, 4711);

        using var stringWriter = new StringWriter();
        using var csvWriter = new CsvWriter(stringWriter, ["A", "B"]);
        table.WriteCsv(csvWriter, mapping);
    }
}

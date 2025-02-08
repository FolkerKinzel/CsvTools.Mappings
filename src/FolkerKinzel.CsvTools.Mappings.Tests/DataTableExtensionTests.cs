using FolkerKinzel.CsvTools.Mappings.TypeConverters;
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

        table.Rows.Add( 7, -1 );
        table.Rows.Add(42, 4711);

        CsvRecordMapping mapping = CsvRecordMappingBuilder.Create().AddProperty("A", converter).AddProperty("B", converter).Build();

        table.WriteCsv(filePath, ["A", "B"], mapping);

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

        CsvRecordMapping mapping = CsvRecordMappingBuilder.Create().AddProperty("A", converter).AddProperty("B", converter).Build();

        table.WriteCsv(filePath, ["A", "B"], mapping);

        table.Clear();

        table.ReadCsvAnalyzed(filePath, mapping);

        CollectionAssert.AreEqual(new object[] { 7, -1 }, table.Rows[0].ItemArray);
        CollectionAssert.AreEqual(new object[] { 42, 4711 }, table.Rows[1].ItemArray);
    }
}

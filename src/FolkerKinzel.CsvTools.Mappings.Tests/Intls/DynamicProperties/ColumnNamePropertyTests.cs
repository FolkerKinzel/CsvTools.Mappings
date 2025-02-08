using FolkerKinzel.CsvTools.Mappings.TypeConverters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Mappings.Intls.DynamicProperties.Tests;

[TestClass]
public class ColumnNamePropertyTests
{
    [TestMethod]
    public void NoTargetTest()
    {
        TypeConverter<int?> conv = new Int32Converter().ToNullableConverter();

        CsvRecordMapping mapping = CsvRecordMappingBuilder
            .Create()
            .AddProperty("NotInCsv", conv)
            .Build();

        int?[] results = CsvConverter.Parse<int?>("""
            A
            42
            """, mapping, dyn => dyn.NotInCsv);
        Assert.AreEqual(1, results.Length);
        Assert.IsFalse(results[0].HasValue);
    }
}

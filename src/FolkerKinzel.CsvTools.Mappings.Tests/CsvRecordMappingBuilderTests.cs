using FolkerKinzel.CsvTools.Mappings.TypeConverters;

namespace FolkerKinzel.CsvTools.Mappings.Tests;

[TestClass]
public class CsvRecordMappingBuilderTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void MultiColumnPropertyTest2()
        => _ = CsvRecordMappingBuilder.Create().AddProperty("PropName", (MultiColumnTypeConverter<int>)null!);

    [TestMethod()]
    public void CreateAndBuildTest1()
    {
        var builder = CsvRecordMappingBuilder.Create();
        Assert.IsInstanceOfType<CsvRecordMappingBuilder>(builder);
        CsvRecordMapping mapping1 = builder.Build();
        Assert.IsInstanceOfType<CsvRecordMapping>(mapping1);
        CsvRecordMapping mapping2 = builder.Build();
        Assert.IsInstanceOfType<CsvRecordMapping>(mapping1);
        Assert.AreNotSame(mapping1, mapping2);

        Assert.AreEqual(0, mapping2.Count);

        CsvRecordMapping mapping3 = builder.AddProperty("Prop", new ByteConverter()).Build();
        Assert.AreEqual(1, mapping3.Count);

        CsvRecordMapping mapping4 = builder.Build();
        Assert.AreEqual(0, mapping4.Count);
    }
}

using FolkerKinzel.CsvTools.Mappings.TypeConverters;

namespace FolkerKinzel.CsvTools.Mappings.Tests;

[TestClass]
public class CsvMappingBuilderTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void MultiColumnPropertyTest2()
        => _ = CsvMappingBuilder.Create().AddProperty("PropName", (MultiColumnTypeConverter<int>)null!);

    [TestMethod()]
    public void CreateAndBuildTest1()
    {
        var builder = CsvMappingBuilder.Create();
        Assert.IsInstanceOfType<CsvMappingBuilder>(builder);
        CsvMapping mapping1 = builder.Build();
        Assert.IsInstanceOfType<CsvMapping>(mapping1);
        CsvMapping mapping2 = builder.Build();
        Assert.IsInstanceOfType<CsvMapping>(mapping1);
        Assert.AreNotSame(mapping1, mapping2);

        Assert.AreEqual(0, mapping2.Count);

        CsvMapping mapping3 = builder.AddProperty("Prop", new ByteConverter()).Build();
        Assert.AreEqual(1, mapping3.Count);

        CsvMapping mapping4 = builder.Build();
        Assert.AreEqual(0, mapping4.Count);
    }
}

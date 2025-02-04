using FolkerKinzel.CsvTools.Mappings.Converters;

namespace FolkerKinzel.CsvTools.Mappings.Tests;

[TestClass]
public class MappingBuilderTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void MultiColumnPropertyTest2()
        => _ = MappingBuilder.Create().AddProperty("PropName", (MultiColumnTypeConverter<int>)null!);

    [TestMethod()]
    public void CreateAndBuildTest1()
    {
        var builder = MappingBuilder.Create();
        Assert.IsInstanceOfType<MappingBuilder>(builder);
        Mapping mapping1 = builder.Build();
        Assert.IsInstanceOfType<Mapping>(mapping1);
        Mapping mapping2 = builder.Build();
        Assert.IsInstanceOfType<Mapping>(mapping1);
        Assert.AreNotSame(mapping1, mapping2);

        Assert.AreEqual(0, mapping2.Count);

        Mapping mapping3 = builder.AddProperty("Prop", new ByteConverter()).Build();
        Assert.AreEqual(1, mapping3.Count);

        Mapping mapping4 = builder.Build();
        Assert.AreEqual(0, mapping4.Count);
    }
}

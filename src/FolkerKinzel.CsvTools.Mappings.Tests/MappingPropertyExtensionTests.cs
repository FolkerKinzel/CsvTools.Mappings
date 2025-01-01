using FolkerKinzel.CsvTools.Mappings.Converters;

namespace FolkerKinzel.CsvTools.Mappings.Tests;

[TestClass]
public class MappingPropertyExtensionTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void AsTest1()
    {
        MappingProperty? prop = null;
        _ = prop!.As<int>();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidCastException))]
    public void AsTest2()
    {
        MappingProperty prop = new IndexProperty<string?>("name", 0, new StringConverter());
        _ = prop.As<int>();
    }

    [TestMethod]
    public void AsTest3()
    {
        MappingProperty? prop = new IndexProperty<string?>("name", 0, new StringConverter());
        ITypedProperty<string?> casted = prop.As<string?>();
        Assert.IsNotNull(casted);
    }
}

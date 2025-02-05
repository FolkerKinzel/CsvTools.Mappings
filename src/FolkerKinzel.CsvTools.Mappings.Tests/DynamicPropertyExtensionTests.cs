using FolkerKinzel.CsvTools.Mappings.Converters;
using FolkerKinzel.CsvTools.Mappings.Intls.MappingProperties;

namespace FolkerKinzel.CsvTools.Mappings.Tests;

[TestClass]
public class DynamicPropertyExtensionTests
{
    [TestMethod]
    public void AsITypedPropertyTest1()
    {
        DynamicProperty? prop = null;
        Assert.IsNull(prop!.AsITypedProperty<int>());
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidCastException))]
    public void AsITypedPropertyTest2()
    {
        DynamicProperty prop = new IndexProperty<string?>("name", 0, StringConverter.CreateNullable());
        _ = prop.AsITypedProperty<int>();
    }

    [TestMethod]
    public void AsITypedPropertyTest3()
    {
        DynamicProperty? prop = new IndexProperty<string?>("name", 0, StringConverter.CreateNullable());
        ITypedProperty<string?> casted = prop.AsITypedProperty<string?>();
        Assert.IsNotNull(casted);
    }
}

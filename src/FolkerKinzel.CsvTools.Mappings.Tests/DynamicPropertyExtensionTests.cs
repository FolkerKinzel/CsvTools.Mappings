using FolkerKinzel.CsvTools.Mappings.Converters;
using FolkerKinzel.CsvTools.Mappings.Intls.MappingProperties;

namespace FolkerKinzel.CsvTools.Mappings.Tests;

[TestClass]
public class DynamicPropertyExtensionTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void AsTest1()
    {
        DynamicProperty? prop = null;
        _ = prop!.AsITypedProperty<int>();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidCastException))]
    public void AsTest2()
    {
        DynamicProperty prop = new IndexProperty<string?>("name", 0, StringConverter.CreateNullable());
        _ = prop.AsITypedProperty<int>();
    }

    [TestMethod]
    public void AsTest3()
    {
        DynamicProperty? prop = new IndexProperty<string?>("name", 0, StringConverter.CreateNullable());
        ITypedProperty<string?> casted = prop.AsITypedProperty<string?>();
        Assert.IsNotNull(casted);
    }
}

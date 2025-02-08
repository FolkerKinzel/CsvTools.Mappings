using FolkerKinzel.CsvTools.Mappings.TypeConverters;
using FolkerKinzel.CsvTools.Mappings.Intls.MappingProperties;
using System.Collections.ObjectModel;

namespace FolkerKinzel.CsvTools.Mappings.Intls.Extensions.Tests;

internal sealed class TestCollection : KeyedCollection<string, DynamicProperty>
{
    protected override string GetKeyForItem(DynamicProperty item) => item.PropertyName;
}

[TestClass()]
public class PropertyCollectionExtensionsTests
{
    [TestMethod()]
    public void TryGetValueTest()
    {
        KeyedCollection<string, DynamicProperty> kColl = new TestCollection();

        var prop1 = new ColumnNameProperty<bool>("Test", [], new BooleanConverter());

        kColl.Add(prop1);

        Assert.IsTrue(kColl.TryGetValue("Test", out DynamicProperty? prop2));

        Assert.AreEqual(prop1, prop2);
    }
}

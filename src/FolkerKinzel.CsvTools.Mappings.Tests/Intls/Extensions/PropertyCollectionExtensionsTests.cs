using FolkerKinzel.CsvTools.Mappings.Converters;
using FolkerKinzel.CsvTools.Mappings.Intls.MappingProperties;
using System.Collections.ObjectModel;

namespace FolkerKinzel.CsvTools.Mappings.Intls.Extensions.Tests;

internal sealed class TestCollection : KeyedCollection<string, MappingProperty>
{
    protected override string GetKeyForItem(MappingProperty item) => item.PropertyName;
}

[TestClass()]
public class PropertyCollectionExtensionsTests
{
    [TestMethod()]
    public void TryGetValueTest()
    {
        KeyedCollection<string, MappingProperty> kColl = new TestCollection();

        var prop1 = new ColumnNameProperty<bool>("Test", [], new BooleanConverter());

        kColl.Add(prop1);

        Assert.IsTrue(kColl.TryGetValue("Test", out MappingProperty? prop2));

        Assert.AreEqual(prop1, prop2);
    }
}

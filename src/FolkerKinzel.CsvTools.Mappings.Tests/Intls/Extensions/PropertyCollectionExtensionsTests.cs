using FolkerKinzel.CsvTools.Mappings.Converters;
using System.Collections.ObjectModel;

namespace FolkerKinzel.CsvTools.Mappings.Intls.Extensions.Tests;

internal sealed class TestCollection : KeyedCollection<string, CsvProperty>
{
    protected override string GetKeyForItem(CsvProperty item) => item.PropertyName;
}

[TestClass()]
public class PropertyCollectionExtensionsTests
{
    [TestMethod()]
    public void TryGetValueTest()
    {
        KeyedCollection<string, CsvProperty> kColl = new TestCollection();

        var prop1 = new CsvColumnNameProperty<bool>("Test", [], new BooleanConverter());

        kColl.Add(prop1);

        Assert.IsTrue(kColl.TryGetValue("Test", out CsvProperty? prop2));

        Assert.AreEqual(prop1, prop2);
    }
}

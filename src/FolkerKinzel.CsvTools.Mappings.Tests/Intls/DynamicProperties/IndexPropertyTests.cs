using FolkerKinzel.CsvTools.Mappings.Tests;
using FolkerKinzel.CsvTools.Mappings.TypeConverters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Mappings.Intls.DynamicProperties.Tests;

[TestClass]
public class IndexPropertyTests
{
    [TestMethod]
    public void CloneTest1()
    {
        var prop1 = new IndexProperty<int>("Prop", 42, new Int32Converter());
        var prop2 = (IndexProperty<int>)prop1.Clone();
        Assert.AreNotSame(prop1, prop2);
        
        Assert.AreEqual(prop1.PropertyName, prop2.PropertyName);
        Assert.AreEqual(prop1.Index, prop2.Index);
        Assert.AreEqual(prop1.Converter, prop2.Converter);
        Assert.AreEqual(prop1.Record, prop2.Record);
    }
}

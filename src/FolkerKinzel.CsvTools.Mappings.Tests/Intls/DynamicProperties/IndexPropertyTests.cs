using FolkerKinzel.CsvTools.Mappings.Tests;
using FolkerKinzel.CsvTools.Mappings.TypeConverters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Mappings.Intls.DynamicProperties.Tests;

[TestClass]
public class SingleColumnPropertyTests
{
    [TestMethod]
    public void TypedPropertyTest1()
    {
        var prop1 = new IndexProperty<int>("Prop", 0, new Int32Converter());
        Assert.AreEqual(0, prop1.DefaultValue);

        CsvRecord record = Utility.CreateCsvRecord(1);
        prop1.Record = record;

        ITypedProperty<int> typedProp = prop1.AsITypedProperty<int>();
        Assert.AreEqual(0, typedProp.DefaultValue);

        typedProp.Value = 42;
        Assert.AreEqual("42", typedProp.Record![0].ToString());
    }
}

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

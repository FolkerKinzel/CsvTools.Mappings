using FolkerKinzel.CsvTools.Mappings.Tests;
using FolkerKinzel.CsvTools.Mappings.TypeConverters;

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

    [TestMethod]
    public void SetValueTest1()
    {
        var prop1 = new IndexProperty<string>("Prop", 0, StringConverter.CreateNonNullable());
        Assert.AreEqual("", prop1.DefaultValue);

        CsvRecord record = Utility.CreateCsvRecord(1);
        prop1.Record = record;

        prop1.Value = null;

        Assert.IsTrue(record[0].IsEmpty);
    }
}

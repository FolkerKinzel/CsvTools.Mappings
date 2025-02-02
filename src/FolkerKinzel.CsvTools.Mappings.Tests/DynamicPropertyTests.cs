using System;
using FolkerKinzel.CsvTools.Mappings.Converters;
using FolkerKinzel.CsvTools.Mappings.Intls.Converters;
using FolkerKinzel.CsvTools.Mappings.Intls.MappingProperties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Mappings.Tests;

[TestClass()]
public class DynamicPropertyTests
{
    [TestMethod()]
    public void CsvPropertyTest1()
    {
        var prop = new  ColumnNameProperty<string?>("Prop", ["Col1"], StringConverter.CreateNullable());
        Assert.IsNotNull(prop);
    }

    [TestMethod()]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CsvPropertyTest2() => _ = new ColumnNameProperty<string?>(null!, ["Col1"], StringConverter.CreateNullable());

    [TestMethod()]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CsvPropertyTest3() => _ = new ColumnNameProperty<string?>("Prop", null!, StringConverter.CreateNullable());

    [TestMethod()]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CsvPropertyTest4() => _ = new ColumnNameProperty<string?>("Prop", ["Col1"], null!);

    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    public void CsvPropertyTest5()
        => _ = new ColumnNameProperty<string?>("Ähh", ["Col1"], StringConverter.CreateNullable());

    [TestMethod()]
    public void CsvPropertyTest7()
        => _ = new ColumnNameProperty<string?>("Prop", ["Col1"], StringConverter.CreateNullable());

    [TestMethod()]
    public void CsvIndexPropertyTest9()
    {
        const string propertyName = "myProp";
        var prop = new IndexProperty<string?>(propertyName, 0, StringConverter.CreateNullable());

        Assert.IsNotNull(prop);
        Assert.AreEqual(prop.PropertyName, propertyName);
        Assert.IsInstanceOfType<StringConverterIntl>(((ITypedProperty<string?>)prop).Converter);
    }

    [TestMethod()]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void CsvPropertyTest10() => _ = new IndexProperty<string?>("propertyName", -1, StringConverter.CreateNullable());

    [TestMethod()]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CsvPropertyTest11() => _ = new IndexProperty<string?>(null!, 17, StringConverter.CreateNullable());

    [TestMethod()]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CsvPropertyTest12() => _ = new IndexProperty<string?>("Prop", 17, null!);

    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    public void CsvPropertyTest13() => _ = new IndexProperty<string?>("Ähh", 17, StringConverter.CreateNullable());

    //[TestMethod()]
    //[Obsolete("Obsolete")]
    //public void CloneTest1()
    //{
    //    const string propName = "Prop";
    //    string[] aliases = new string[] { "Col1", "Other" };
    //    ICsvTypeConverter conv = CsvConverterFactory.CreateConverter(CsvTypeCode.String);


    //    var prop = new CsvProperty(propName, aliases, conv);

    //    Assert.IsInstanceOfType(prop, typeof(CsvProperty));

    //    var clone = (CsvProperty)prop.Clone();

    //    Assert.AreNotSame(prop, clone);
    //    Assert.AreEqual(propName, prop.PropertyName, clone.PropertyName);
    //    CollectionAssert.AreEqual(prop.ColumnNameAliases, clone.ColumnNameAliases);
    //    Assert.AreSame(prop.Converter, clone.Converter);
    //}
}
using System;
using System.Collections.Generic;
using System.Globalization;
using FolkerKinzel.CsvTools.Mappings.Converters;
using FolkerKinzel.CsvTools.Mappings.Intls.MappingProperties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Mappings.Tests;

[TestClass()]
public class CsvRecordMappingTests
{
    [TestMethod()]
    public void CsvRecordWrapperTest()
    {
        var wrapper = CsvRecordMapping.Create();
        Assert.IsInstanceOfType<CsvRecordMapping>(wrapper);
    }

    //[TestMethod()]
    //public void InsertPropertyTest1()
    //{
    //    var wrapper = new CsvRecordMapping();

    //    const string prop1Name = "Prop1";
    //    const string prop2Name = "Prop2";

    //    var conv = new StringConverter();
    //    var prop1 =
    //        new ColumnNameProperty<string?>(prop1Name, ["Hallo1"],
    //        conv);

    //    var prop2 =
    //        new ColumnNameProperty<string?>(prop2Name, ["Hallo2"],
    //        conv);

    //    wrapper.Add(prop2);
    //    Assert.AreEqual(1, wrapper.Count);
    //    Assert.AreEqual(prop2Name, wrapper.PropertyNames.First());

    //    wrapper.InsertProperty(0, prop1);

    //    Assert.AreEqual(2, wrapper.Count);
    //    Assert.AreEqual(prop1Name, wrapper.PropertyNames.First());
    //    Assert.AreEqual(prop2Name, wrapper.PropertyNames.ElementAt(1));
    //}

    //[TestMethod()]
    //public void InsertPropertyTest2()
    //{
    //    var wrapper = new CsvRecordMapping();

    //    const string prop1Name = "Prop1";


    //    var prop1 =
    //        new ColumnNameProperty<string?>(prop1Name, ["Hallo1"],
    //        new StringConverter());


    //    wrapper.InsertProperty(0, prop1);
    //    Assert.AreEqual(1, wrapper.Count);
    //    Assert.AreEqual(prop1Name, wrapper.PropertyNames.First());
    //}

    //[TestMethod()]
    //[ExpectedException(typeof(ArgumentNullException))]
    //public void InsertPropertyTest3()
    //{
    //    var wrapper = new CsvRecordMapping();

    //    wrapper.InsertProperty(0, null!);
    //}


    //[TestMethod()]
    //[ExpectedException(typeof(ArgumentOutOfRangeException))]
    //public void InsertPropertyTest4()
    //{
    //    var wrapper = new CsvRecordMapping();

    //    const string prop1Name = "Prop1";


    //    var prop1 =
    //        new ColumnNameProperty<string?>(prop1Name, ["Hallo1"],
    //        new StringConverter());


    //    wrapper.InsertProperty(4711, prop1);
    //}

    //[TestMethod()]
    //[ExpectedException(typeof(ArgumentException))]
    //public void InsertPropertyTest5()
    //{
    //    var wrapper = new CsvRecordMapping();

    //    const string prop1Name = "Prop1";

    //    var conv = new StringConverter();

    //    var prop1 =
    //        new ColumnNameProperty<string?>(prop1Name, ["Hallo1"],
    //        conv);

    //    var prop2 =
    //        new ColumnNameProperty<string?>(prop1Name, ["Hallo2"],
    //        conv);

    //    wrapper.Add(prop2);
    //    wrapper.InsertProperty(0, prop1);
    //}


    //[TestMethod()]
    //public void ReplacePropertyAtTest1()
    //{
    //    var wrapper = new CsvRecordMapping();

    //    const string prop1Name = "Prop1";
    //    const string prop2Name = "Prop2";
    //    const string prop3Name = "Prop3";

    //    var conv = new StringConverter();


    //    var prop1 =
    //        new ColumnNameProperty<string?>(prop1Name, ["Hallo1"],
    //        conv);

    //    var prop2 =
    //        new ColumnNameProperty<string?>(prop2Name, ["Hallo2"],
    //        conv);

    //    var prop3 =
    //        new ColumnNameProperty<string?>(prop3Name, ["Hallo3"],
    //        conv);

    //    wrapper.AddProperty(prop1);
    //    wrapper.AddProperty(prop2);

    //    Assert.AreEqual(2, wrapper.Count);
    //    Assert.AreEqual(prop1Name, wrapper.PropertyNames.First());
    //    Assert.AreEqual(prop2Name, wrapper.PropertyNames.ElementAt(1));

    //    wrapper.ReplacePropertyAt(0, prop3);
    ////        new ColumnNameProperty<string?>(prop1Name, ["Hallo1"],
    ////        conv);

    //    Assert.AreEqual(2, wrapper.Count);
    //    Assert.AreEqual(prop3Name, wrapper.PropertyNames.First());
    //    Assert.AreEqual(prop2Name, wrapper.PropertyNames.ElementAt(1));
    //}

    //    var prop3 =
    //        new ColumnNameProperty<string?>(prop2Name, ["Hallo3"],
    //        conv);

    //    wrapper.Add(prop1);
    //    wrapper.Add(prop2);

    //    wrapper.ReplacePropertyAt(0, prop3);
    //}


    //[TestMethod()]
    //[ExpectedException(typeof(ArgumentOutOfRangeException))]
    //public void ReplacePropertyAtTest3()
    //{
    //    var wrapper = new CsvRecordMapping();

    //    const string prop1Name = "Prop1";
    //    const string prop2Name = "Prop2";
    //    const string prop3Name = "Prop3";

    //    var conv = new StringConverter();

    //    var prop1 =
    //        new ColumnNameProperty<string?>(prop1Name, ["Hallo1"],
    //        conv);

    //    var prop2 =
    //        new ColumnNameProperty<string?>(prop2Name, ["Hallo2"],
    //        conv);

    //    var prop3 =
    //        new ColumnNameProperty<string?>(prop3Name, ["Hallo3"],
    //        conv);

    //    wrapper.Add(prop1);
    //    wrapper.Add(prop2);

    //    wrapper.ReplacePropertyAt(4711, prop3);
    //}


    //[TestMethod()]
    //[ExpectedException(typeof(ArgumentNullException))]
    //public void ReplacePropertyAtTest4()
    //{
    //    var wrapper = new CsvRecordMapping();

    //    const string prop1Name = "Prop1";

    //    var prop1 =
    //        new ColumnNameProperty<string?>(prop1Name, ["Hallo1"],
    //        new StringConverter());


    //    wrapper.Add(prop1);

    //    wrapper.ReplacePropertyAt(0, null!);
    //}

    //[TestMethod()]
    //public void ReplacePropertyTest1()
    //{
    //    var wrapper = new CsvRecordMapping();

    //    const string prop1Name = "Prop1";
    //    const string prop2Name = "Prop2";
    //    const string prop3Name = "Prop3";

    //    var conv = new StringConverter();


    //    var prop1 =
    //        new ColumnNameProperty<string?>(prop1Name, ["Hallo1"],
    //        conv);

    //    var prop2 =
    //        new ColumnNameProperty<string?>(prop2Name, ["Hallo2"],
    //        conv);

    //    var prop3 =
    //        new ColumnNameProperty<string?>(prop3Name, ["Hallo3"],
    //        conv);

    //    wrapper.Add(prop1);
    //    wrapper.Add(prop2);

    //    Assert.AreEqual(2, wrapper.Count);
    //    Assert.AreEqual(prop1Name, wrapper.PropertyNames.First());
    //    Assert.AreEqual(prop2Name, wrapper.PropertyNames.ElementAt(1));

    //    wrapper.ReplaceProperty(prop1Name, prop3);

    //    Assert.AreEqual(2, wrapper.Count);
    //    Assert.AreEqual(prop3Name, wrapper.PropertyNames.First());
    //    Assert.AreEqual(prop2Name, wrapper.PropertyNames.ElementAt(1));
    //}

    //[TestMethod()]
    //[ExpectedException(typeof(ArgumentException))]
    //public void ReplacePropertyTest2a()
    //{
    //    var wrapper = new CsvRecordMapping();

    //    const string prop1Name = "Prop1";
    //    const string prop2Name = "Prop2";

    //    var conv = new StringConverter();


    //    var prop1 =
    //        new ColumnNameProperty<string?>(prop1Name, ["Hallo1"],
    //        conv);

    //    var prop2 =
    //        new ColumnNameProperty<string?>(prop2Name, ["Hallo2"],
    //        conv);

    //    var prop3 =
    //        new ColumnNameProperty<string?>(prop2Name, ["Hallo3"],
    //        conv);

    //    wrapper.Add(prop1);
    //    wrapper.Add(prop2);

    //    wrapper.ReplaceProperty(prop1Name, prop3);
    //}


    //[TestMethod()]
    //[ExpectedException(typeof(ArgumentException))]
    //public void ReplacePropertyTest2b()
    //{
    //    var wrapper = new CsvRecordMapping();

    //    const string prop1Name = "Prop1";

    //    var prop1 =
    //        new ColumnNameProperty<string?>(prop1Name, ["Hallo1"],
    //        new StringConverter());

    //    wrapper.ReplaceProperty("bla", prop1);
    //}


    //[TestMethod()]
    //[ExpectedException(typeof(ArgumentNullException))]
    //public void ReplacePropertyTest3()
    //{
    //    var wrapper = new CsvRecordMapping();

    //    const string prop1Name = "Prop1";

    //    var prop1 =
    //        new ColumnNameProperty<string?>(prop1Name, ["Hallo1"],
    //        new StringConverter());


    //    wrapper.Add(prop1);

    //    wrapper.ReplaceProperty(null!, prop1);
    //}



    //[TestMethod()]
    //[ExpectedException(typeof(ArgumentNullException))]
    //public void ReplacePropertyTest4()
    //{
    //    var wrapper = new CsvRecordMapping();

    //    const string prop1Name = "Prop1";

    //    var prop1 =
    //        new ColumnNameProperty<string?>(prop1Name, ["Hallo1"],
    //        new StringConverter());


    //    wrapper.Add(prop1);

    //    wrapper.ReplaceProperty(prop1Name, null!);
    //}




    [TestMethod()]
    [ExpectedException(typeof(InvalidOperationException))]
    public void TrySetMemberTest()
    {
        var wrapper = CsvRecordMapping.Create();

        const string prop1Name = "Prop1";

        var prop1 =
            new ColumnNameProperty<int?>(prop1Name, ["Hallo1"],
            new Int32Converter().ToNullableConverter());

        wrapper.Add(prop1);

        dynamic dyn = wrapper;

        dyn.Prop1 = 42;
    }


    [TestMethod()]
    [ExpectedException(typeof(InvalidOperationException))]
    public void TryGetMemberTest()
    {
        var wrapper = CsvRecordMapping.Create();

        const string prop1Name = "Prop1";

        var prop1 =
            new ColumnNameProperty<int?>(prop1Name, ["Hallo1"],
            new Int32Converter().ToNullableConverter());

        wrapper.Add(prop1);

        dynamic dyn = wrapper;

        _ = dyn.Prop1;
    }


    [TestMethod]
    public void DynPropTest()
    {
        var rec = new CsvRecord(["Hallo1", "Blabla"], false, true, true);

        var wrapper = CsvRecordMapping.Create();
        wrapper.Record = rec;

        const string prop1Name = "Prop1";
        const string prop2Name = "Prop2";

        var prop1 =
            new ColumnNameProperty<int?>(prop1Name, ["Hallo1"],
            new Int32Converter().ToNullableConverter());

        wrapper.Add(prop1);

        var prop2 =
            new ColumnNameProperty<string?>(prop2Name, ["Blub", null!, "Bla*"],
            StringConverter.CreateNullable());

        wrapper.Add(prop2);

        dynamic dyn = wrapper;

        const int val = 42;

        dyn.Prop1 = val;
        int i = dyn.Prop1;

        Assert.AreEqual(val, i);

        const string prop2Value = "HullyGully";
        dyn.Prop2 = prop2Value;
        string? s = dyn.Prop2;

        Assert.AreEqual(prop2Value, s);
    }


    [TestMethod()]
    public void GetEnumeratorTest1()
    {
        var rec = new CsvRecord(3);

        var wrapper = CsvRecordMapping.Create();
        wrapper.Record = rec;

        const string prop1Name = "Prop1";
        const string prop2Name = "Prop2";
        const string prop3Name = "Prop3";

        TypeConverter<int?> conv = new Int32Converter().ToNullableConverter();

        var prop1 =
            new IndexProperty<int?>(prop1Name, 0, conv);

        var prop2 =
            new IndexProperty<int?>(prop2Name, 1, conv);

        var prop3 =
            new IndexProperty<int?>(prop3Name, 2, conv);

        wrapper.Add(prop1);
        wrapper.Add(prop2);
        wrapper.Add(prop3);

        dynamic dyn = wrapper;

        dyn.Prop1 = 1;
        dyn.Prop2 = 2;
        dyn.Prop3 = 3;


        foreach (MappingProperty kvp in dyn)
        {
            switch (kvp.PropertyName)
            {
                case prop1Name:
                    Assert.AreEqual(1, kvp.Value);
                    break;
                case prop2Name:
                    Assert.AreEqual(2, kvp.Value);
                    break;
                case prop3Name:
                    Assert.AreEqual(3, kvp.Value);
                    break;
                default:
                    Assert.Fail();
                    break;
            }
        }
    }

    //[TestMethod()]
    //public void IndexOfTest()
    //{
    //    CsvRecordMapping wrapper =
    //        CsvRecordMapping.Create().AddSingleColumnProperty("Hallo", StringConverter.CreateNullable());

    //    Assert.AreEqual(0, wrapper.IndexOf("Hallo"));
    //    Assert.AreEqual(-1, wrapper.IndexOf("Wolli"));
    //    Assert.AreEqual(-1, wrapper.IndexOf(string.Empty));
    //}

    [TestMethod()]
    public void ContainsTest()
    {
        CsvRecordMapping wrapper =
            CsvRecordMapping.Create().AddSingleColumnProperty("Hallo", StringConverter.CreateNullable());

        Assert.IsTrue(wrapper.Contains("Hallo"));
        Assert.IsFalse(wrapper.Contains("Wolli"));
        Assert.IsFalse(wrapper.Contains(string.Empty));
    }


    [TestMethod()]
    public void AddPropertyTest1()
    {
        var wrapper = CsvRecordMapping.Create();

        Assert.AreEqual(0, wrapper.Count);

        var prop =
            new ColumnNameProperty<string?>("Hallo", ["Hallo"],
            StringConverter.CreateNullable());

        wrapper.Add(prop);

        Assert.AreEqual(1, wrapper.Count);
    }

    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    public void AddPropertyTest3()
    {
        var wrapper = CsvRecordMapping.Create();

        TypeConverter<string?> conv = StringConverter.CreateNullable();

        var prop1 =
            new ColumnNameProperty<string?>("Hallo", ["Hallo"],
            conv);

        var prop2 =
            new ColumnNameProperty<string?>("Hallo", ["Hallo"],
            conv);

        wrapper.Add(prop1);
        wrapper.Add(prop2);
    }

    //[TestMethod()]
    //public void RemovePropertyTest1()
    //{
    //    var wrapper = CsvRecordMapping.Create();

    //    const string prop1Name = "Prop1";
    //    const string prop2Name = "Prop2";

    //    var conv = StringConverter.CreateNullable();


    //    var prop1 =
    //        new ColumnNameProperty<string?>(prop1Name, ["Hallo1"],
    //        conv);

    //    var prop2 =
    //        new ColumnNameProperty<string?>(prop2Name, ["Hallo2"],
    //        conv);

    //    wrapper.Add(prop1);
    //    wrapper.Add(prop2);

    //    Assert.AreEqual(2, wrapper.Count);
    //    Assert.AreEqual(prop1Name, wrapper.PropertyNames.First());
    //    Assert.AreEqual(prop2Name, wrapper.PropertyNames.ElementAt(1));

    //    Assert.IsTrue(wrapper.RemoveProperty(prop1Name));

    //    Assert.AreEqual(1, wrapper.Count);
    //    Assert.AreEqual(prop2Name, wrapper.PropertyNames.First());
    //}


    //[TestMethod()]
    //public void RemovePropertyTest2()
    //{
    //    var wrapper = CsvRecordMapping.Create();
    //    Assert.IsFalse(wrapper.RemoveProperty("bla"));
    //}


    //[TestMethod()]
    //public void RemovePropertyAtTest1()
    //{
    //    var wrapper = CsvRecordMapping.Create();

    //    const string prop1Name = "Prop1";
    //    const string prop2Name = "Prop2";

    //    var conv = StringConverter.CreateNullable();

    //    var prop1 =
    //        new ColumnNameProperty<string?>(prop1Name, ["Hallo1"],
    //        conv);

    //    var prop2 =
    //        new ColumnNameProperty<string?>(prop2Name, ["Hallo2"],
    //        conv);

    //    wrapper.Add(prop1);
    //    wrapper.Add(prop2);

    //    Assert.AreEqual(2, wrapper.Count);
    //    Assert.AreEqual(prop1Name, wrapper.PropertyNames.First());
    //    Assert.AreEqual(prop2Name, wrapper.PropertyNames.ElementAt(1));

    //    wrapper.RemovePropertyAt(0);

    //    Assert.AreEqual(1, wrapper.Count);
    //    Assert.AreEqual(prop2Name, wrapper.PropertyNames.First());
    //}

    //[TestMethod()]
    //[ExpectedException(typeof(ArgumentOutOfRangeException))]
    //public void RemovePropertyAtTest2()
    //{
    //    var wrapper = CsvRecordMapping.Create();
    //    wrapper.RemovePropertyAt(42);
    //}

    [TestMethod()]
    public void IndexerTest()
    {
        var record = new CsvRecord(2);
        record.Values[0] = "42".AsMemory();
        record.Values[1] = "43".AsMemory();

        var wrapper = CsvRecordMapping.Create();

        var intConverter = new Int32Converter();

        wrapper.Add(new ColumnNameProperty<int>(record.ColumnNames[0], [record.ColumnNames[0]], intConverter));
        wrapper.Add(new ColumnNameProperty<int>(record.ColumnNames[1], [record.ColumnNames[1]], intConverter));

        wrapper.Record = record;

        Assert.AreEqual(42, wrapper[0].Value);
        Assert.AreEqual(43, wrapper[1].Value);


        dynamic dyn = wrapper;

        Assert.AreEqual(42, dyn[0].Value);
        Assert.AreEqual(43, dyn[1].Value);

        int test = dyn[0].Value;
        Assert.AreEqual(42, test);

        test = dyn["Column1"].Value;
        Assert.AreEqual(42, test);

        dyn["Column2"].Value = 7;
        Assert.AreEqual(7, dyn["Column2"].Value);

        dyn[0].Value = 3;
        Assert.AreEqual(3, dyn[0].Value);
    }

    [TestMethod()]
    public void ToStringTest()
    {
        var rec = new CsvRecord(3);

        var wrapper = CsvRecordMapping.Create();

        string s = wrapper.ToString();
        Assert.IsNotNull(s);
        Assert.AreNotEqual(0, s.Length);

        wrapper.Record = rec;

        s = wrapper.ToString();
        Assert.IsNotNull(s);
        Assert.AreNotEqual(0, s.Length);

        const string prop1Name = "Prop1";
        const string prop2Name = "Prop2";
        const string prop3Name = "Prop3";

        TypeConverter<int?> conv = new Int32Converter().ToNullableConverter();

        var prop1 =
            new IndexProperty<int?>(prop1Name, 0, conv);

        var prop2 =
            new IndexProperty<int?>(prop2Name, 1, conv);

        var prop3 =
            new IndexProperty<int?>(prop3Name, 2, conv);

        wrapper.Add(prop1);
        wrapper.Add(prop2);
        wrapper.Add(prop3);

        s = wrapper.ToString();
        Assert.IsNotNull(s);
        Assert.AreNotEqual(0, s.Length);

        dynamic dyn = wrapper;

        dyn.Prop1 = 1;
        dyn.Prop2 = 2;
        dyn.Prop3 = 3;

        s = wrapper.ToString();
        Assert.IsNotNull(s);
        Assert.AreNotEqual(0, s.Length);

        rec.Values[0] = "bla".AsMemory();

        s = wrapper.ToString();
        Assert.IsNotNull(s);
        Assert.AreNotEqual(0, s.Length);
    }
}
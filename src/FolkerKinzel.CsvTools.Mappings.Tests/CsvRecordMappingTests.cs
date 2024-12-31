﻿using System;
using System.Collections.Generic;
using System.Globalization;
using FolkerKinzel.CsvTools.Mappings.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Mappings.Tests;

[TestClass()]
public class CsvRecordMappingTests
{
    [TestMethod()]
    public void CsvRecordWrapperTest()
    {
        var wrapper = new CsvRecordMapping();
        Assert.IsNotNull(wrapper);
    }

    [TestMethod()]
    public void InsertPropertyTest1()
    {
        var wrapper = new CsvRecordMapping();

        const string prop1Name = "Prop1";
        const string prop2Name = "Prop2";

        var conv = new StringConverter();
        var prop1 =
            new ColumnNameProperty<string?>(prop1Name, ["Hallo1"],
            conv);

        var prop2 =
            new ColumnNameProperty<string?>(prop2Name, ["Hallo2"],
            conv);

        wrapper.AddProperty(prop2);
        Assert.AreEqual(1, wrapper.Count);
        Assert.AreEqual(prop2Name, wrapper.PropertyNames.First());

        wrapper.InsertProperty(0, prop1);

        Assert.AreEqual(2, wrapper.Count);
        Assert.AreEqual(prop1Name, wrapper.PropertyNames.First());
        Assert.AreEqual(prop2Name, wrapper.PropertyNames.ElementAt(1));
    }

    [TestMethod()]
    public void InsertPropertyTest2()
    {
        var wrapper = new CsvRecordMapping();

        const string prop1Name = "Prop1";


        var prop1 =
            new ColumnNameProperty<string?>(prop1Name, ["Hallo1"],
            new StringConverter());


        wrapper.InsertProperty(0, prop1);
        Assert.AreEqual(1, wrapper.Count);
        Assert.AreEqual(prop1Name, wrapper.PropertyNames.First());
    }

    [TestMethod()]
    [ExpectedException(typeof(ArgumentNullException))]
    public void InsertPropertyTest3()
    {
        var wrapper = new CsvRecordMapping();

        wrapper.InsertProperty(0, null!);
    }


    [TestMethod()]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void InsertPropertyTest4()
    {
        var wrapper = new CsvRecordMapping();

        const string prop1Name = "Prop1";


        var prop1 =
            new ColumnNameProperty<string?>(prop1Name, ["Hallo1"],
            new StringConverter());


        wrapper.InsertProperty(4711, prop1);
    }

    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    public void InsertPropertyTest5()
    {
        var wrapper = new CsvRecordMapping();

        const string prop1Name = "Prop1";

        var conv = new StringConverter();

        var prop1 =
            new ColumnNameProperty<string?>(prop1Name, ["Hallo1"],
            conv);

        var prop2 =
            new ColumnNameProperty<string?>(prop1Name, ["Hallo2"],
            conv);

        wrapper.AddProperty(prop2);
        wrapper.InsertProperty(0, prop1);
    }


    [TestMethod()]
    public void ReplacePropertyAtTest1()
    {
        var wrapper = new CsvRecordMapping();

        const string prop1Name = "Prop1";
        const string prop2Name = "Prop2";
        const string prop3Name = "Prop3";

        var conv = new StringConverter();


        var prop1 =
            new ColumnNameProperty<string?>(prop1Name, ["Hallo1"],
            conv);

        var prop2 =
            new ColumnNameProperty<string?>(prop2Name, ["Hallo2"],
            conv);

        var prop3 =
            new ColumnNameProperty<string?>(prop3Name, ["Hallo3"],
            conv);

        wrapper.AddProperty(prop1);
        wrapper.AddProperty(prop2);

        Assert.AreEqual(2, wrapper.Count);
        Assert.AreEqual(prop1Name, wrapper.PropertyNames.First());
        Assert.AreEqual(prop2Name, wrapper.PropertyNames.ElementAt(1));

        wrapper.ReplacePropertyAt(0, prop3);

        Assert.AreEqual(2, wrapper.Count);
        Assert.AreEqual(prop3Name, wrapper.PropertyNames.First());
        Assert.AreEqual(prop2Name, wrapper.PropertyNames.ElementAt(1));
    }


    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    public void ReplacePropertyAtTest2()
    {
        var wrapper = new CsvRecordMapping();

        const string prop1Name = "Prop1";
        const string prop2Name = "Prop2";

        var conv = new StringConverter();


        var prop1 =
            new ColumnNameProperty<string?>(prop1Name, ["Hallo1"],
            conv);

        var prop2 =
            new ColumnNameProperty<string?>(prop2Name, ["Hallo2"],
            conv);

        var prop3 =
            new ColumnNameProperty<string?>(prop2Name, ["Hallo3"],
            conv);

        wrapper.AddProperty(prop1);
        wrapper.AddProperty(prop2);

        wrapper.ReplacePropertyAt(0, prop3);
    }


    [TestMethod()]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void ReplacePropertyAtTest3()
    {
        var wrapper = new CsvRecordMapping();

        const string prop1Name = "Prop1";
        const string prop2Name = "Prop2";
        const string prop3Name = "Prop3";

        var conv = new StringConverter();

        var prop1 =
            new ColumnNameProperty<string?>(prop1Name, ["Hallo1"],
            conv);

        var prop2 =
            new ColumnNameProperty<string?>(prop2Name, ["Hallo2"],
            conv);

        var prop3 =
            new ColumnNameProperty<string?>(prop3Name, ["Hallo3"],
            conv);

        wrapper.AddProperty(prop1);
        wrapper.AddProperty(prop2);

        wrapper.ReplacePropertyAt(4711, prop3);
    }


    [TestMethod()]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ReplacePropertyAtTest4()
    {
        var wrapper = new CsvRecordMapping();

        const string prop1Name = "Prop1";

        var prop1 =
            new ColumnNameProperty<string?>(prop1Name, ["Hallo1"],
            new StringConverter());


        wrapper.AddProperty(prop1);

        wrapper.ReplacePropertyAt(0, null!);
    }

    [TestMethod()]
    public void ReplacePropertyTest1()
    {
        var wrapper = new CsvRecordMapping();

        const string prop1Name = "Prop1";
        const string prop2Name = "Prop2";
        const string prop3Name = "Prop3";

        var conv = new StringConverter();


        var prop1 =
            new ColumnNameProperty<string?>(prop1Name, ["Hallo1"],
            conv);

        var prop2 =
            new ColumnNameProperty<string?>(prop2Name, ["Hallo2"],
            conv);

        var prop3 =
            new ColumnNameProperty<string?>(prop3Name, ["Hallo3"],
            conv);

        wrapper.AddProperty(prop1);
        wrapper.AddProperty(prop2);

        Assert.AreEqual(2, wrapper.Count);
        Assert.AreEqual(prop1Name, wrapper.PropertyNames.First());
        Assert.AreEqual(prop2Name, wrapper.PropertyNames.ElementAt(1));

        wrapper.ReplaceProperty(prop1Name, prop3);

        Assert.AreEqual(2, wrapper.Count);
        Assert.AreEqual(prop3Name, wrapper.PropertyNames.First());
        Assert.AreEqual(prop2Name, wrapper.PropertyNames.ElementAt(1));
    }

    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    public void ReplacePropertyTest2a()
    {
        var wrapper = new CsvRecordMapping();

        const string prop1Name = "Prop1";
        const string prop2Name = "Prop2";

        var conv = new StringConverter();


        var prop1 =
            new ColumnNameProperty<string?>(prop1Name, ["Hallo1"],
            conv);

        var prop2 =
            new ColumnNameProperty<string?>(prop2Name, ["Hallo2"],
            conv);

        var prop3 =
            new ColumnNameProperty<string?>(prop2Name, ["Hallo3"],
            conv);

        wrapper.AddProperty(prop1);
        wrapper.AddProperty(prop2);

        wrapper.ReplaceProperty(prop1Name, prop3);
    }


    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    public void ReplacePropertyTest2b()
    {
        var wrapper = new CsvRecordMapping();

        const string prop1Name = "Prop1";

        var prop1 =
            new ColumnNameProperty<string?>(prop1Name, ["Hallo1"],
            new StringConverter());

        wrapper.ReplaceProperty("bla", prop1);
    }


    [TestMethod()]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ReplacePropertyTest3()
    {
        var wrapper = new CsvRecordMapping();

        const string prop1Name = "Prop1";

        var prop1 =
            new ColumnNameProperty<string?>(prop1Name, ["Hallo1"],
            new StringConverter());


        wrapper.AddProperty(prop1);

        wrapper.ReplaceProperty(null!, prop1);
    }



    [TestMethod()]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ReplacePropertyTest4()
    {
        var wrapper = new CsvRecordMapping();

        const string prop1Name = "Prop1";

        var prop1 =
            new ColumnNameProperty<string?>(prop1Name, ["Hallo1"],
            new StringConverter());


        wrapper.AddProperty(prop1);

        wrapper.ReplaceProperty(prop1Name, null!);
    }




    [TestMethod()]
    [ExpectedException(typeof(InvalidOperationException))]
    public void TrySetMemberTest()
    {
        var wrapper = new CsvRecordMapping();

        const string prop1Name = "Prop1";

        var prop1 =
            new ColumnNameProperty<int?>(prop1Name, ["Hallo1"],
            new Int32Converter().ToNullableConverter());

        wrapper.AddProperty(prop1);

        dynamic dyn = wrapper;

        dyn.Prop1 = 42;
    }


    [TestMethod()]
    [ExpectedException(typeof(InvalidOperationException))]
    public void TryGetMemberTest()
    {
        var wrapper = new CsvRecordMapping();

        const string prop1Name = "Prop1";

        var prop1 =
            new ColumnNameProperty<int?>(prop1Name, ["Hallo1"],
            new Int32Converter().ToNullableConverter());

        wrapper.AddProperty(prop1);

        dynamic dyn = wrapper;

        _ = dyn.Prop1;
    }


    [DataTestMethod]
    [DataRow(0)]
    [DataRow(ColumnNameProperty<int>.MaxWildcardTimeout)]
    [DataRow(ColumnNameProperty<int>.MaxWildcardTimeout + 1)]
    public void DynPropTest(int wildcardTimeout)
    {
        var rec = new CsvRecord([ "Hallo1", "Blabla" ], false, false, true);

        var wrapper = new CsvRecordMapping
        {
            Record = rec
        };

        const string prop1Name = "Prop1";
        const string prop2Name = "Prop2";


        var prop1 =
            new ColumnNameProperty<int?>(prop1Name, [ "Hallo1" ],
            new Int32Converter().ToNullableConverter());

        wrapper.AddProperty(prop1);

        var prop2 =
            new ColumnNameProperty<string?>(prop2Name, ["Blub", null!, "Bla*"],
            new StringConverter(),
            wildcardTimeout);

        wrapper.AddProperty(prop2);

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

        var wrapper = new CsvRecordMapping
        {
            Record = rec
        };

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

        wrapper.AddProperty(prop1);
        wrapper.AddProperty(prop2);
        wrapper.AddProperty(prop3);

        dynamic dyn = wrapper;

        dyn.Prop1 = 1;
        dyn.Prop2 = 2;
        dyn.Prop3 = 3;


        foreach (KeyValuePair<string, object> kvp in dyn)
        {
            switch (kvp.Key)
            {
                case prop1Name:
                    Assert.AreEqual(1, (int)kvp.Value);
                    break;
                case prop2Name:
                    Assert.AreEqual(2, (int)kvp.Value);
                    break;
                case prop3Name:
                    Assert.AreEqual(3, (int)kvp.Value);
                    break;
                default:
                    Assert.Fail();
                    break;
            }
        }
    }


    [TestMethod()]
    [ExpectedException(typeof(InvalidOperationException))]
    public void GetEnumeratorTest2()
    {
        var wrapper = new CsvRecordMapping();

        foreach (KeyValuePair<string, object?> _ in wrapper)
        {

        }
    }


    [TestMethod()]
    public void IndexOfTest()
    {
        var wrapper = new CsvRecordMapping();

        wrapper.AddProperty(new ColumnNameProperty<string?>("Hallo", ["Hallo"], new StringConverter()));

        Assert.AreEqual(0, wrapper.IndexOf("Hallo"));
        Assert.AreEqual(-1, wrapper.IndexOf("Wolli"));
        Assert.AreEqual(-1, wrapper.IndexOf(null));
        Assert.AreEqual(-1, wrapper.IndexOf(string.Empty));
    }

    [TestMethod()]
    public void ContainsTest()
    {
        var wrapper = new CsvRecordMapping();

        wrapper.AddProperty(new ColumnNameProperty<string?>("Hallo", ["Hallo"], new StringConverter()));

        Assert.IsTrue(wrapper.Contains("Hallo"));
        Assert.IsFalse(wrapper.Contains("Wolli"));
        Assert.IsFalse(wrapper.Contains(null));
        Assert.IsFalse(wrapper.Contains(string.Empty));
    }


    [TestMethod()]
    public void AddPropertyTest1()
    {
        var wrapper = new CsvRecordMapping();

        Assert.AreEqual(0, wrapper.Count);

        var prop =
            new ColumnNameProperty<string?>("Hallo", ["Hallo"],
            new StringConverter());

        wrapper.AddProperty(prop);

        Assert.AreEqual(1, wrapper.Count);
    }


    [TestMethod()]
    [ExpectedException(typeof(ArgumentNullException))]
    public void AddPropertyTest2()
    {
        var wrapper = new CsvRecordMapping();

        wrapper.AddProperty(null!);
    }


    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    public void AddPropertyTest3()
    {
        var wrapper = new CsvRecordMapping();

        var conv = new StringConverter();

        var prop1 =
            new ColumnNameProperty<string?>("Hallo", ["Hallo"],
            conv);

        var prop2 =
            new ColumnNameProperty<string?>("Hallo", ["Hallo"],
            conv);

        wrapper.AddProperty(prop1);
        wrapper.AddProperty(prop2);
    }


    [TestMethod()]
    public void RemovePropertyTest1()
    {
        var wrapper = new CsvRecordMapping();

        const string prop1Name = "Prop1";
        const string prop2Name = "Prop2";

        var conv = new StringConverter();


        var prop1 =
            new ColumnNameProperty<string?>(prop1Name, ["Hallo1"],
            conv);

        var prop2 =
            new ColumnNameProperty<string?>(prop2Name, ["Hallo2"],
            conv);

        wrapper.AddProperty(prop1);
        wrapper.AddProperty(prop2);

        Assert.AreEqual(2, wrapper.Count);
        Assert.AreEqual(prop1Name, wrapper.PropertyNames.First());
        Assert.AreEqual(prop2Name, wrapper.PropertyNames.ElementAt(1));

        Assert.IsTrue(wrapper.RemoveProperty(prop1Name));

        Assert.AreEqual(1, wrapper.Count);
        Assert.AreEqual(prop2Name, wrapper.PropertyNames.First());
    }


    [TestMethod()]
    public void RemovePropertyTest2()
    {
        var wrapper = new CsvRecordMapping();
        Assert.IsFalse(wrapper.RemoveProperty("bla"));
    }


    [TestMethod()]
    public void RemovePropertyAtTest1()
    {
        var wrapper = new CsvRecordMapping();

        const string prop1Name = "Prop1";
        const string prop2Name = "Prop2";

        var conv = new StringConverter();

        var prop1 =
            new ColumnNameProperty<string?>(prop1Name, ["Hallo1"],
            conv);

        var prop2 =
            new ColumnNameProperty<string?>(prop2Name, ["Hallo2"],
            conv);

        wrapper.AddProperty(prop1);
        wrapper.AddProperty(prop2);

        Assert.AreEqual(2, wrapper.Count);
        Assert.AreEqual(prop1Name, wrapper.PropertyNames.First());
        Assert.AreEqual(prop2Name, wrapper.PropertyNames.ElementAt(1));

        wrapper.RemovePropertyAt(0);

        Assert.AreEqual(1, wrapper.Count);
        Assert.AreEqual(prop2Name, wrapper.PropertyNames.First());
    }

    [TestMethod()]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void RemovePropertyAtTest2()
    {
        var wrapper = new CsvRecordMapping();
        wrapper.RemovePropertyAt(42);
    }

    [TestMethod()]
    public void IndexerTest()
    {
        var record = new CsvRecord(2);
        record.Values[0] = "42".AsMemory();
        record.Values[1] = "43".AsMemory();

        var wrapper = new CsvRecordMapping();

        var intConverter = new Int32Converter();

        wrapper.AddProperty(new ColumnNameProperty<int>(record.ColumnNames[0], [record.ColumnNames[0]], intConverter));
        wrapper.AddProperty(new ColumnNameProperty<int>(record.ColumnNames[1], [record.ColumnNames[1]], intConverter));

        wrapper.Record = record;

        Assert.AreEqual(42, wrapper[0]);
        Assert.AreEqual(43, wrapper[1]);


        dynamic dyn = wrapper;

        Assert.AreEqual(42, dyn[0]);
        Assert.AreEqual(43, dyn[1]);

        int test = dyn[0];
        Assert.AreEqual(42, test);

        test = dyn["Column1"];
        Assert.AreEqual(42, test);

        dyn["Column2"] = 7;
        Assert.AreEqual(7, dyn["Column2"]);

        dyn[0] = 3;
        Assert.AreEqual(3, dyn[0]);
    }

    [TestMethod()]
    public void ToStringTest()
    {
        var rec = new CsvRecord(3);

        var wrapper = new CsvRecordMapping();

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

        wrapper.AddProperty(prop1);
        wrapper.AddProperty(prop2);
        wrapper.AddProperty(prop3);

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
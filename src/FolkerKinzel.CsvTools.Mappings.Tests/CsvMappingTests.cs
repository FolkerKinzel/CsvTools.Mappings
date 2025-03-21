﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using FolkerKinzel.CsvTools.Mappings.TypeConverters;
using FolkerKinzel.CsvTools.Mappings.Intls.DynamicProperties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Mappings.Tests;

[TestClass()]
public class CsvMappingTests
{
    [TestMethod()]
    [ExpectedException(typeof(InvalidOperationException))]
    public void TrySetMemberTest1()
    {
        CsvMapping mapping = CsvMappingBuilder.Create().Build();

        const string prop1Name = "Prop1";

        var prop1 =
            new ColumnNameProperty<int?>(prop1Name, ["Hallo1"],
            new Int32Converter().ToNullableConverter());

        mapping.AddProperty(prop1);

        dynamic dyn = mapping;

        dyn.Prop1 = 42;
    }

    [TestMethod]
    [ExpectedException(typeof(Microsoft.CSharp.RuntimeBinder.RuntimeBinderException))]
    public void TrySetMemberTest2()
    {
        dynamic dyn = new CsvMapping();
        dyn.Property = 42;
    }

    [TestMethod()]
    [ExpectedException(typeof(InvalidOperationException))]
    public void TryGetMemberTest()
    {
        CsvMapping wrapper = CsvMappingBuilder.Create().Build();

        const string prop1Name = "Prop1";

        var prop1 =
            new ColumnNameProperty<int?>(prop1Name, ["Hallo1"],
            new Int32Converter().ToNullableConverter());

        wrapper.AddProperty(prop1);

        dynamic dyn = wrapper;

        _ = dyn.Prop1;
    }

    [TestMethod]
    [ExpectedException(typeof(Microsoft.CSharp.RuntimeBinder.RuntimeBinderException))]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "<Pending>")]
    public void TryGetMemberTest2()
    {
        dynamic dyn = new CsvMapping();
        int i = dyn.Property;
    }

    [TestMethod]
    public void DynPropTest()
    {
        const string prop1Name = "Prop1";
        const string prop2Name = "Prop2";

        CsvRecord rec = Utility.CreateCsvRecord(["Hallo1", "Blabla"]);

        CsvMapping mapping = CsvMappingBuilder
            .Create()
            .AddProperty(prop1Name, ["Hallo1"], new Int32Converter().ToNullableConverter())
            .AddProperty(prop2Name, ["Blub", null!, "Bla*"], StringConverter.CreateNullable())
            .Build();
        mapping.Record = rec;

        dynamic dyn = mapping;

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
        CsvRecord rec = Utility.CreateCsvRecord(3);

        CsvMapping wrapper = CsvMappingBuilder.Create().Build();

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

        wrapper.Record = rec;

        dynamic dyn = wrapper;

        dyn.Prop1 = 1;
        dyn.Prop2 = 2;
        dyn.Prop3 = 3;

        foreach (DynamicProperty kvp in dyn)
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

    [TestMethod()]
    public void ContainsTest()
    {
        CsvMapping mapping =
            CsvMappingBuilder.Create().AddProperty("Hallo", StringConverter.CreateNullable()).Build();

        Assert.IsTrue(mapping.Contains("Hallo"));
        Assert.IsFalse(mapping.Contains("Wolli"));
        Assert.IsFalse(mapping.Contains(string.Empty));
    }

    [TestMethod()]
    public void AddPropertyTest1()
    {
        CsvMapping mapping = CsvMappingBuilder.Create().Build();

        Assert.AreEqual(0, mapping.Count);

        var prop =
            new ColumnNameProperty<string?>("Hallo", ["Hallo"],
            StringConverter.CreateNullable());

        mapping.AddProperty(prop);

        Assert.AreEqual(1, mapping.Count);
    }

    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    public void AddPropertyTest3()
    {
        CsvMapping mapping = CsvMappingBuilder.Create().Build();

        TypeConverter<string?> conv = StringConverter.CreateNullable();

        var prop1 =
            new ColumnNameProperty<string?>("Hallo", ["Hallo"],
            conv);

        var prop2 =
            new ColumnNameProperty<string?>("Hallo", ["Hallo"],
            conv);

        mapping.AddProperty(prop1);
        mapping.AddProperty(prop2);
    }

    [TestMethod()]
    public void IndexerTest()
    {
        CsvRecord record = Utility.CreateCsvRecord(2);
        record.Values[0] = "42".AsMemory();
        record.Values[1] = "43".AsMemory();

        CsvMapping mapping = CsvMappingBuilder.Create().Build();

        var intConverter = new Int32Converter();

        mapping.AddProperty(new ColumnNameProperty<int>(record.ColumnNames[0], [record.ColumnNames[0]], intConverter));
        mapping.AddProperty(new ColumnNameProperty<int>(record.ColumnNames[1], [record.ColumnNames[1]], intConverter));

        mapping.Record = record;

        Assert.AreEqual(42, mapping[0].Value);
        Assert.AreEqual(43, mapping[1].Value);

        dynamic dyn = mapping;

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
        CsvRecord rec = Utility.CreateCsvRecord(3);

        CsvMapping mapping = CsvMappingBuilder.Create().Build();

        string s = mapping.ToString();
        Assert.IsNotNull(s);
        Assert.AreNotEqual(0, s.Length);

        const string prop1Name = "Prop1";
        const string prop2Name = "Prop2";
        const string prop3Name = "Prop3";

        TypeConverter<int?> conv = new Int32Converter().ToNullableConverter();

        var prop1 =
            new IndexProperty<int?>(prop1Name, 0, conv);

        var prop2 =
            new IndexProperty<object>(prop2Name, 1, conv.ToDBNullConverter());

        var prop3 =
            new IndexProperty<int?>(prop3Name, 2, conv);

        mapping.AddProperty(prop1);
        mapping.AddProperty(prop2);
        mapping.AddProperty(prop3);

        mapping.Record = rec;

        s = mapping.ToString();
        Assert.IsNotNull(s);
        Assert.AreNotEqual(0, s.Length);

        dynamic dyn = mapping;

        dyn.Prop1 = 1;
        dyn.Prop2 = 2;
        dyn.Prop3 = 3;

        s = mapping.ToString();
        Assert.IsNotNull(s);
        Assert.AreNotEqual(0, s.Length);

        rec.Values[0] = "bla".AsMemory();

        s = mapping.ToString();
        Assert.IsNotNull(s);
        Assert.AreNotEqual(0, s.Length);
    }

    [TestMethod]
    public void RegexTimeoutTest1() => CsvMapping.RegexTimeout = 42;

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void RegexTimeoutTest2() => CsvMapping.RegexTimeout = 0;

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void RegexTimeoutTest3() => CsvMapping.RegexTimeout = -17;

    [TestMethod]
    public void RegexTimeoutTest4() => CsvMapping.RegexTimeout = Timeout.Infinite;

    [TestMethod]
    public void PropertyNamesTest1() => Assert.AreEqual(0, CsvMappingBuilder.Create().Build().PropertyNames.Count());
}
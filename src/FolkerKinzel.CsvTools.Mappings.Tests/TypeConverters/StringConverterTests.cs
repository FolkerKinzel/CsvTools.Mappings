﻿using FolkerKinzel.CsvTools.Mappings.TypeConverters.Interfaces;
using FolkerKinzel.CsvTools.Mappings.Intls.DynamicProperties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FolkerKinzel.CsvTools.Mappings.TypeConverters.Tests;

[TestClass()]
public class StringConverterTests
{
    [TestMethod()]
    public void StringConverterTest()
    {
        TypeConverter<string> conv = StringConverter.CreateNonNullable();

        Assert.IsNotNull(conv.Parse(null));
        Assert.IsInstanceOfType<ITypeConverter<string?>>(conv);
        Assert.IsTrue(conv.AcceptsNull);
    }

    [TestMethod()]
    public void ParseTest()
    {
        TypeConverter<string?> conv = StringConverter.CreateNullable();
        Assert.IsTrue(conv.AcceptsNull);

        Assert.IsNull(conv.Parse(null));

        TypeConverter<string> conv2 = StringConverter.CreateNonNullable();

        Assert.IsNotNull(conv2.Parse(null));

        const string test = "Test";

        Assert.AreEqual(test, conv.Parse(test.AsSpan()));

    }

    [TestMethod()]
    public void ConvertToStringTest()
    {
        TypeConverter<object> conv = StringConverter.CreateNullable().ToDBNullConverter();

        Assert.IsNull(conv.ConvertToString(DBNull.Value));

        Assert.IsNull(conv.ConvertToString(null!));

        const string test = "Test";

        Assert.AreEqual(test, conv.ConvertToString(test));
    }

    [ExpectedException(typeof(InvalidCastException))]
    [TestMethod()]
    public void ConvertToStringTest_ThrowOnInvalidType()
    {
        TypeConverter<string> conv = StringConverter.CreateNonNullable();

        new IndexProperty<string>("prop", 0,  conv).Value = 4711;
    }

    [ExpectedException(typeof(InvalidCastException))]
    [TestMethod()]
    public void ConvertToStringTest_ThrowOnDBNull()
        => new IndexProperty<string?>("prop",0, StringConverter.CreateNullable()).Value = DBNull.Value;
}
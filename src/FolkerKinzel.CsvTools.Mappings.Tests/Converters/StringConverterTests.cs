using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FolkerKinzel.CsvTools.Mappings.Converters.Tests;

[TestClass()]
public class StringConverterTests
{
    [TestMethod()]
    public void StringConverterTest()
    {
        var conv = new StringConverter(false);

        Assert.IsNotNull(conv.Parse(null));
        Assert.AreEqual(typeof(StringConverter), conv.GetType());
    }

    [TestMethod()]
    public void ParseTest()
    {
        var conv = new StringConverter();

        Assert.IsNull(conv.Parse(null));

        conv = new StringConverter(false);

        Assert.IsNotNull(conv.Parse(null));

        const string test = "Test";

        Assert.AreEqual(test, conv.Parse(test.AsSpan()));

    }

    [TestMethod()]
    public void ConvertToStringTest()
    {
        CsvTypeConverter<object> conv = new StringConverter().AsDBNullEnabled();

        Assert.IsNull(conv.ConvertToString(DBNull.Value));

        Assert.IsNull(conv.ConvertToString(null!));

        const string test = "Test";

        Assert.AreEqual(test, conv.ConvertToString(test));
    }

    [ExpectedException(typeof(InvalidCastException))]
    [TestMethod()]
    public void ConvertToStringTest_ThrowOnInvalidType()
    {
        var conv = new StringConverter();

        new CsvIndexProperty<string?>("prop", 0,  conv).SetValue(4711);
    }


    [ExpectedException(typeof(InvalidCastException))]
    [TestMethod()]
    public void ConvertToStringTest_ThrowOnDBNull()
        => new CsvIndexProperty<string?>("prop",0, new StringConverter()).SetValue(DBNull.Value);
}
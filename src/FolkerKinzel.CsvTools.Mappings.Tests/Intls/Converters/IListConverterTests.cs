using FolkerKinzel.CsvTools.Mappings.TypeConverters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;

namespace FolkerKinzel.CsvTools.Mappings.Intls.Converters.Tests;

[TestClass()]
public class IListConverterTests
{
    [TestMethod]
    public void IListConverterTest1()
    {
        TypeConverter<IList<int>?> conv = new Int32Converter().ToIListConverter("::");
        Assert.IsNotNull(conv);
        Assert.IsTrue(conv.AcceptsNull);
        Assert.IsNull(conv.DefaultValue);
    }

    [TestMethod]
    public void IListConverterTest2()
    {
        TypeConverter<IList<int>?> conv = new Int32Converter().ToIListConverter("::", false);
        Assert.IsNotNull(conv);
        Assert.IsTrue(conv.AcceptsNull);
        Assert.IsNotNull(conv.DefaultValue);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void IListConverterTest3() => _ = ((Int32Converter?)null)!.ToIListConverter("::");

    [TestMethod]
    public void ConvertToStringTest1()
    {
        TypeConverter<IList<int>?> conv = new Int32Converter().ToIListConverter("::", false);
        Assert.IsNull(conv.ConvertToString(null));
        Assert.IsNull(conv.ConvertToString([]));
        Assert.AreEqual("1::2::3", conv.ConvertToString([1, 2, 3]));
    }

    [TestMethod]
    public void TryParseTest1()
    {
        TypeConverter<IList<int>?> conv = new Int32Converter().ToIListConverter("::", false);
        Assert.IsTrue(conv.TryParse("1::2::3".AsSpan(), out IList<int>? result));
        Assert.IsNotNull(result);
        CollectionAssert.AreEqual(new int[] { 1, 2, 3 }, result.ToArray());
    }
}
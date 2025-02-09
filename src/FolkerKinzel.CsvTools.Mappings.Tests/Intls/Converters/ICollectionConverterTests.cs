using FolkerKinzel.CsvTools.Mappings.TypeConverters;

namespace FolkerKinzel.CsvTools.Mappings.Intls.Converters.Tests;

[TestClass()]
public class ICollectionConverterTests
{
    [TestMethod]
    public void ICollectionConverterTest1()
    {
        TypeConverter<ICollection<int>?> conv = new Int32Converter().ToICollectionConverter("::");
        Assert.IsNotNull(conv);
        Assert.IsTrue(conv.AcceptsNull);
        Assert.IsNull(conv.DefaultValue);
    }

    [TestMethod]
    public void ICollectionConverterTest2()
    {
        TypeConverter<ICollection<int>?> conv = new Int32Converter().ToICollectionConverter("::", false);
        Assert.IsNotNull(conv);
        Assert.IsTrue(conv.AcceptsNull);
        Assert.IsNotNull(conv.DefaultValue);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ICollectionConverterTest3() => _ = ((Int32Converter?)null)!.ToICollectionConverter("::");

    [TestMethod]
    public void ConvertToStringTest1()
    {
        TypeConverter<ICollection<int>?> conv = new Int32Converter().ToICollectionConverter("::", false);
        Assert.IsNull(conv.ConvertToString(null));
        Assert.IsNull(conv.ConvertToString([]));
        Assert.AreEqual("1::2::3", conv.ConvertToString([1, 2, 3]));
    }

    [TestMethod]
    public void TryParseTest1()
    {
        TypeConverter<ICollection<int>?> conv = new Int32Converter().ToICollectionConverter("::", false);
        Assert.IsTrue(conv.TryParse("1::2::3".AsSpan(), out ICollection<int>? result));
        Assert.IsNotNull(result);
        CollectionAssert.AreEqual(new int[] { 1, 2, 3 }, result.ToArray());
    }
}


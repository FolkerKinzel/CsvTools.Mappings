using FolkerKinzel.CsvTools.Mappings.TypeConverters;

namespace FolkerKinzel.CsvTools.Mappings.Intls.TypeConverters.Tests;

[TestClass()]
public class IReadOnlyCollectionConverterTests
{
    [TestMethod]
    public void IReadOnlyCollectionConverterTest1()
    {
        TypeConverter<IReadOnlyCollection<int>?> conv = new Int32Converter().ToIReadOnlyCollectionConverter("::");
        Assert.IsNotNull(conv);
        Assert.IsTrue(conv.AcceptsNull);
        Assert.IsNull(conv.DefaultValue);
    }

    [TestMethod]
    public void IReadOnlyCollectionConverterTest2()
    {
        TypeConverter<IReadOnlyCollection<int>?> conv = new Int32Converter().ToIReadOnlyCollectionConverter("::", false);
        Assert.IsNotNull(conv);
        Assert.IsTrue(conv.AcceptsNull);
        Assert.IsNotNull(conv.DefaultValue);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void IReadOnlyCollectionConverterTest3() => _ = ((Int32Converter?)null)!.ToIReadOnlyCollectionConverter("::");

    [TestMethod]
    public void ConvertToStringTest1()
    {
        TypeConverter<IReadOnlyCollection<int>?> conv = new Int32Converter().ToIReadOnlyCollectionConverter("::", false);
        Assert.IsNull(conv.ConvertToString(null));
        Assert.IsNull(conv.ConvertToString([]));
        Assert.AreEqual("1::2::3", conv.ConvertToString([1, 2, 3]));
    }

    [TestMethod]
    public void TryParseTest1()
    {
        TypeConverter<IReadOnlyCollection<int>?> conv = new Int32Converter().ToIReadOnlyCollectionConverter("::", false);
        Assert.IsTrue(conv.TryParse("1::2::3".AsSpan(), out IReadOnlyCollection<int>? result));
        Assert.IsNotNull(result);
        CollectionAssert.AreEqual(new int[] { 1, 2, 3 }, result.ToArray());
    }
}


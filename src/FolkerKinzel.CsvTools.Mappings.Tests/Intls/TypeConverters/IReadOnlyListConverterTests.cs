using FolkerKinzel.CsvTools.Mappings.TypeConverters;

namespace FolkerKinzel.CsvTools.Mappings.Intls.TypeConverters.Tests;

[TestClass()]
public class IReadOnlyListConverterTests
{
    [TestMethod]
    public void IReadOnlyListConverterTest1()
    {
        TypeConverter<IReadOnlyList<int>?> conv = new Int32Converter().ToIReadOnlyListConverter("::");
        Assert.IsNotNull(conv);
        Assert.IsTrue(conv.AcceptsNull);
        Assert.IsNull(conv.DefaultValue);
    }

    [TestMethod]
    public void IReadOnlyListConverterTest2()
    {
        TypeConverter<IReadOnlyList<int>?> conv = new Int32Converter().ToIReadOnlyListConverter("::", false);
        Assert.IsNotNull(conv);
        Assert.IsTrue(conv.AcceptsNull);
        Assert.IsNotNull(conv.DefaultValue);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void IReadOnlyListConverterTest3() => _ = ((Int32Converter?)null)!.ToIReadOnlyListConverter("::");

    [TestMethod]
    public void ConvertToStringTest1()
    {
        TypeConverter<IReadOnlyList<int>?> conv = new Int32Converter().ToIReadOnlyListConverter("::", false);
        Assert.IsNull(conv.ConvertToString(null));
        Assert.IsNull(conv.ConvertToString([]));
        Assert.AreEqual("1::2::3", conv.ConvertToString([1, 2, 3]));
    }

    [TestMethod]
    public void TryParseTest1()
    {
        TypeConverter<IReadOnlyList<int>?> conv = new Int32Converter().ToIReadOnlyListConverter("::", false);
        Assert.IsTrue(conv.TryParse("1::2::3".AsSpan(), out IReadOnlyList<int>? result));
        Assert.IsNotNull(result);
        CollectionAssert.AreEqual(new int[] { 1, 2, 3 }, result.ToArray());
    }
}


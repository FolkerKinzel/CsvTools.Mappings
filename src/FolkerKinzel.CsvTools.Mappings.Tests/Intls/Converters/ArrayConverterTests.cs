using FolkerKinzel.CsvTools.Mappings.TypeConverters;

namespace FolkerKinzel.CsvTools.Mappings.Intls.Converters.Tests;

[TestClass()]
public class ArrayConverterTests
{
    [TestMethod]
    public void ArrayConverterTest1()
    {
        TypeConverter<int[]?> conv = new Int32Converter().ToArrayConverter("::");
        Assert.IsNotNull(conv);
        Assert.IsTrue(conv.AcceptsNull);
        Assert.IsNull(conv.DefaultValue);
    }

    [TestMethod]
    public void ArrayConverterTest2()
    {
        TypeConverter<int[]?> conv = new Int32Converter().ToArrayConverter("::", false);
        Assert.IsNotNull(conv);
        Assert.IsTrue(conv.AcceptsNull);
        Assert.IsNotNull(conv.DefaultValue);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ArrayConverterTest3() => _ = ((Int32Converter?)null)!.ToArrayConverter("::");

    [TestMethod]
    public void ConvertToStringTest1()
    {
        TypeConverter<int[]?> conv = new Int32Converter().ToArrayConverter("::", false);
        Assert.IsNull(conv.ConvertToString(null));
        Assert.IsNull(conv.ConvertToString([]));
        Assert.AreEqual("1::2::3", conv.ConvertToString([1, 2, 3]));
    }

    [TestMethod]
    public void TryParseTest1()
    {
        TypeConverter<int[]?> conv = new Int32Converter().ToArrayConverter("::", false);
        Assert.IsTrue(conv.TryParse("1::2::3".AsSpan(), out int[]? result));
        Assert.IsNotNull(result);
        CollectionAssert.AreEqual(new int[] { 1, 2, 3 }, result);
    }
}


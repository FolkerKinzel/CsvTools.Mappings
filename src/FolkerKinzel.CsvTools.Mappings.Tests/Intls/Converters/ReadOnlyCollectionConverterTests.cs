using FolkerKinzel.CsvTools.Mappings.TypeConverters;
using System.Collections.ObjectModel;

namespace FolkerKinzel.CsvTools.Mappings.Intls.Converters.Tests;

[TestClass()]
public class ReadOnlyCollectionConverterTests
{
    [TestMethod]
    public void ReadOnlyCollectionConverterTest1()
    {
        TypeConverter<ReadOnlyCollection<int>?> conv = new Int32Converter().ToReadOnlyCollectionConverter("::");
        Assert.IsNotNull(conv);
        Assert.IsTrue(conv.AcceptsNull);
        Assert.IsNull(conv.DefaultValue);
    }

    [TestMethod]
    public void ReadOnlyCollectionConverterTest2()
    {
        TypeConverter<ReadOnlyCollection<int>?> conv = new Int32Converter().ToReadOnlyCollectionConverter("::", false);
        Assert.IsNotNull(conv);
        Assert.IsTrue(conv.AcceptsNull);
        Assert.IsNotNull(conv.DefaultValue);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ReadOnlyCollectionConverterTest3() => _ = ((Int32Converter?)null)!.ToReadOnlyCollectionConverter("::");

    [TestMethod]
    public void ConvertToStringTest1()
    {
        TypeConverter<ReadOnlyCollection<int>?> conv = new Int32Converter().ToReadOnlyCollectionConverter("::", false);
        Assert.IsNull(conv.ConvertToString(null));
        Assert.IsNull(conv.ConvertToString(new ReadOnlyCollection<int>([])));
        Assert.AreEqual("1::2::3", conv.ConvertToString(new ReadOnlyCollection<int>([1, 2, 3])));
    }

    [TestMethod]
    public void TryParseTest1()
    {
        TypeConverter<ReadOnlyCollection<int>?> conv = new Int32Converter().ToReadOnlyCollectionConverter("::", false);
        Assert.IsTrue(conv.TryParse("1::2::3".AsSpan(), out ReadOnlyCollection<int>? result));
        Assert.IsNotNull(result);
        CollectionAssert.AreEqual(new int[] { 1, 2, 3 }, result.ToArray());
    }
}


using FolkerKinzel.CsvTools.Mappings.TypeConverters;

namespace FolkerKinzel.CsvTools.Mappings.Intls.TypeConverters.Tests;

[TestClass()]
public class ListConverterTests
{
    [TestMethod]
    public void ListConverterTest1()
    {
        var conv = new Int32Converter().ToListConverter("::");
        Assert.IsNotNull(conv);
        Assert.IsTrue(conv.AcceptsNull);
        Assert.IsNull(conv.DefaultValue);
    }

    [TestMethod]
    public void ListConverterTest2()
    {
        var conv = new Int32Converter().ToListConverter("::", false);
        Assert.IsNotNull(conv);
        Assert.IsTrue(conv.AcceptsNull);
        Assert.IsNotNull(conv.DefaultValue);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ListConverterTest3() => _ = ((Int32Converter?)null)!.ToListConverter("::");

    [TestMethod]
    public void ConvertToStringTest1()
    {
        var conv = new Int32Converter().ToListConverter("::", false);
        Assert.IsNull(conv.ConvertToString(null));
        Assert.IsNull(conv.ConvertToString([]));
        Assert.AreEqual("1::2::3", conv.ConvertToString([1,2,3]));
    }
}

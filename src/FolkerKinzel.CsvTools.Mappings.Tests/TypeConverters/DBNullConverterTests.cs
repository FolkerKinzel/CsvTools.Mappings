using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Mappings.TypeConverters.Tests;

[TestClass]
public class DBNullConverterTests
{
    [TestMethod]
    public void DBNullConverterTest1()
    {
        TypeConverter<object> conv1 = new Int32Converter(throwing: false)
                                      .ToNullableConverter()
                                      .ToDBNullConverter();
        Assert.AreEqual(conv1.DataType, typeof(object));
        Assert.IsTrue(Convert.IsDBNull(conv1.DefaultValue));
        Assert.IsFalse(conv1.AcceptsNull);
        Assert.IsTrue(conv1.TryParse("42".AsSpan(), out object? result));
        Assert.IsInstanceOfType<int>(result);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void DBNullConverterTest() => _ = new Int32Converter().ToDBNullConverter();

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void DBNullConverterTest3()
    {
        TypeConverter<int>? conv1 = null;
        _ = conv1!.ToDBNullConverter();
    }
}

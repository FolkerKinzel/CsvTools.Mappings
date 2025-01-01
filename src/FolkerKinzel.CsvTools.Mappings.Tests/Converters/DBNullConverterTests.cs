using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Mappings.Converters.Tests;

[TestClass]
public class DBNullConverterTests
{
    [TestMethod]
    public void DBNullConverterTest1()
    {
        TypeConverter<int> conv1 = new Int32Converter();
        TypeConverter<object> conv2 = conv1.ToDBNullConverter();
        Assert.AreNotSame<object>(conv1, conv2);

        Assert.AreEqual(conv2.DataType, typeof(object));
        Assert.IsTrue(Convert.IsDBNull(conv2.FallbackValue));

        Assert.AreSame(conv2, conv2.ToDBNullConverter());
    }
}

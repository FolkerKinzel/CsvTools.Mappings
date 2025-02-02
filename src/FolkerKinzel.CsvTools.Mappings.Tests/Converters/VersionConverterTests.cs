namespace FolkerKinzel.CsvTools.Mappings.Converters.Tests;

[TestClass]
public class VersionConverterTests
{
    [TestMethod]
    public void CreateNonNullableTest1()
    {
        TypeConverter<Version> conv = VersionConverter.CreateNonNullable();
        Assert.IsNotNull(conv);
        Assert.IsInstanceOfType<Version>(conv.DefaultValue);
    }

    [TestMethod]
    public void CreateNullableTest2()
    {
        TypeConverter<Version?> conv = VersionConverter.CreateNullable();
        Assert.IsNotNull(conv);
        Assert.IsNull(conv.DefaultValue);
    }
}

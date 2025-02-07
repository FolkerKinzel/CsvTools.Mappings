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
        Assert.IsTrue(conv.AcceptsNull);

        Assert.AreEqual("4.3.2.1", conv.ConvertToString(new Version(4, 3, 2, 1)));
        Assert.IsNull(conv.ConvertToString(null!));
    }

    [TestMethod]
    public void CreateNullableTest2()
    {
        TypeConverter<Version?> conv = VersionConverter.CreateNullable();
        Assert.IsNotNull(conv);
        Assert.IsNull(conv.DefaultValue);

        Assert.IsTrue(conv.TryParseValue("  ".AsSpan(), out _));
        Assert.IsFalse(conv.TryParseValue("bla".AsSpan(), out _));
        Assert.IsTrue(conv.TryParseValue("4.3.2.1".AsSpan(), out Version? version));
        Assert.AreEqual(new Version(4, 3, 2, 1), version);
    }
}

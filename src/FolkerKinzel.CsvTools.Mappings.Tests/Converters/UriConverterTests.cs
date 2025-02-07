using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Mappings.Converters.Tests;

[TestClass]
public class UriConverterTests
{
    [TestMethod]
    public void CreateNonNullableTest2()
    {
        const string uriString = "http://www.example.com/";
        TypeConverter<Uri> conv = UriConverter.CreateNonNullable();
        Assert.IsTrue(conv.AcceptsNull);
        Assert.IsNotNull(conv.DefaultValue);

        Uri? uri = conv.Parse("   ".AsSpan());
        Assert.IsNotNull(uri);
        Assert.AreSame(conv.DefaultValue, uri);

        uri = conv.Parse(uriString.AsSpan());
        Assert.IsNotNull(uri);
        Assert.IsTrue(uri.IsAbsoluteUri);
        Assert.AreEqual(uriString, uri.AbsoluteUri);
        Assert.AreEqual(uriString, conv.ConvertToString(uri));
    }

    [TestMethod]
    public void CreateNonNullableTest1()
    {
        TypeConverter<Uri> conv = UriConverter.CreateNonNullable();
        Assert.IsNotNull(conv);
        Assert.IsInstanceOfType<Uri>(conv.DefaultValue);
    }

    [TestMethod]
    public void CreateNullableTest2()
    {
        TypeConverter<Uri?> conv = UriConverter.CreateNullable();
        Assert.IsNotNull(conv);
        Assert.IsNull(conv.DefaultValue);
    }
}

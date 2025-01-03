using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Mappings.Converters.Tests;

[TestClass]
public class UriConverterTests
{
    [TestMethod]
    public void CreateNonNullableTest1()
    {
        const string uriString = "http://www.example.com/";
        TypeConverter<Uri> conv = UriConverter.CreateNonNullable();

        Uri? uri = conv.Parse("   ".AsSpan());
        Assert.IsNotNull(uri);
        Assert.AreSame(conv.FallbackValue, uri);

        uri = conv.Parse(uriString.AsSpan());
        Assert.IsNotNull(uri);
        Assert.IsTrue(uri.IsAbsoluteUri);
        Assert.AreEqual(uriString, uri.AbsoluteUri);
        Assert.AreEqual(uriString, conv.ConvertToString(uri));
    }
}

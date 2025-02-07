using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Mappings.Converters.Tests;

[TestClass]
public class UriConverterTests
{

    [TestMethod]
    public void CreateNonNullableTest1()
    {
        TypeConverter<Uri> conv = UriConverter.CreateNonNullable(new Uri("", UriKind.Relative));
        Assert.IsNotNull(conv);
        Assert.IsInstanceOfType<Uri>(conv.DefaultValue);
    }

    [TestMethod]
    public void CreateNonNullableTest2()
    {
        const string uriString = "http://www.example.com/";
        TypeConverter<Uri> conv = UriConverter.CreateNonNullable(new Uri("", UriKind.Relative));
        Assert.IsTrue(conv.AcceptsNull);
        Assert.IsNotNull(conv.DefaultValue);

        Uri? uri = conv.Parse("   ".AsSpan());
        Assert.IsNotNull(uri);
        Assert.AreEqual(conv.DefaultValue, uri);
        Assert.AreNotSame(conv.DefaultValue, uri);

        uri = conv.Parse(uriString.AsSpan());
        Assert.IsNotNull(uri);
        Assert.IsTrue(uri.IsAbsoluteUri);
        Assert.AreEqual(uriString, uri.AbsoluteUri);
        Assert.AreEqual(uriString, conv.ConvertToString(uri));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CreateNonNullableTest3() => _ = UriConverter.CreateNonNullable(null!);

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateNonNullableTest4() => _ = UriConverter.CreateNonNullable(new Uri("http://folker.com/", UriKind.Absolute), UriKind.Relative);

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateNonNullableTest5() => _ = UriConverter.CreateNonNullable(new Uri("../relative.htm", UriKind.Relative), UriKind.Absolute);

    [TestMethod]
    public void CreateNullableTest2()
    {
        TypeConverter<Uri?> conv = UriConverter.CreateNullable();
        Assert.IsNotNull(conv);
        Assert.IsNull(conv.DefaultValue);
        Assert.IsNull(conv.ConvertToString(null));
        string absoluteUriString = "http://folker.com/";
        Assert.AreEqual(absoluteUriString, conv.ConvertToString(new Uri(absoluteUriString)));
        string relativeUriString = "../relative.htm";
        Assert.AreEqual(relativeUriString, conv.ConvertToString( new Uri(relativeUriString, UriKind.Relative)));
    }
}

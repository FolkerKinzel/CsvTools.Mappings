namespace FolkerKinzel.CsvTools.Mappings.Converters.Tests;

[TestClass()]
public class DateOnlyConverterTests
{
    private readonly DateOnlyConverter _conv = new();

    [TestMethod()]
    public void DateOnlyConverterTest1()
    {
        Assert.IsNotNull(_conv);
        Assert.IsFalse(_conv.AllowsNull);
        Assert.IsFalse(_conv.ParseExact);
    }

    [TestMethod]
    public void DateOnlyConverterTest2() => _ = new DateOnlyConverter(format: null);
}

namespace FolkerKinzel.CsvTools.Mappings.Converters.Tests;

[TestClass()]
public class TimeOnlyConverterTests
{
    private readonly TimeOnlyConverter _conv = new();

    [TestMethod()]
    public void TimeOnlyConverterTest1()
    {
        Assert.IsNotNull(_conv);
        Assert.IsFalse(_conv.AcceptsNull);
        Assert.IsFalse(_conv.ParseExact);
    }

    [TestMethod]
    public void TimeOnlyConverterTest2() => _ = new DateOnlyConverter(format: null);
}
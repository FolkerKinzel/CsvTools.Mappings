namespace FolkerKinzel.CsvTools.Mappings.Tests;

[TestClass]
public class CsvWriterTests
{
    [TestMethod]
    public void DelimiterTest1()
    {
        using var stringWriter = new StringWriter();
        using CsvWriter csvWriter = Csv.OpenWrite(stringWriter, 1);
        using var objectWriter = new CsvWriter<object>(csvWriter, CsvMappingBuilder.Create().Build(), (o, dyn) => { });

        Assert.AreEqual(',', objectWriter.Delimiter);
    }
}

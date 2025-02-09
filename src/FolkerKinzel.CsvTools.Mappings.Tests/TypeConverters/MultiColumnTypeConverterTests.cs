
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Mappings.TypeConverters.Tests;

[TestClass]
public class MultiColumnTypeConverterTests
{
    private sealed class MultiIntConverter : MultiColumnTypeConverter<int>
    {
        public override bool AcceptsNull => false;

        public MultiIntConverter(CsvRecordMappingBuilder mapping, bool throwing) 
            : base(mapping, 0, throwing) { }
        
        public MultiIntConverter(MultiIntConverter other) : base(other) { }

        public override object Clone() => new MultiIntConverter(this);

        protected override void DoConvertToCsv(int value)
        {
            throw new NotImplementedException();
        }

        protected override bool TryParse(out int result)
        {
            result = default;
            return false;
        }
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CtorTest() => _ = new MultiIntConverter((CsvRecordMappingBuilder?)null!, false);

    [TestMethod]
    [ExpectedException(typeof(InvalidCastException))]
    public void ConvertToCsvTest1()
    {
        var conv = new MultiIntConverter(CsvRecordMappingBuilder.Create(), false);
        conv.ConvertToCsv(null);
    }

    [TestMethod]
    public void ParseTest1()
    {
        var conv = new MultiIntConverter(CsvRecordMappingBuilder.Create(), false);
        Assert.AreEqual(0, conv.Parse());
    }

    [TestMethod]
    [ExpectedException (typeof(FormatException))]
    public void ParseTest2()
    {
        var conv = new MultiIntConverter(CsvRecordMappingBuilder.Create(), true);
        Assert.AreEqual(0, conv.Parse());
    }
}

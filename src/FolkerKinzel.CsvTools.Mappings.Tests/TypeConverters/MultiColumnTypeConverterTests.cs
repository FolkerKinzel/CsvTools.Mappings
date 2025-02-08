
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Mappings.TypeConverters.Tests;

[TestClass]
public class MultiColumnTypeConverterTests
{
    private sealed class MultiIntConverter : MultiColumnTypeConverter<int>
    {
        public override bool AcceptsNull => false;

        public MultiIntConverter(CsvRecordMappingBuilder mapping) : base(mapping, 0, true) { }
        
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
    [ExpectedException(typeof(InvalidCastException))]
    public void ConvertToCsv()
    {
        var conv = new MultiIntConverter(CsvRecordMappingBuilder.Create());
        conv.ConvertToCsv(null);
    }
}

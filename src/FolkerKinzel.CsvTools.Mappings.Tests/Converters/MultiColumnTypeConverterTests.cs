
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Mappings.Converters.Tests;

[TestClass]
public class MultiColumnTypeConverterTests
{
    private sealed class MultiIntConverter : MultiColumnTypeConverter<int>
    {
        public override bool AcceptsNull => false;

        public MultiIntConverter(MappingBuilder mapping) : base(mapping, 0, true) { }
        
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
        var conv = new MultiIntConverter(MappingBuilder.Create());
        conv.ConvertToCsv(null);
    }
}

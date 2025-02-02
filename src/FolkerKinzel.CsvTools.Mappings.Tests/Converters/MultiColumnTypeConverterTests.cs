﻿
namespace FolkerKinzel.CsvTools.Mappings.Converters.Tests;

[TestClass]
public class MultiColumnTypeConverterTests
{
    private class MultiIntConverter : MultiColumnTypeConverter<int>
    {
        public MultiIntConverter(Mapping mapping) 
            : base(mapping, 0, true)
        {
        }

        public override bool AcceptsNull => false;

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


}

using FolkerKinzel.CsvTools.Mappings.TypeConverters;
using System.Globalization;

namespace Examples;

public sealed class Int128Converter() : TypeConverter<Int128>(default, true)
{
    public override bool AcceptsNull => false;

    public override string? ConvertToString(Int128 value)
        => value.ToString(null, CultureInfo.InvariantCulture);

    public override bool TryParse(ReadOnlySpan<char> value, out Int128 result) 
        => Int128.TryParse(value, out result);
}
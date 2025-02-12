using FolkerKinzel.CsvTools.Mappings.TypeConverters;
using System.Globalization;

namespace Examples;

/// <summary>
/// Example implementation of <see cref="TypeConverter{T}"/> for the
/// <see cref="Int128"/> struct.
/// </summary>
public sealed class Int128Converter() : TypeConverter<Int128>(true, default)
{
    public override bool AcceptsNull => false;

    public override string? ConvertToString(Int128 value)
        => value.ToString(null, CultureInfo.InvariantCulture);

    public override bool TryParse(ReadOnlySpan<char> value, out Int128 result)
        => Int128.TryParse(value, out result);
}
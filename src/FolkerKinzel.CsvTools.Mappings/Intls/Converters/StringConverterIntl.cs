using FolkerKinzel.CsvTools.Mappings.Converters;

namespace FolkerKinzel.CsvTools.Mappings.Intls.Converters;

/// <summary>
/// <see cref="TypeConverter{T}"/> implementation for <see cref="string"/>.
/// </summary>
internal sealed class StringConverterIntl : TypeConverter<string?>
{
    /// Initializes a new <see cref="StringConverter"/> instance.
    /// <param name="fallbackValue">The value of <see cref="TypeConverter{T}.FallbackValue"/>.</param>
    internal StringConverterIntl(string? fallbackValue)
        : base(fallbackValue, false) { }

    /// <inheritdoc/>
    public override bool AllowsNull => true;

    /// <inheritdoc/>
    public override string? ConvertToString(string? value) => value;

    /// <inheritdoc/>
    public override bool TryParseValue(ReadOnlySpan<char> value, out string? result)
    {
        if (value.IsEmpty)
        {
            result = FallbackValue;
            return false;
        }

        result = value.ToString();
        return true;
    }
}

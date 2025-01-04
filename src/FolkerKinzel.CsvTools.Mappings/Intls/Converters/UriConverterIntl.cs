using FolkerKinzel.CsvTools.Mappings.Converters;

namespace FolkerKinzel.CsvTools.Mappings.Intls.Converters;

/// <summary>
/// <see cref="TypeConverter{T}"/> implementation for <see cref="Uri"/>.
/// </summary>
internal sealed class UriConverterIntl : TypeConverter<Uri?>
{
    /// <summary>Initializes a new <see cref="UriConverter"/> instance.</summary>
    /// <param name="throwing">Sets the value of the 
    /// <see cref="TypeConverter{T}.Throwing"/> property.</param>
    /// <param name="fallbackValue">The value of <see cref="TypeConverter{T}.FallbackValue"/>.</param>
    internal UriConverterIntl(bool throwing, Uri? fallbackValue)
        : base(fallbackValue, throwing) { }

    /// <inheritdoc/>
    public override bool AllowsNull => true;

    /// <inheritdoc/>
    public override string? ConvertToString(Uri? value)
        => value is null
            ? null
            : value.IsAbsoluteUri
                ? value.AbsoluteUri
                : value.ToString();

    /// <inheritdoc/>
    public override bool TryParseValue(ReadOnlySpan<char> value, out Uri? result)
    {
        if (value.IsWhiteSpace())
        {
            result = FallbackValue;
            return true;
        }

        return Uri.TryCreate(value.Trim().ToString(), UriKind.RelativeOrAbsolute, out result);
    }
}

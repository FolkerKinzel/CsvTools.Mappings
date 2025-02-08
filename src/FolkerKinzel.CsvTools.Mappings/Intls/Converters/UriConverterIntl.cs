using FolkerKinzel.CsvTools.Mappings.TypeConverters;

namespace FolkerKinzel.CsvTools.Mappings.Intls.Converters;

/// <summary>
/// <see cref="TypeConverter{T}"/> implementation for <see cref="Uri"/>.
/// </summary>
internal sealed class UriConverterIntl : TypeConverter<Uri?>
{
    private readonly UriKind _uriKind;

    /// <summary>Initializes a new <see cref="UriConverter"/> instance.</summary>
    /// <param name="uriKind">The type of the <see cref="Uri"/>.</param>
    /// <param name="throwing">Sets the value of the 
    /// <see cref="TypeConverter{T}.Throwing"/> property.</param>
    /// <param name="defaultValue">The value of <see cref="TypeConverter{T}.DefaultValue"/>.</param>
    internal UriConverterIntl(UriKind uriKind, bool throwing, Uri? defaultValue)
        : base(defaultValue, throwing)
    {
        _uriKind = uriKind;
    }

    /// <inheritdoc/>
    public override bool AcceptsNull => true;

    /// <inheritdoc/>
    public override string? ConvertToString(Uri? value)
        => value is null
                ? null
                : value.IsAbsoluteUri
                    ? value.AbsoluteUri
                    : value.ToString();

    /// <inheritdoc/>
    public override bool TryParse(ReadOnlySpan<char> value, out Uri? result)
    {
        Debug.Assert(!value.IsEmpty);

        return Uri.TryCreate(value.Trim().ToString(), _uriKind, out result);
    }
}

using FolkerKinzel.CsvTools.Mappings.TypeConverters;

namespace FolkerKinzel.CsvTools.Mappings.Intls.Converters;

/// <summary>
/// <see cref="TypeConverter{T}"/> implementation for <see cref="Version"/>.
/// </summary>
internal sealed class VersionConverterIntl : TypeConverter<Version?>
{
    /// <summary>Initializes a new <see cref="VersionConverterIntl"/> instance.</summary>
    /// <param name="throwing">Sets the value of the 
    /// <see cref="TypeConverter{T}.Throwing"/> property.</param>
    /// <param name="defaultValue">The value of <see cref="TypeConverter{T}.DefaultValue"/>.</param>
    internal VersionConverterIntl(bool throwing, Version? defaultValue)
        : base(defaultValue, throwing) { }

    /// <inheritdoc/>
    public override bool AcceptsNull => true;

    /// <inheritdoc/>
    public override string? ConvertToString(Version? value) => value?.ToString();

    /// <inheritdoc/>
    public override bool TryParseValue(ReadOnlySpan<char> value, out Version? result)
    {
        if (value.IsWhiteSpace())
        {
            result = DefaultValue;
            return true;
        }

        try
        {
            result = new Version(value.ToString());
            return true;
        }
        catch
        {
            result = DefaultValue;
            return false;
        }
    }
}

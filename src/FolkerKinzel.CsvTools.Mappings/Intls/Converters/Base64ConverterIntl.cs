using FolkerKinzel.CsvTools.Mappings.TypeConverters;
using System.Diagnostics.CodeAnalysis;

namespace FolkerKinzel.CsvTools.Mappings.Intls.Converters;

/// <summary>
/// <see cref="TypeConverter{T}"/> implementation for <see cref="byte"/> arrays.
/// </summary>
internal sealed class Base64ConverterIntl : TypeConverter<byte[]?>
{
    /// <summary>Initializes a new <see cref="Base64Converter"/> instance.</summary>
    /// <param name="throwing">Sets the value of the 
    /// <see cref="TypeConverter{T}.Throwing"/> property.</param>
    /// <param name="defaultValue">The value of <see cref="TypeConverter{T}.DefaultValue"/>.</param>
    internal Base64ConverterIntl(bool throwing, byte[]? defaultValue)
        : base(throwing, defaultValue) { }

    /// <inheritdoc/>
    public override bool AcceptsNull => true;

    /// <inheritdoc/>
    public override string? ConvertToString(byte[]? value)
        => value is null
              ? null
              : Convert.ToBase64String(value, Base64FormattingOptions.None);

    /// <inheritdoc/>
    public override bool TryParse(ReadOnlySpan<char> value, out byte[]? result)
    {
        // Needed to return null if the default value is null
        if (value.IsWhiteSpace())
        {
            result = DefaultValue;
            return true;
        }

        try
        {
            result = Base64.GetBytes(value, Base64ParserOptions.AcceptMissingPadding);
            return true;
        }
        catch (FormatException)
        {
            result = [];
            return false;
        }
    }
}

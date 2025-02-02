using FolkerKinzel.CsvTools.Mappings.Converters;
using System.Diagnostics.CodeAnalysis;

namespace FolkerKinzel.CsvTools.Mappings.Intls.Converters;

/// <summary>
/// <see cref="TypeConverter{T}"/> implementation for <see cref="byte"/> arrays.
/// </summary>
internal sealed class ByteArrayConverterIntl : TypeConverter<byte[]?>
{
    /// <summary>Initializes a new <see cref="ByteArrayConverter"/> instance.</summary>
    /// <param name="throwing">Sets the value of the 
    /// <see cref="TypeConverter{T}.Throwing"/> property.</param>
    /// <param name="defaultValue">The value of <see cref="TypeConverter{T}.DefaultValue"/>.</param>
    internal ByteArrayConverterIntl(bool throwing, byte[]? defaultValue)
        : base(defaultValue, throwing) { }

    /// <inheritdoc/>
    public override bool AcceptsNull => true;

    /// <inheritdoc/>
    public override string? ConvertToString(byte[]? value)
        => value is null
              ? null
              : Convert.ToBase64String(value, Base64FormattingOptions.None);

    /// <inheritdoc/>
    public override bool TryParseValue(ReadOnlySpan<char> value, out byte[]? result)
    {
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

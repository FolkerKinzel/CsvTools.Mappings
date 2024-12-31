using System.Diagnostics.CodeAnalysis;

namespace FolkerKinzel.CsvTools.Mappings.Converters;

/// <summary>
/// <see cref="TypeConverter{T}"/> implementation for <see cref="byte"/> arrays.
/// </summary>
/// <param name="throwing">Sets the value of the 
/// <see cref="TypeConverter{T}.Throwing"/> property.</param>
/// <param name="nullable"><c>true</c> to set <see cref="TypeConverter{T}.FallbackValue"/>
/// to <c>null</c>; <c>false</c> to have an empty <see cref="byte"/> array as 
/// <see cref="TypeConverter{T}.FallbackValue"/>.</param>
public sealed class ByteArrayConverter(bool throwing = true, bool nullable = true) 
    : TypeConverter<byte[]?>(throwing, nullable ? null : [])
{
    /// <inheritdoc/>
    public override bool AcceptsNull => true;

    /// <inheritdoc/>
    public override string? ConvertToString(byte[]? value)
        => value is null 
              ? null 
              : Convert.ToBase64String(value, Base64FormattingOptions.None);

    /// <inheritdoc/>
    public override bool TryParseValue(ReadOnlySpan<char> value, [NotNullWhen(true)] out byte[]? result)
    {
        try
        {
            result = Base64.GetBytes(value, Base64ParserOptions.AcceptMissingPadding);
            return true;
        }
        catch (FormatException)
        {
            result = null;
            return false;
        }
    }
}

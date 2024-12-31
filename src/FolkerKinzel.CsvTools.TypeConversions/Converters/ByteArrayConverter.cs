using System.Diagnostics.CodeAnalysis;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

/// <summary>
/// <see cref="CsvTypeConverter{T}"/> implementation for <see cref="byte"/> arrays.
/// </summary>
/// <param name="throwing">Sets the value of the 
/// <see cref="CsvTypeConverter{T}.Throwing"/> property.</param>
/// <param name="nullable"><c>true</c> to set <see cref="CsvTypeConverter{T}.FallbackValue"/>
/// to <c>null</c>; <c>false</c> to have an empty <see cref="byte"/> array as 
/// <see cref="CsvTypeConverter{T}.FallbackValue"/>.</param>
public sealed class ByteArrayConverter(bool throwing = true, bool nullable = true) 
    : CsvTypeConverter<byte[]?>(throwing, nullable ? null : [])
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
            result = (byte[])Convert.FromBase64String(value.ToString());
            return true;
        }
        catch (FormatException)
        {
            result = null;
            return false;
        }
    }
}

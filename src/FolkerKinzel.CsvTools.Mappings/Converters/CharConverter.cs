namespace FolkerKinzel.CsvTools.Mappings.Converters;

/// <summary>
/// <see cref="TypeConverter{T}"/> implementation for <see cref="char"/>.
/// </summary>
/// <param name="fallbackValue">
/// The <see cref="TypeConverter{T}.FallbackValue"/> to return when a parsing error occurs and
/// the <see cref="TypeConverter{T}.Throwing"/> property is <c>false</c>.
/// </param>
/// <param name="throwing">Sets the value of the 
/// <see cref="TypeConverter{T}.Throwing"/> property.</param>
public sealed class CharConverter(char fallbackValue = default, bool throwing = true)
    : TypeConverter<char>(fallbackValue, throwing)
{
    /// <inheritdoc/>
    public override bool AllowsNull => false;

    /// <inheritdoc/>
    public override string? ConvertToString(char value) => value.ToString();

    /// <inheritdoc/>
    public override bool TryParseValue(ReadOnlySpan<char> value, out char result)
    {
        if (value.Length == 1)
        {
            result = value[0];
            return true;
        }

        result = default;
        return false;
    }
}

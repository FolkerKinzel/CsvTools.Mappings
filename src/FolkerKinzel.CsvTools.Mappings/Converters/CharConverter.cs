namespace FolkerKinzel.CsvTools.Mappings.Converters;

/// <summary>
/// <see cref="CsvTypeConverter{T}"/> implementation for <see cref="char"/>.
/// </summary>
/// <param name="throwing">Sets the value of the 
/// <see cref="CsvTypeConverter{T}.Throwing"/> property.</param>
/// <param name="fallbackValue">
/// The <see cref="CsvTypeConverter{T}.FallbackValue"/> to return when a parsing error occurs and
/// the <see cref="CsvTypeConverter{T}.Throwing"/> property is <c>false</c>.
/// </param>
public sealed class CharConverter(bool throwing = true, char fallbackValue = default) 
    : CsvTypeConverter<char>(throwing, fallbackValue)
{
    /// <inheritdoc/>
    public override bool AcceptsNull => false;

    /// <inheritdoc/>
    public override string? ConvertToString(char value) => value.ToString();

    /// <inheritdoc/>
    public override bool TryParseValue(ReadOnlySpan<char> value, out char result)
    {
        if (value.Trim().Length == 1)
        {
            result = value[0];
            return true;
        }

        result = default;
        return false;
    }
}

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

/// <summary>
/// <see cref="CsvTypeConverter{T}"/> implementation for <see cref="char"/>.
/// </summary>
public sealed class CharConverter(bool throwing = true, char fallbackValue = default) 
    : CsvTypeConverter<char>(throwing, fallbackValue)
{
    /// <inheritdoc/>
    public override bool AcceptsNull => false;

    /// <inheritdoc/>
    protected override string? DoConvertToString(char value) => value.ToString();

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

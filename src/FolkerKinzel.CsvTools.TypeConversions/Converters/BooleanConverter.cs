namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

/// <summary>
/// <see cref="CsvTypeConverter{T}"/> implementation for <see cref="bool"/>.
/// </summary>
/// <param name="throwing">Sets the value of the 
/// <see cref="CsvTypeConverter{T}.Throwing"/> property.</param>
/// <param name="fallbackValue">
/// The <see cref="CsvTypeConverter{T}.FallbackValue"/> to return when a parsing error occurs and
/// the <see cref="CsvTypeConverter{T}.Throwing"/> property is <c>false</c>.
/// </param>
public sealed class BooleanConverter(bool throwing = true, bool fallbackValue = default)
    : CsvTypeConverter<bool>(throwing, fallbackValue)
{
    /// <inheritdoc/>
    public override bool AcceptsNull => false;

    /// <inheritdoc/>
    public override string? ConvertToString(bool value) => value.ToString();

    /// <inheritdoc/>
    public override bool TryParseValue(ReadOnlySpan<char> value, out bool result)
#if NET462 || NETSTANDARD2_0
        => bool.TryParse(value.ToString(), out result);
#else
        => bool.TryParse(value, out result);
#endif
}

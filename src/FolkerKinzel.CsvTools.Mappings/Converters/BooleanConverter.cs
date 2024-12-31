namespace FolkerKinzel.CsvTools.Mappings.Converters;

/// <summary>
/// <see cref="TypeConverter{T}"/> implementation for <see cref="bool"/>.
/// </summary>
/// <param name="throwing">Sets the value of the 
/// <see cref="TypeConverter{T}.Throwing"/> property.</param>
/// <param name="fallbackValue">
/// The <see cref="TypeConverter{T}.FallbackValue"/> to return when a parsing error occurs and
/// the <see cref="TypeConverter{T}.Throwing"/> property is <c>false</c>.
/// </param>
public sealed class BooleanConverter(bool throwing = true, bool fallbackValue = default)
    : TypeConverter<bool>(throwing, fallbackValue)
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

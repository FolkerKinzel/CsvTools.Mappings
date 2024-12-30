namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

/// <summary>
/// <see cref="CsvTypeConverter{T}"/> implementation for <see cref="bool"/>.
/// </summary>
public sealed class BooleanConverter(bool throwing = true, bool fallbackValue = default)
    : CsvTypeConverter<bool>(throwing, fallbackValue)
{

    /// <inheritdoc/>
    public override bool AcceptsNull => false;

    /// <inheritdoc/>
    protected override string? DoConvertToString(bool value) => value.ToString();

    /// <inheritdoc/>
    public override bool TryParseValue(ReadOnlySpan<char> value, out bool result)
#if NET462 || NETSTANDARD2_0
        => bool.TryParse(value.ToString(), out result);
#else
        => bool.TryParse(value, out result);
#endif
}

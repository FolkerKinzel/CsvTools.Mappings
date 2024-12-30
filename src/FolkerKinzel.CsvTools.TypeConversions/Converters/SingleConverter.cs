using System.Globalization;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

/// <summary>
/// <see cref="CsvTypeConverter{T}"/> implementation for <see cref="float"/>.
/// </summary>
public sealed class SingleConverter(bool throwing = true, IFormatProvider? formatProvider = null)
    : CsvTypeConverter<float>(throwing)
{
    private readonly IFormatProvider? _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;

    private const string FORMAT = "G9";
    private const NumberStyles STYLE = NumberStyles.Any;

    /// <inheritdoc/>
    public override bool AcceptsNull => false;

    /// <inheritdoc/>
    public override string? ConvertToString(float value) 
        => value.ToString(FORMAT, _formatProvider);

    /// <inheritdoc/>
    public override bool TryParseValue(ReadOnlySpan<char> value, out float result)
#if NET462 || NETSTANDARD2_0
        => float.TryParse(value.ToString(), STYLE, _formatProvider, out result);
#else
        => float.TryParse(value, STYLE, _formatProvider, out result);
#endif
}

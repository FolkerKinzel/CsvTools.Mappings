using System.Globalization;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

/// <summary>
/// <see cref="CsvTypeConverter{T}"/> implementation for <see cref="double"/>.
/// </summary>
public sealed class DoubleConverter(bool throwing = true, IFormatProvider? formatProvider = null)
    : CsvTypeConverter<double>(throwing)
{
    private readonly IFormatProvider? _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;

    private const string FORMAT = "G17";
    private const NumberStyles STYLE = NumberStyles.Any;

    /// <inheritdoc/>
    public override bool AcceptsNull => false;

    /// <inheritdoc/>
    public override string? ConvertToString(double value) => value.ToString(FORMAT, _formatProvider);

    /// <inheritdoc/>
    public override bool TryParseValue(ReadOnlySpan<char> value, out double result)
#if NET462 || NETSTANDARD2_0
        => double.TryParse(value.ToString(), STYLE, _formatProvider, out result);
#else
        => double.TryParse(value, STYLE, _formatProvider, out result);
#endif
}

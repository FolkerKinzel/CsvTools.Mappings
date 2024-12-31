using System.Globalization;

namespace FolkerKinzel.CsvTools.Mappings.Converters;

/// <summary>
/// <see cref="CsvTypeConverter{T}"/> implementation for <see cref="ulong"/>.
/// </summary>
/// <param name="throwing">Sets the value of the 
/// <see cref="CsvTypeConverter{T}.Throwing"/> property.</param>
/// <param name="formatProvider">
/// An <see cref="IFormatProvider"/> instance that provides culture-specific formatting information, or <c>null</c> for 
/// <see cref="CultureInfo.InvariantCulture"/>.
/// </param>
[CLSCompliant(false)]
public sealed class UInt64Converter(bool throwing = true, IFormatProvider? formatProvider = null)
    : CsvTypeConverter<ulong>(throwing), IHexConverter<ulong>
{
    private const NumberStyles DEFAULT_STYLE = NumberStyles.Any;
    private const NumberStyles HEX_STYLE = NumberStyles.HexNumber;
    private const string HEX_FORMAT = "X";
    private const string? DEFAULT_FORMAT = null;

    private readonly IFormatProvider? _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
    private NumberStyles _styles = DEFAULT_STYLE;
    private string? _format = DEFAULT_FORMAT;

    /// <inheritdoc/>
    public override bool AcceptsNull => false;

    /// <inheritdoc/>
    public CsvTypeConverter<ulong> AsHexConverter()
    {
        _styles = HEX_STYLE;
        _format = HEX_FORMAT;
        return this;
    }

    /// <inheritdoc/>
    public override string? ConvertToString(ulong value) => value.ToString(_format, _formatProvider);

    /// <inheritdoc/>
    public override bool TryParseValue(ReadOnlySpan<char> value, out ulong result)
#if NET462 || NETSTANDARD2_0
        => ulong.TryParse(value.ToString(), _styles, _formatProvider, out result);
#else
        => ulong.TryParse(value, _styles, _formatProvider, out result);
#endif
}

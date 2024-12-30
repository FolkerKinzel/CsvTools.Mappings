using System.Globalization;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

/// <summary>
/// <see cref="CsvTypeConverter{T}"/> implementation for <see cref="byte"/>.
/// </summary>
public sealed class ByteConverter(bool throwing = true, IFormatProvider? formatProvider = null)
    : CsvTypeConverter<byte>(throwing), IHexConverter<byte>
{
    private const NumberStyles DEFAULT_STYLE = NumberStyles.Any;
    private const NumberStyles HEX_STYLE = NumberStyles.HexNumber;
    private const string HEX_FORMAT = "X";
    private const string? DEFAULT_FORMAT = null;

    private readonly IFormatProvider? _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
    private  NumberStyles _styles = DEFAULT_STYLE;
    private  string? _format = DEFAULT_FORMAT;

    /// <inheritdoc/>
    public CsvTypeConverter<byte> AsHexConverter()
    {
        _styles = HEX_STYLE;
        _format = HEX_FORMAT;
        return this;
    }

    /// <inheritdoc/>
    public override bool AcceptsNull => false;

    /// <inheritdoc/>
    protected override string? DoConvertToString(byte value) 
        => value.ToString(_format, _formatProvider);

    /// <inheritdoc/>
    public override bool TryParseValue(ReadOnlySpan<char> value, out byte result)
#if NET462 || NETSTANDARD2_0
        => byte.TryParse(value.ToString(), _styles, _formatProvider, out result);
#else
        => byte.TryParse(value, _styles, _formatProvider, out result);
#endif
}

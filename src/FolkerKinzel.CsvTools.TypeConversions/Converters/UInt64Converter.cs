﻿using System.Globalization;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

[CLSCompliant(false)]
public sealed class UInt64Converter : CsvTypeConverter<ulong>, IHexConverter
{
    private const NumberStyles DEFAULT_STYLE = NumberStyles.Any;
    private const NumberStyles HEX_STYLE = NumberStyles.HexNumber;
    private const string HEX_FORMAT = "X";
    private const string? DEFAULT_FORMAT = null;

    private readonly IFormatProvider? _formatProvider;
    private NumberStyles _styles = DEFAULT_STYLE;
    private string? _format = DEFAULT_FORMAT;

    public UInt64Converter(bool throwing = true, IFormatProvider? formatProvider = null)
        : base(throwing) => _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;

    /// <inheritdoc/>
    public override bool AcceptsNull => false;

    /// <inheritdoc/>
    public ICsvTypeConverter AsHexConverter()
    {
        _styles = HEX_STYLE;
        _format = HEX_FORMAT;
        return this;
    }

    /// <inheritdoc/>
    protected override string? DoConvertToString(ulong value) => value.ToString(_format, _formatProvider);

    /// <inheritdoc/>
    public override bool TryParseValue(ReadOnlySpan<char> value, out ulong result)
#if NET462 || NETSTANDARD2_0
        => ulong.TryParse(value.ToString(), _styles, _formatProvider, out result);
#else
        => ulong.TryParse(value, _styles, _formatProvider, out result);
#endif
}

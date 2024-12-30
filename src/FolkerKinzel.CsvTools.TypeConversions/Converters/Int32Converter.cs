﻿using System.Globalization;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

/// <summary>
/// <see cref="CsvTypeConverter{T}"/> implementation for <see cref="int"/>.
/// </summary>
public sealed class Int32Converter(bool throwing = true, IFormatProvider? formatProvider = null) 
    : CsvTypeConverter<int>(throwing), IHexConverter<int>
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
    public CsvTypeConverter<int> AsHexConverter()
    {
        _styles = HEX_STYLE;
        _format = HEX_FORMAT;
        return this;
    }

    /// <inheritdoc/>
    public override string? ConvertToString(int value) => value.ToString(_format, _formatProvider);
    
    /// <inheritdoc/>
    public override bool TryParseValue(ReadOnlySpan<char> value, out int result)
#if NET462 || NETSTANDARD2_0
        => int.TryParse(value.ToString(), _styles, _formatProvider, out result);
#else
        => int.TryParse(value, _styles, _formatProvider, out result);
#endif
}

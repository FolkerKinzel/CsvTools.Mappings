﻿using System.Globalization;

namespace FolkerKinzel.CsvTools.Mappings.Converters;

/// <summary>
/// <see cref="TypeConverter{T}"/> implementation for <see cref="long"/>.
/// </summary>
/// <param name="throwing">Sets the value of the 
/// <see cref="TypeConverter{T}.Throwing"/> property.</param>
/// <param name="formatProvider">
/// An <see cref="IFormatProvider"/> instance that provides culture-specific formatting information, or <c>null</c> for 
/// <see cref="CultureInfo.InvariantCulture"/>.
/// </param>
public sealed class Int64Converter(bool throwing = true, IFormatProvider? formatProvider = null)
    : TypeConverter<long>(throwing), IHexConverter<long>
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
    public TypeConverter<long> AsHexConverter()
    {
        _styles = HEX_STYLE;
        _format = HEX_FORMAT;
        return this;
    }

    /// <inheritdoc/>
    public override string? ConvertToString(long value) => value.ToString(_format, _formatProvider);

    /// <inheritdoc/>
    public override bool TryParseValue(ReadOnlySpan<char> value, out long result)
#if NET462 || NETSTANDARD2_0
        => long.TryParse(value.ToString(), _styles, _formatProvider, out result);
#else
        => long.TryParse(value, _styles, _formatProvider, out result);
#endif
}

﻿using System.Globalization;

namespace FolkerKinzel.CsvTools.Mappings.Converters;

/// <summary>
/// <see cref="TypeConverter{T}"/> implementation for <see cref="int"/>.
/// </summary>
/// <param name="formatProvider">
/// An <see cref="IFormatProvider"/> instance that provides culture-specific formatting information, or <c>null</c> for 
/// <see cref="CultureInfo.InvariantCulture"/>.
/// </param>
/// <param name="throwing">Sets the value of the 
/// <see cref="TypeConverter{T}.Throwing"/> property.</param>
public sealed class Int32Converter(IFormatProvider? formatProvider = null, bool throwing = true)
    : TypeConverter<int>(default, throwing), IHexConverter<int>
{
    private const NumberStyles DEFAULT_STYLE = NumberStyles.Any;
    private const NumberStyles HEX_STYLE = NumberStyles.HexNumber;
    private const string HEX_FORMAT = "X";
    private const string? DEFAULT_FORMAT = null;

    private readonly IFormatProvider? _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
    private NumberStyles _styles = DEFAULT_STYLE;
    private string? _format = DEFAULT_FORMAT;

    /// <inheritdoc/>
    public override bool AllowsNull => false;

    /// <inheritdoc/>
    public bool IsHexConverter { get; private set; }

    /// <inheritdoc/>
    public TypeConverter<int> ToHexConverter()
    {
        var clone = (Int32Converter)Clone();
        clone._styles = HEX_STYLE;
        clone._format = HEX_FORMAT;
        clone.IsHexConverter = true;
        return clone;
    }

    /// <inheritdoc/>
    public object Clone() => new Int32Converter(_formatProvider, Throwing);

    /// <inheritdoc/>
    public override string? ConvertToString(int value) => value.ToString(_format, _formatProvider);

    /// <inheritdoc/>
    public override bool TryParseValue(ReadOnlySpan<char> value, out int result)
    {
#if NET462 || NETSTANDARD2_0
        result = default;
        return !value.IsWhiteSpace() && int.TryParse(value.ToString(), _styles, _formatProvider, out result);
#else
        return int.TryParse(value, _styles, _formatProvider, out result);
#endif
    }
}

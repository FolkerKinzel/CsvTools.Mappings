using System.Globalization;

namespace FolkerKinzel.CsvTools.Mappings.Converters;

/// <summary>
/// <see cref="TypeConverter{T}"/> implementation for <see cref="sbyte"/>.
/// </summary>
/// <param name="throwing">Sets the value of the 
/// <see cref="TypeConverter{T}.Throwing"/> property.</param>
/// <param name="formatProvider">
/// An <see cref="IFormatProvider"/> instance that provides culture-specific formatting information, or <c>null</c> for 
/// <see cref="CultureInfo.InvariantCulture"/>.
/// </param>
[CLSCompliant(false)]
public sealed class SByteConverter(bool throwing = true, IFormatProvider? formatProvider = null) 
    : TypeConverter<sbyte>(throwing), IHexConverter<sbyte>
{
    private const NumberStyles DEFAULT_STYLE = NumberStyles.Any;
    private const NumberStyles HEX_STYLE = NumberStyles.HexNumber;
    private const string HEX_FORMAT = "X";

    private readonly IFormatProvider? _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
    private NumberStyles _styles = DEFAULT_STYLE;
    private string? _format = DEFAULT_FORMAT;
    
    private const string? DEFAULT_FORMAT = null;

    /// <inheritdoc/>
    public override bool AcceptsNull => false;

    /// <inheritdoc/>
    public bool IsHexConverter { get; private set; }

    /// <inheritdoc/>
    public TypeConverter<sbyte> ToHexConverter()
    {
        var clone = (SByteConverter)Clone();
        clone._styles = HEX_STYLE;
        clone._format = HEX_FORMAT;
        clone.IsHexConverter = true;
        return clone;
    }

    /// <inheritdoc/>
    public object Clone() => new SByteConverter(Throwing, _formatProvider);

    /// <inheritdoc/>
    public override string? ConvertToString(sbyte value) => value.ToString(_format, _formatProvider);

    /// <inheritdoc/>
    public override bool TryParseValue(ReadOnlySpan<char> value, out sbyte result)
#if NET462 || NETSTANDARD2_0
        => sbyte.TryParse(value.ToString(), _styles, _formatProvider, out result);
#else
        => sbyte.TryParse(value, _styles, _formatProvider, out result);
#endif
}

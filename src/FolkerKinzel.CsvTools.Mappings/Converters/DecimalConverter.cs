using System.Globalization;

namespace FolkerKinzel.CsvTools.Mappings.Converters;

/// <summary>
/// <see cref="TypeConverter{T}"/> implementation for <see cref="decimal"/>.
/// </summary>
/// <param name="formatProvider">
/// An <see cref="IFormatProvider"/> instance that provides culture-specific formatting information, or <c>null</c> for 
/// <see cref="CultureInfo.InvariantCulture"/>.
/// </param>
/// <param name="throwing">Sets the value of the 
/// <see cref="TypeConverter{T}.Throwing"/> property.</param>
public sealed class DecimalConverter(IFormatProvider? formatProvider = null, bool throwing = true)
    : TypeConverter<decimal>(default, throwing)
{
    private readonly IFormatProvider? _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;

    private const string FORMAT = "G";
    private const NumberStyles STYLE = NumberStyles.Any;

    /// <inheritdoc/>
    public override bool AllowsNull => false;

    /// <inheritdoc/>
    public override string? ConvertToString(decimal value) => value.ToString(FORMAT, _formatProvider);

    /// <inheritdoc/>
    public override bool TryParseValue(ReadOnlySpan<char> value, out decimal result)
    {
#if NET462 || NETSTANDARD2_0
        result = default;
        return !value.IsWhiteSpace() && decimal.TryParse(value.ToString(), STYLE, _formatProvider, out result);
#else
        return decimal.TryParse(value, STYLE, _formatProvider, out result);
#endif
    }
}

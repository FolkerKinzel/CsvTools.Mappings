using System.Globalization;

namespace FolkerKinzel.CsvTools.Mappings.Converters;

/// <summary>
/// <see cref="CsvTypeConverter{T}"/> implementation for <see cref="decimal"/>.
/// </summary>
/// <param name="throwing">Sets the value of the 
/// <see cref="CsvTypeConverter{T}.Throwing"/> property.</param>
/// <param name="formatProvider">
/// An <see cref="IFormatProvider"/> instance that provides culture-specific formatting information, or <c>null</c> for 
/// <see cref="CultureInfo.InvariantCulture"/>.
/// </param>
public sealed class DecimalConverter(bool throwing = true, IFormatProvider? formatProvider = null)
    : CsvTypeConverter<decimal>(throwing)
{
    private readonly IFormatProvider? _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;

    private const string FORMAT = "G";
    private const NumberStyles STYLE = NumberStyles.Any;

    /// <inheritdoc/>
    public override bool AcceptsNull => false;

    /// <inheritdoc/>
    public override string? ConvertToString(decimal value) => value.ToString(FORMAT, _formatProvider);

    /// <inheritdoc/>
    public override bool TryParseValue(ReadOnlySpan<char> value, out decimal result)
#if NET462 || NETSTANDARD2_0
        => decimal.TryParse(value.ToString(), STYLE, _formatProvider, out result);
#else
        => decimal.TryParse(value, STYLE, _formatProvider, out result);
#endif
}

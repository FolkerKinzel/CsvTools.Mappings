using System.Globalization;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

public sealed class DecimalConverter : CsvTypeConverter<decimal>
{
    private readonly IFormatProvider? _formatProvider;

    private const string FORMAT = "G";
    private const NumberStyles STYLE = NumberStyles.Any;

    public DecimalConverter(bool throwing = true, IFormatProvider? formatProvider = null)
        : base(throwing) => _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;

    /// <inheritdoc/>
    public override bool AcceptsNull => false;

    /// <inheritdoc/>
    protected override string? DoConvertToString(decimal value) => value.ToString(FORMAT, _formatProvider);

    /// <inheritdoc/>
    public override bool TryParseValue(ReadOnlySpan<char> value, out decimal result)
#if NET462 || NETSTANDARD2_0
        => decimal.TryParse(value.ToString(), STYLE, _formatProvider, out result);
#else
        => decimal.TryParse(value, STYLE, _formatProvider, out result);
#endif
}

using System.Globalization;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

public sealed class DoubleConverter : CsvTypeConverter<double>
{
    private readonly IFormatProvider? _formatProvider;

    private const string FORMAT = "G17";
    private const NumberStyles STYLE = NumberStyles.Any;

    public DoubleConverter(bool throwing = true, IFormatProvider? formatProvider = null)
        : base(throwing) => _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;

    /// <inheritdoc/>
    public override bool AcceptsNull => false;

    /// <inheritdoc/>
    protected override string? DoConvertToString(double value) => value.ToString(FORMAT, _formatProvider);

    /// <inheritdoc/>
    public override bool TryParseValue(ReadOnlySpan<char> value, out double result)
#if NET462 || NETSTANDARD2_0
        => double.TryParse(value.ToString(), STYLE, _formatProvider, out result);
#else
        => double.TryParse(value, STYLE, _formatProvider, out result);
#endif
}

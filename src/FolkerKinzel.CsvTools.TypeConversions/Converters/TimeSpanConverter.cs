using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

/// <summary>
/// <see cref="CsvTypeConverter{T}"/> implementation for <see cref="TimeSpan"/>.
/// </summary>
public sealed class TimeSpanConverter : CsvTypeConverter<TimeSpan>
{
    private const string DEFAULT_FORMAT = "g";

    private readonly IFormatProvider _formatProvider;
    private readonly bool _parseExact;
    private readonly TimeSpanStyles _styles;

    /// <summary>
    /// Initializes a new <see cref="TimeSpanConverter"/> instance.
    /// </summary>
    /// <param name="throwing">Sets the value of the 
    /// <see cref="CsvTypeConverter{T}.Throwing"/> property.</param>
    /// <param name="formatProvider">
    /// An <see cref="IFormatProvider"/> instance that provides culture-specific formatting information, or <c>null</c> for 
    /// <see cref="CultureInfo.InvariantCulture"/>.
    /// </param>
    /// <remarks>This constructor initializes a <see cref="TimeSpanConverter"/> instance that uses the format string
    /// "g". This constructor is much faster than its overload.</remarks>
    public TimeSpanConverter(bool throwing = true, IFormatProvider? formatProvider = null) : base(throwing)
        => _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;

    /// <summary>
    /// Initializes a new <see cref="TimeSpanConverter"/> instance and allows to specify a format string.
    /// </summary>
    /// <param name="format">
    /// A format string that is used for the <see cref="string"/> output of <see cref="TimeSpan"/> values. If the 
    /// option <paramref name="parseExact"/> is selected this format string is also used for parsing.</param>
    /// <param name="throwing">Sets the value of the 
    /// <see cref="CsvTypeConverter{T}.Throwing"/> property.</param>
    /// <param name="formatProvider">
    /// An <see cref="IFormatProvider"/> instance that provides culture-specific formatting information, or <c>null</c> for 
    /// <see cref="CultureInfo.InvariantCulture"/>.
    /// </param>
    /// <param name="parseExact">
    /// If <c>true</c> the text in the CSV file must exactly match the format string specified with <paramref name="format"/>.
    /// </param>
    /// 
    /// <param name="styles">
    /// A value of the <see cref="TimeSpanStyles"/> enum that provides additional information for parsing. (Becomes evaluated only if 
    /// <paramref name="parseExact"/> is <c>true</c>.)
    /// </param>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="format"/> is <c>null</c> and <paramref name="parseExact"/> is <c>true</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="format"/> is not a valid format string.</exception>
    public TimeSpanConverter(
        string format,
        bool throwing = true,
        IFormatProvider? formatProvider = null,
        bool parseExact = false,
        TimeSpanStyles styles = TimeSpanStyles.None) : base(throwing)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _styles = styles;
        Format = format;
        _parseExact = parseExact;
        ExamineFormat();
    }

    /// <inheritdoc/>
    public override bool AcceptsNull => false;

    /// <summary>
    /// The format string to use.
    /// </summary>
    public string Format { get; } = DEFAULT_FORMAT;

    /// <inheritdoc/>
    public override string? ConvertToString(TimeSpan value) => value.ToString(Format, _formatProvider);

    /// <inheritdoc/>
    public override bool TryParseValue(ReadOnlySpan<char> value, [NotNullWhen(true)] out TimeSpan result)
#if NET462 || NETSTANDARD2_0
        => _parseExact
            ? TimeSpan.TryParseExact(value.ToString(), Format, _formatProvider, _styles, out result)
            : TimeSpan.TryParse(value.ToString(), _formatProvider, out result);
#else
        => _parseExact
            ? TimeSpan.TryParseExact(value, Format, _formatProvider, _styles, out result)
            : TimeSpan.TryParse(value, _formatProvider, out result);
#endif

    private void ExamineFormat()
    {
        try
        {
            string tmp = TimeSpan.Zero.ToString(Format, _formatProvider);

            if (_parseExact)
            {
                _ = TimeSpan.ParseExact(tmp, Format, _formatProvider, _styles);
            }
        }
        catch (FormatException e)
        {
            throw new ArgumentException(e.Message, e);
        }
    }
}

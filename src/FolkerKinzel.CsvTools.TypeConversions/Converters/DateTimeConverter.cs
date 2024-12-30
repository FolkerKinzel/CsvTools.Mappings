using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

/// <summary>
/// <see cref="CsvTypeConverter{T}"/> implementation for <see cref="DateTime"/>.
/// </summary>
public sealed class DateTimeConverter : CsvTypeConverter<DateTime>
{
    private const string DEFAULT_FORMAT = "s";
    private const string DATE_FORMAT = "d";

    private readonly IFormatProvider _formatProvider;
    private readonly string _format = DEFAULT_FORMAT;
    private readonly bool _parseExact;

    private const DateTimeStyles STYLE = DateTimeStyles.NoCurrentDateDefault | DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.RoundtripKind;


    public DateTimeConverter(bool throwing = true, IFormatProvider? formatProvider = null) : base(throwing)
        => _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;


    /// <summary>
    /// Initializes a new <see cref="DateTimeConverter"/> instance.
    /// </summary>
    /// <param name="format">
    /// A format string that is used for the <see cref="string"/> output of <see cref="DateTime"/> values. If the 
    /// option <paramref name="parseExact"/> is selected this format string is also used for parsing.</param>
    /// <param name="throwing">
    /// If <c>true</c> the method <see cref="CsvTypeConverter{T}.Parse"/> throws an exception when parsing fails, 
    /// otherwise it returns
    /// <see cref="CsvTypeConverter{T}.FallbackValue"/> in this case.
    /// </param>
    /// <param name="formatProvider">
    /// An <see cref="IFormatProvider"/> instance that provides culture-specific formatting information, or <c>null</c> for 
    /// <see cref="CultureInfo.InvariantCulture"/>.
    /// </param>
    /// <param name="parseExact">
    /// If <c>true</c> the text in the CSV file must exactly match the format string specified with <paramref name="format"/>.
    /// </param>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="format"/> is <c>null</c> and <paramref name="parseExact"/> is <c>true</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="format"/> is not a valid format string.</exception>
    public DateTimeConverter(
        string format,
        bool throwing = true,
        IFormatProvider? formatProvider = null,
        bool parseExact = false) : base(throwing)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _format = format;
        _parseExact = parseExact;
        ExamineFormat();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTimeConverter CreateDateConverter(bool throwing = true, IFormatProvider? formatProvider = null, bool parseExact = false)
        => new(DATE_FORMAT, throwing, formatProvider, parseExact);

    /// <inheritdoc/>
    public override bool AcceptsNull => false;

    /// <inheritdoc/>
    protected override string? DoConvertToString(DateTime value) => value.ToString(_format, _formatProvider);

    /// <inheritdoc/>
    public override bool TryParseValue(ReadOnlySpan<char> value, [NotNullWhen(true)] out DateTime result)
#if NET462 || NETSTANDARD2_0
        => _parseExact
            ? DateTime.TryParseExact(value.ToString(), _format, _formatProvider, STYLE, out result)
            : DateTime.TryParse(value.ToString(), _formatProvider, STYLE, out result);
#else
        => _parseExact
            ? DateTime.TryParseExact(value, _format, _formatProvider, STYLE, out result)
            : DateTime.TryParse(value, _formatProvider, STYLE, out result);
#endif


    private void ExamineFormat()
    {
        try
        {
            string tmp = DateTime.Now.ToString(_format, _formatProvider);

            if (_parseExact)
            {
                _ = DateTime.ParseExact(tmp, _format, _formatProvider, STYLE);
            }
        }
        catch (FormatException e)
        {
            throw new ArgumentException(e.Message, e);
        }
    }
}

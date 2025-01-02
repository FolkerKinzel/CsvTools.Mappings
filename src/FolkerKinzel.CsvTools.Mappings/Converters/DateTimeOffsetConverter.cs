using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace FolkerKinzel.CsvTools.Mappings.Converters;

/// <summary>
/// <see cref="TypeConverter{T}"/> implementation for <see cref="DateTimeOffset"/>.
/// </summary>
public sealed class DateTimeOffsetConverter : TypeConverter<DateTimeOffset>
{
    private const string DEFAULT_FORMAT = "O";
    private readonly IFormatProvider _formatProvider;
    private readonly bool _parseExact;

    private const DateTimeStyles STYLE = DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.RoundtripKind;

    /// <summary>
    /// Initializes a new <see cref="DateTimeOffsetConverter"/> instance.
    /// </summary>
    /// <param name="throwing">Sets the value of the 
    /// <see cref="TypeConverter{T}.Throwing"/> property.</param>
    /// <param name="formatProvider">
    /// An <see cref="IFormatProvider"/> instance that provides culture-specific formatting information, or <c>null</c> for 
    /// <see cref="CultureInfo.InvariantCulture"/>.
    /// </param>
    /// <remarks>This constructor initializes a <see cref="DateTimeOffsetConverter"/> instance that uses the format string
    /// "O". This constructor is much faster than its overload.</remarks>
    public DateTimeOffsetConverter(bool throwing = true, IFormatProvider? formatProvider = null) : base(throwing)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
    }

    /// <summary>
    /// Initializes a new <see cref="DateTimeOffsetConverter"/> instance and allows to specify a format string.
    /// </summary>
    /// 
    /// <param name="format">
    /// A format string that is used for the <see cref="string"/> output of <see cref="DateTimeOffset"/> values. If the 
    /// option <paramref name="parseExact"/> is selected, this format string is also used for parsing.</param>
    /// <param name="throwing">Sets the value of the 
    /// <see cref="TypeConverter{T}.Throwing"/> property.</param>
    /// <param name="formatProvider">
    /// An <see cref="IFormatProvider"/> instance that provides culture-specific formatting information, or <c>null</c> for 
    /// <see cref="CultureInfo.InvariantCulture"/>.
    /// </param>
    /// <param name="parseExact">
    /// If <c>true</c>, the text in the CSV file must exactly match the format string specified with <paramref name="format"/>.
    /// </param>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="format"/> is <c>null</c> and <paramref name="parseExact"/> is <c>true</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="format"/> is not a valid format string.</exception>
    public DateTimeOffsetConverter(
        string format,
        bool throwing = true,
        IFormatProvider? formatProvider = null,
        bool parseExact = false) : base(throwing)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        Format = format;
        _parseExact = parseExact;
        ExamineFormat(nameof(format));
    }

    /// <inheritdoc/>
    public override bool AllowsNull => false;

    /// <summary>
    /// The format string to use.
    /// </summary>
    public string Format { get; } = DEFAULT_FORMAT;

    /// <inheritdoc/>
    public override string? ConvertToString(DateTimeOffset value) => value.ToString(Format, _formatProvider);

    /// <inheritdoc/>
    public override bool TryParseValue(ReadOnlySpan<char> value, [NotNullWhen(true)] out DateTimeOffset result)
#if NET462 || NETSTANDARD2_0
        => _parseExact
            ? DateTimeOffset.TryParseExact(value.ToString(), Format, _formatProvider, STYLE, out result)
            : DateTimeOffset.TryParse(value.ToString(), _formatProvider, STYLE, out result);
#else
        => _parseExact
            ? DateTimeOffset.TryParseExact(value, Format, _formatProvider, STYLE, out result)
            : DateTimeOffset.TryParse(value, _formatProvider, STYLE, out result);
#endif

    private void ExamineFormat(string paramName)
    {
        try
        {
            string tmp = DateTimeOffset.Now.ToString(Format, _formatProvider);

            if (_parseExact)
            {
                _ = DateTimeOffset.ParseExact(tmp, Format, _formatProvider, STYLE);
            }
        }
        catch (FormatException e)
        {
            throw new ArgumentException(e.Message, paramName, e);
        }
    }
}

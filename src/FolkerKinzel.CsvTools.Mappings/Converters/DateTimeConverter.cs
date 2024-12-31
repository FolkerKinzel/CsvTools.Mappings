using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace FolkerKinzel.CsvTools.Mappings.Converters;

/// <summary>
/// <see cref="TypeConverter{T}"/> implementation for <see cref="DateTime"/>.
/// </summary>
public sealed class DateTimeConverter : TypeConverter<DateTime>
{
    private const DateTimeStyles STYLE = DateTimeStyles.NoCurrentDateDefault | DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.RoundtripKind;
    private const string DEFAULT_FORMAT = "s";
    //private const string DATE_FORMAT = "d";

    private readonly IFormatProvider _formatProvider;
    private readonly bool _parseExact;

    /// <summary>
    /// Initializes a new <see cref="DateTimeConverter"/> instance.
    /// </summary>
    /// <param name="throwing">Sets the value of the 
    /// <see cref="TypeConverter{T}.Throwing"/> property.</param>
    /// <param name="formatProvider">
    /// An <see cref="IFormatProvider"/> instance that provides culture-specific formatting information, or <c>null</c> for 
    /// <see cref="CultureInfo.InvariantCulture"/>.
    /// </param>
    /// <remarks>This constructor initializes a <see cref="DateTimeConverter"/> instance that uses the format string
    /// "s". This constructor is much faster than its overload.</remarks>
    public DateTimeConverter(bool throwing = true, IFormatProvider? formatProvider = null) : base(throwing)
        => _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;

    /// <summary>
    /// Initializes a new <see cref="DateTimeConverter"/> instance and allows to specify a format string.
    /// </summary>
    /// <param name="format">
    /// A format string that is used for the <see cref="string"/> output of <see cref="DateTime"/> values. If the 
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
    public DateTimeConverter(
        string format,
        bool throwing = true,
        IFormatProvider? formatProvider = null,
        bool parseExact = false) : base(throwing)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        Format = format;
        _parseExact = parseExact;
        ExamineFormat();
    }

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static DateTimeConverter CreateDateConverter(bool throwing = true, IFormatProvider? formatProvider = null, bool parseExact = false)
    //    => new(DATE_FORMAT, throwing, formatProvider, parseExact);

    /// <summary>
    /// The format string to use.
    /// </summary>
    public string Format { get; } = DEFAULT_FORMAT;

    /// <inheritdoc/>
    public override bool AcceptsNull => false;

    /// <inheritdoc/>
    public override string? ConvertToString(DateTime value) => value.ToString(Format, _formatProvider);

    /// <inheritdoc/>
    public override bool TryParseValue(ReadOnlySpan<char> value, [NotNullWhen(true)] out DateTime result)
#if NET462 || NETSTANDARD2_0
        => _parseExact
            ? DateTime.TryParseExact(value.ToString(), Format, _formatProvider, STYLE, out result)
            : DateTime.TryParse(value.ToString(), _formatProvider, STYLE, out result);
#else
        => _parseExact
            ? DateTime.TryParseExact(value, Format, _formatProvider, STYLE, out result)
            : DateTime.TryParse(value, _formatProvider, STYLE, out result);
#endif


    private void ExamineFormat()
    {
        try
        {
            string tmp = DateTime.Now.ToString(Format, _formatProvider);

            if (_parseExact)
            {
                _ = DateTime.ParseExact(tmp, Format, _formatProvider, STYLE);
            }
        }
        catch (FormatException e)
        {
            throw new ArgumentException(e.Message, e);
        }
    }
}

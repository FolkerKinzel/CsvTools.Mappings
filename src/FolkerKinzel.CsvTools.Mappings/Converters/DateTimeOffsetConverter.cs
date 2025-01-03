using FolkerKinzel.CsvTools.Mappings.Intls;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace FolkerKinzel.CsvTools.Mappings.Converters;

/// <summary>
/// <see cref="TypeConverter{T}"/> implementation for <see cref="DateTimeOffset"/>.
/// </summary>
public sealed class DateTimeOffsetConverter : TypeConverter<DateTimeOffset>
{
    /// <summary>
    /// The default value of <see cref="Styles"/>.
    /// </summary>
    public const DateTimeStyles DefaultStyle = DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.RoundtripKind;

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
    /// <param name="styles">
    /// A combined value of the <see cref="DateTimeStyles"/> enum that provides additional information for parsing.
    /// </param>
    /// <param name="parseExact">
    /// If <c>true</c>, the text in the CSV file must exactly match the format string specified with <paramref name="format"/>.
    /// </param>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="format"/> is <c>null</c> and <paramref name="parseExact"/> is <c>true</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The value of <paramref name="styles"/> is invalid.</exception>
    public DateTimeOffsetConverter(
#if !(NET462 || NETSTANDARD2_0 || NETSTANDARD2_1)
        [StringSyntax(StringSyntaxAttribute.DateTimeFormat)]
#endif
        string format = "O",
        bool throwing = true,
        IFormatProvider? formatProvider = null,
        DateTimeStyles styles = DefaultStyle,
        bool parseExact = false) : base(throwing, default)
    {
        FormatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        Format = format;

        if(parseExact)
        {
            ParseExact = parseExact;
            _ArgumentNullException.ThrowIfNull(format, nameof(format));
        }

        ValidateStyles(styles);
    }

    /// <inheritdoc/>
    public override bool AllowsNull => false;

    /// <summary>
    /// Gets the <see cref="IFormatProvider"/> instance that provides 
    /// culture-specific formatting information.
    /// </summary>
    public IFormatProvider FormatProvider { get; }

    /// <summary>
    /// The format string to use.
    /// </summary>
    public string Format { get; }

    /// <summary>
    /// Gets a combined value of the <see cref="DateTimeStyles"/> enum that provides additional information for parsing.
    /// </summary>
    public DateTimeStyles Styles { get; }

    /// <summary>
    /// Gets a value indicating whether the text in the CSV file must exactly match the format determined with <see cref="Format"/>.
    /// </summary>
    /// <value><c>true</c> if the text in the CSV file must exactly match the format determined with <see cref="Format"/>,
    /// <c>false</c>, if not.</value>
    public bool ParseExact { get; }

    /// <inheritdoc/>
    public override string? ConvertToString(DateTimeOffset value) => value.ToString(Format, FormatProvider);

    /// <inheritdoc/>
    public override bool TryParseValue(ReadOnlySpan<char> value, out DateTimeOffset result)
    {
#if NET462 || NETSTANDARD2_0
        return ParseExact
            ? DateTimeOffset.TryParseExact(value.ToString(), Format, FormatProvider, Styles, out result)
            : DateTimeOffset.TryParse(value.ToString(), FormatProvider, Styles, out result);
#else
        return ParseExact
            ? DateTimeOffset.TryParseExact(value, Format, FormatProvider, Styles, out result)
            : DateTimeOffset.TryParse(value, FormatProvider, Styles, out result);
#endif
    }

    private static void ValidateStyles(DateTimeStyles styles)
    {
        const DateTimeStyles InvalidDateTimeStyles = ~(DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite
                                                     | DateTimeStyles.AllowInnerWhite | DateTimeStyles.NoCurrentDateDefault
                                                     | DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeLocal
                                                     | DateTimeStyles.AssumeUniversal | DateTimeStyles.RoundtripKind);

        if ((styles & InvalidDateTimeStyles) != 0
          || styles.HasFlag(DateTimeStyles.AssumeLocal | DateTimeStyles.AssumeUniversal)
          || styles.HasFlag(DateTimeStyles.NoCurrentDateDefault))
        {
            throw new ArgumentOutOfRangeException(nameof(styles));
        }
    }
}

using FolkerKinzel.CsvTools.Mappings.TypeConverters.Interfaces;
using FolkerKinzel.CsvTools.Mappings.Intls;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace FolkerKinzel.CsvTools.Mappings.TypeConverters;

/// <summary>
/// <see cref="TypeConverter{T}"/> implementation for <see cref="TimeSpan"/>.
/// </summary>
public sealed class TimeSpanConverter : TypeConverter<TimeSpan>, ILocalizable
{
    /// <summary>
    /// Initializes a new <see cref="TimeSpanConverter"/> instance.
    /// </summary>
    /// <param name="formatProvider">
    /// An <see cref="IFormatProvider"/> instance that provides culture-specific formatting information, or <c>null</c> for 
    /// <see cref="CultureInfo.InvariantCulture"/>.
    /// </param>
    /// <param name="format">
    /// A format string that is used for the <see cref="string"/> output of <see cref="TimeSpan"/> values. If the 
    /// option <paramref name="parseExact"/> is selected this format string is also used for parsing.</param>
    /// <param name="parseExact">
    /// If <c>true</c> the text in the CSV file must exactly match the format string specified with <paramref name="format"/>,
    /// if <c>false</c>, it doesn't.
    /// </param>
    /// <param name="styles">
    /// A value of the <see cref="TimeSpanStyles"/> enum that provides additional information for parsing. (Becomes evaluated
    /// only if <paramref name="parseExact"/> is <c>true</c>.)
    /// </param>
    /// 
    /// <param name="throwing">Sets the value of the <see cref="TypeConverter{T}.Throwing"/> property.</param>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="format"/> is <c>null</c> and 
    /// <paramref name="parseExact"/> is <c>true</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The value of <paramref name="styles"/> is invalid and 
    /// <paramref name="parseExact"/> is <c>true</c>.</exception>
    public TimeSpanConverter(
        IFormatProvider? formatProvider = null,
#if !(NET462 || NETSTANDARD2_0 || NETSTANDARD2_1)
        [StringSyntax(StringSyntaxAttribute.TimeSpanFormat)]
#endif
        string? format = "g",
        bool parseExact = false,
        TimeSpanStyles styles = TimeSpanStyles.None,
        bool throwing = true) : base(default, throwing)
    {
        FormatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        Styles = styles;
        Format = format;

        if (parseExact)
        {
            ParseExact = parseExact;
            _ArgumentNullException.ThrowIfNull(format, nameof(format));
            ValidateStyles(styles);
        }
    }

    /// <inheritdoc/>
    public override bool AcceptsNull => false;

    /// <inheritdoc/>
    public IFormatProvider FormatProvider { get; }

    /// <summary>
    /// Gets the format string to use.
    /// </summary>
    public string? Format { get; }

    /// <summary>
    /// Gets a value of the <see cref="TimeSpanStyles"/> enum that provides additional information for parsing.
    /// </summary>
    public TimeSpanStyles Styles { get; }

    /// <summary>
    /// Gets a value indicating whether the text in the CSV file must exactly match the format determined with <see cref="Format"/>.
    /// </summary>
    /// <value><c>true</c> if the text in the CSV file must exactly match the format determined with <see cref="Format"/>,
    /// <c>false</c>, if not.</value>
    public bool ParseExact { get; }

    /// <inheritdoc/>
    /// <exception cref="FormatException">The value of <see cref="Format"/> is not recognized 
    /// or is not supported.</exception>
    public override string? ConvertToString(TimeSpan value) => value.ToString(Format, FormatProvider);

    /// <inheritdoc/>
    public override bool TryParse(ReadOnlySpan<char> value, [NotNullWhen(true)] out TimeSpan result)
    { 
#if NET462 || NETSTANDARD2_0
        return ParseExact
            ? TimeSpan.TryParseExact(value.ToString(), Format, FormatProvider, Styles, out result)
            : TimeSpan.TryParse(value.ToString(), FormatProvider, out result);
#else
        return ParseExact
            ? TimeSpan.TryParseExact(value, Format, FormatProvider, Styles, out result)
            : TimeSpan.TryParse(value, FormatProvider, out result);
#endif
    }

    private static void ValidateStyles(TimeSpanStyles styles)
    {
        if ((styles & ~TimeSpanStyles.AssumeNegative) != TimeSpanStyles.None)
        {
            throw new ArgumentOutOfRangeException(nameof(styles));
        }
    }
}

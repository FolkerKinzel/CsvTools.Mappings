using FolkerKinzel.CsvTools.Mappings.Intls;
using FolkerKinzel.CsvTools.Mappings.TypeConverters.Interfaces;
using System.Globalization;

namespace FolkerKinzel.CsvTools.Mappings.TypeConverters;

/// <summary>
/// <see cref="TypeConverter{T}"/> implementation for <see cref="System.DateTime"/>.
/// </summary>
public sealed class DateTimeConverter : TypeConverter<DateTime>, ILocalizable
{
    /// <summary>
    /// Initializes a new <see cref="DateTimeConverter"/> instance.
    /// </summary>
    /// <param name="formatProvider">
    /// An <see cref="IFormatProvider"/> instance that provides culture-specific formatting
    /// information, or <c>null</c> for <see cref="CultureInfo.InvariantCulture"/>.
    /// </param>
    /// <param name="format">
    /// A format string that is used for the <see cref="string"/> output of 
    /// <see cref="System.DateTime"/> values. If <paramref name="parseExact"/> is <c>true</c>,
    /// this format string is also used for parsing.</param>
    /// <param name="styles">
    /// A combined value of the <see cref="DateTimeStyles"/> enum that provides additional 
    /// information for parsing.
    /// </param>
    /// <param name="parseExact">
    /// If <c>true</c> the text in the CSV file must exactly match the format string specified 
    /// with <paramref name="format"/>, if <c>false</c>, it doesn't.
    /// </param>
    /// <param name="throwing">Sets the value of the <see cref="TypeConverter{T}.Throwing"/> 
    /// property.</param>
    /// <param name="defaultValue">Sets the value of the <see cref="TypeConverter{T}.DefaultValue"/>
    /// property.</param>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="format"/> is <c>null</c> and 
    /// <paramref name="parseExact"/> is <c>true</c>.</exception>
    public DateTimeConverter(
        IFormatProvider? formatProvider = null,
#if !(NET462 || NETSTANDARD2_0 || NETSTANDARD2_1)
        [StringSyntax(StringSyntaxAttribute.DateTimeFormat)]
#endif
        string? format = "s",
        DateTimeStyles styles = DateTimeStyles.NoCurrentDateDefault 
                              | DateTimeStyles.AllowWhiteSpaces 
                              | DateTimeStyles.RoundtripKind,
        bool parseExact = false,
        bool throwing = true,
        DateTime defaultValue = default) : base(throwing, defaultValue)
    {
        FormatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        Format = format;
        Styles = styles;

        if (parseExact)
        {
            ParseExact = parseExact;
            _ArgumentNullException.ThrowIfNull(format, nameof(format));
        }
    }

    /// <inheritdoc/>
    public IFormatProvider FormatProvider { get; }

    /// <summary>
    /// The format string to use.
    /// </summary>
    public string? Format { get; }

    /// <summary>
    /// Gets a combined value of the <see cref="DateTimeStyles"/> enum that provides 
    /// additional information for parsing.
    /// </summary>
    public DateTimeStyles Styles { get; }

    /// <summary>
    /// Gets a value indicating whether the text in the CSV file must exactly match 
    /// the format determined with <see cref="Format"/>.
    /// </summary>
    /// <value><c>true</c> if the text in the CSV file must exactly match the format 
    /// determined with <see cref="Format"/>, <c>false</c>, if not.</value>
    public bool ParseExact { get; }

    /// <inheritdoc/>
    public override bool AcceptsNull => false;

    /// <inheritdoc/>
    /// <exception cref="FormatException">
    /// <para>The length of <see cref="Format"/> is 1, and it is not one of the format
    /// specifier characters defined for <see cref="DateTimeFormatInfo"/>.</para>
    /// <para>-or-</para>
    /// <para><see cref="Format"/> does not contain a valid custom format pattern.</para>
    /// </exception>
    public override string? ConvertToString(DateTime value)
    {
        try
        {
            return value.ToString(Format, FormatProvider);
        }
        catch (ArgumentOutOfRangeException)
        {
            return null;
        };
    }

    /// <inheritdoc/>
    public override bool TryParse(ReadOnlySpan<char> value, out DateTime result)
    {
#if NET462 || NETSTANDARD2_0
        return ParseExact
            ? DateTime.TryParseExact(value.ToString(), Format, FormatProvider, Styles, out result)
            : DateTime.TryParse(value.ToString(), FormatProvider, Styles, out result);
#else
        return ParseExact
            ? DateTime.TryParseExact(value, Format, FormatProvider, Styles, out result)
            : DateTime.TryParse(value, FormatProvider, Styles, out result);
#endif
    }
}

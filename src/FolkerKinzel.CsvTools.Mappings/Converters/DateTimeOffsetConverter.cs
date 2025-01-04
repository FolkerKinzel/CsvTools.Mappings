﻿using FolkerKinzel.CsvTools.Mappings.Intls;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace FolkerKinzel.CsvTools.Mappings.Converters;

/// <summary>
/// <see cref="TypeConverter{T}"/> implementation for <see cref="DateTimeOffset"/>.
/// </summary>
public sealed class DateTimeOffsetConverter : TypeConverter<DateTimeOffset>
{
    /// <summary>
    /// Initializes a new <see cref="DateTimeOffsetConverter"/> instance and allows to specify a format string.
    /// </summary>
    /// 
    /// <param name="formatProvider">
    /// An <see cref="IFormatProvider"/> instance that provides culture-specific formatting information, or <c>null</c> for 
    /// <see cref="CultureInfo.InvariantCulture"/>.
    /// </param>
    /// <param name="format">
    /// A format string that is used for the <see cref="string"/> output of <see cref="DateTimeOffset"/> values. If the 
    /// option <paramref name="parseExact"/> is selected, this format string is also used for parsing.</param>
    /// <param name="styles">
    /// A combined value of the <see cref="DateTimeStyles"/> enum that provides additional information for parsing.
    /// </param>
    /// <param name="parseExact">
    /// If <c>true</c>, the text in the CSV file must exactly match the format string specified with <paramref name="format"/>.
    /// </param>
    /// <param name="throwing">Sets the value of the 
    /// <see cref="TypeConverter{T}.Throwing"/> property.</param>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="format"/> is <c>null</c> and <paramref name="parseExact"/> is <c>true</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The value of <paramref name="styles"/> is invalid.</exception>
    public DateTimeOffsetConverter(
        IFormatProvider? formatProvider = null,
#if !(NET462 || NETSTANDARD2_0 || NETSTANDARD2_1)
        [StringSyntax(StringSyntaxAttribute.DateTimeFormat)]
#endif
        string format = "O",
        DateTimeStyles styles = DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.RoundtripKind,
        bool parseExact = false,
        bool throwing = true) : base(default, throwing)
    {
        FormatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        Format = format;

        if (parseExact)
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
    /// <exception cref="FormatException">
    /// <para>The length of <see cref="Format"/> is 1, and it is not one of the format specifier characters defined 
    /// for <see cref="DateTimeFormatInfo"/>.</para>
    /// <para>-or-</para>
    /// <para><see cref="Format"/> does not contain a valid custom format pattern.</para>
    /// </exception>
    public override string? ConvertToString(DateTimeOffset value)
    {
        try
        {
            return value.ToString(Format, FormatProvider);
        }
        catch(ArgumentOutOfRangeException)
        {
            return null;
        };
    }

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

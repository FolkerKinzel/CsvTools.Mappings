using FolkerKinzel.CsvTools.Mappings.TypeConverters.Interfaces;
using FolkerKinzel.CsvTools.Mappings.Intls;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace FolkerKinzel.CsvTools.Mappings.TypeConverters;

/// <summary>
/// <see cref="TypeConverter{T}"/> implementation for <see cref="TimeOnly"/>.
/// </summary>
public sealed class TimeOnlyConverter : TypeConverter<TimeOnly>, ILocalizable
{
    /// <summary>
    /// Initializes a new <see cref="TimeOnlyConverter"/> instance.
    /// </summary>
    /// <param name="formatProvider">
    /// An <see cref="IFormatProvider"/> instance that provides culture-specific formatting information, or <c>null</c> for 
    /// <see cref="CultureInfo.InvariantCulture"/>.
    /// </param>
    /// <param name="format">
    /// A format string that is used for the <see cref="string"/> output of <see cref="TimeOnly"/> values. If 
    /// <paramref name="format"/> is not <c>null</c>, this format string is also used for parsing. The accepted standard formats 
    /// are 'r', 'R', 'o', 'O', 't' and 'T'. </param>
    /// <param name="styles">
    /// A combined value of the <see cref="DateTimeStyles"/> enum that provides additional information for parsing.
    /// </param>
    /// <param name="parseExact">
    /// If <c>true</c> the text in the CSV file must exactly match the format string specified with <paramref name="format"/>,
    /// if <c>false</c>, it doesn't.
    /// </param>
    /// <param name="throwing">Sets the value of the <see cref="TypeConverter{T}.Throwing"/> property.</param>
    /// 
    /// <exception cref="ArgumentOutOfRangeException">The value of <paramref name="styles"/> is invalid.</exception>
    public TimeOnlyConverter(
        IFormatProvider? formatProvider = null,
#if !(NET462 || NETSTANDARD2_0 || NETSTANDARD2_1)
        [StringSyntax(StringSyntaxAttribute.TimeOnlyFormat)]
#endif
        string? format = "T",
        DateTimeStyles styles = DateTimeStyles.AllowWhiteSpaces,
        bool parseExact = false,
        bool throwing = true) : base(default, throwing)
    {
        ValidateStyles(styles);
        FormatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        Format = format;
        Styles = styles;
        ParseExact = parseExact;
    }

    /// <inheritdoc/>
    public IFormatProvider FormatProvider { get; }

    /// <summary>
    /// The format string to use.
    /// </summary>
    public string? Format { get; }

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
    public override bool AcceptsNull => false;

    /// <inheritdoc/>
    /// <exception cref="FormatException">
    /// <para>The length of <see cref="Format"/> is 1, and it is not one of the standard format specifier characters 
    /// defined for <see cref="TimeOnly"/> ('r', 'R', 'o', 'O', 't' or 'T').</para>
    /// <para>-or-</para>
    /// <para><see cref="Format"/> does not contain a valid custom format pattern.</para>
    /// </exception>
    public override string? ConvertToString(TimeOnly value) => value.ToString(Format, FormatProvider);
   
    /// <inheritdoc/>
    public override bool TryParseValue(ReadOnlySpan<char> value, out TimeOnly result)
    {
        return ParseExact
            ? TimeOnly.TryParseExact(value, Format.AsSpan(), FormatProvider, Styles, out result)
            : TimeOnly.TryParse(value, FormatProvider, Styles, out result);
    }

    private static void ValidateStyles(DateTimeStyles styles)
    {
        if ((styles & ~DateTimeStyles.AllowWhiteSpaces) != 0)
        {
            throw new ArgumentOutOfRangeException(nameof(styles));
        }
    }
}

using FolkerKinzel.CsvTools.Mappings.Resources;
using System;
using System.Globalization;

namespace FolkerKinzel.CsvTools.Mappings.Converters;

/// <summary>
/// <see cref="TypeConverter{T}"/> implementation for <see cref="short"/>.
/// </summary>
public sealed class Int16Converter : TypeConverter<short>, IHexConverter<short>
{
    /// <summary> Initializes a new <see cref="Int16Converter"/> instance.</summary>
    /// <param name="formatProvider">
    /// An <see cref="IFormatProvider"/> instance that provides culture-specific formatting information, or <c>null</c> for 
    /// <see cref="CultureInfo.InvariantCulture"/>.
    /// </param>
    /// <param name="format">
    /// A format string that is used for the <see cref="string"/> output of <see cref="short"/> values.
    /// The format strings "R" and "r" are not supported.
    /// </param>
    /// <param name="styles">
    /// A combined value of the <see cref="NumberStyles"/> enum that provides additional 
    /// information for parsing.
    /// </param>
    /// <param name="throwing">Sets the value of the 
    /// <see cref="TypeConverter{T}.Throwing"/> property.</param>
    /// <exception cref="ArgumentException">
    /// <paramref name="format"/> is "R" or "r".
    /// </exception>
    public Int16Converter(IFormatProvider? formatProvider = null,
#if !(NET462 || NETSTANDARD2_0 || NETSTANDARD2_1)
        [StringSyntax(StringSyntaxAttribute.NumericFormat)]
#endif
                         string? format = null,
                         NumberStyles styles = NumberStyles.Any,
                         bool throwing = true) 
        : base(default, throwing)
    {
        ValidateFormat(format);
        FormatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        Format = format;
        Styles = styles;
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
    public string? Format { get; private set; }

    /// <summary>
    /// Gets a combined value of the <see cref="NumberStyles"/> enum that 
    /// provides additional information for parsing.
    /// </summary>
    public NumberStyles Styles { get; private set; }

    /// <inheritdoc/>
    public TypeConverter<short> ToHexConverter()
    {
        var clone = (Int16Converter)Clone();
        clone.Styles = Styles | NumberStyles.AllowHexSpecifier;
        clone.Format = "X";
        return clone;
    }

    /// <inheritdoc/>
    public object Clone() => new Int16Converter(FormatProvider, Format, Styles, Throwing);

    /// <inheritdoc/>
    public override string? ConvertToString(short value) => value.ToString(Format, FormatProvider);

    /// <inheritdoc/>
    public override bool TryParseValue(ReadOnlySpan<char> value, out short result)
#if NET462 || NETSTANDARD2_0
        => short.TryParse(value.ToString(), Styles, FormatProvider, out result);
#else
        => short.TryParse(value, Styles, FormatProvider, out result);
#endif

    private static void ValidateFormat(string? format)
    {
        if (StringComparer.OrdinalIgnoreCase.Equals("R", format))
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Res.FormatStringNotSupported, format), nameof(format));
        }
    }
}

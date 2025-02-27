﻿using FolkerKinzel.CsvTools.Mappings.Resources;
using FolkerKinzel.CsvTools.Mappings.TypeConverters.Interfaces;
using System.Globalization;

namespace FolkerKinzel.CsvTools.Mappings.TypeConverters;

/// <summary>
/// <see cref="TypeConverter{T}"/> implementation for <see cref="decimal"/>.
/// </summary>
/// <threadsafety static="true" instance="true"/>
public sealed class DecimalConverter : TypeConverter<decimal>, ILocalizable
{
    /// <summary>Initializes a new <see cref="DecimalConverter"/> instance.</summary>
    /// <param name="formatProvider">
    /// An <see cref="IFormatProvider"/> instance that provides culture-specific 
    /// formatting information, or <c>null</c> for <see cref="CultureInfo.InvariantCulture"/>.
    /// </param>
    /// <param name="format">
    /// A format string that is used for the <see cref="string"/> output of <see cref="byte"/>
    /// values.
    /// The format strings "D", "d", "R", "r", "X", "x" are not supported.
    /// </param>
    /// <param name="styles">
    /// A combined value of the <see cref="NumberStyles"/> enum that provides additional 
    /// information for parsing.
    /// </param>
    /// <param name="throwing">Sets the value of the 
    /// <see cref="TypeConverter{T}.Throwing"/> property.</param>
    /// <param name="defaultValue">Sets the value of the <see cref="TypeConverter{T}.DefaultValue"/>
    /// property.</param>
    /// 
    /// <exception cref="ArgumentException"><paramref name="format"/> is "D", "d", "R", "r", "X", or "x".
    /// </exception>
    public DecimalConverter(IFormatProvider? formatProvider = null,
#if !(NET462 || NETSTANDARD2_0 || NETSTANDARD2_1)
        [StringSyntax(StringSyntaxAttribute.NumericFormat)]
#endif
                            string? format = "G",
                            NumberStyles styles = NumberStyles.Any,
                            bool throwing = true,
                            decimal defaultValue = default)
        : base(throwing, defaultValue)
    {
        ValidateFormat(format);
        FormatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        Format = format;
        Styles = styles;
    }

    /// <inheritdoc/>
    public override bool AcceptsNull => false;

    /// <inheritdoc/>
    public IFormatProvider FormatProvider { get; }

    /// <summary>
    /// The format string to use.
    /// </summary>
    public string? Format { get; }

    /// <summary>
    /// Gets a combined value of the <see cref="NumberStyles"/> enum that 
    /// provides additional information for parsing.
    /// </summary>
    public NumberStyles Styles { get; }

    /// <inheritdoc/>
    public override string? ConvertToString(decimal value) 
        => value.ToString(Format, FormatProvider);

    /// <inheritdoc/>
    public override bool TryParse(ReadOnlySpan<char> value, out decimal result)
#if NET462 || NETSTANDARD2_0
        => decimal.TryParse(value.ToString(), Styles, FormatProvider, out result);
#else
        => decimal.TryParse(value, Styles, FormatProvider, out result);
#endif

    private static void ValidateFormat(string? format)
    {
        ReadOnlySpan<char> span = format.AsSpan();

        if (span.Length == 1 && span[0].ToUpperInvariant() is 'D' or 'R' or 'X')
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, 
                                                      Res.FormatStringNotSupported,
                                                      format),
                                        nameof(format));
        }
    }
}

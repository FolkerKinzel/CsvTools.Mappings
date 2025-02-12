using FolkerKinzel.CsvTools.Mappings.Intls.TypeConverters;
using FolkerKinzel.CsvTools.Mappings.Resources;
using FolkerKinzel.CsvTools.Mappings.TypeConverters.Interfaces;
using System.Globalization;

namespace FolkerKinzel.CsvTools.Mappings.TypeConverters;

/// <summary>
/// <see cref="TypeConverter{T}"/> implementation for <see cref="ushort"/>.
/// </summary>
/// <threadsafety static="true" instance="true"/>
[CLSCompliant(false)]
public sealed class UInt16Converter
    : TypeConverter<ushort>, IHexConverter<ushort>, ILocalizable, IAsHexConverter
{
    /// <summary> Initializes a new <see cref="UInt16Converter"/> instance.</summary>
    /// <param name="formatProvider">
    /// An <see cref="IFormatProvider"/> instance that provides culture-specific 
    /// formatting information, or <c>null</c> for <see cref="CultureInfo.InvariantCulture"/>.
    /// </param>
    /// <param name="format">
    /// A format string that is used for the <see cref="string"/> output of <see cref="ushort"/>
    /// values. The format strings "R" and "r" are not supported.
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
    /// <exception cref="ArgumentException"> <paramref name="format"/> is "R" or "r".
    /// </exception>
    public UInt16Converter(IFormatProvider? formatProvider = null,
#if !(NET462 || NETSTANDARD2_0 || NETSTANDARD2_1)
        [StringSyntax(StringSyntaxAttribute.NumericFormat)]
#endif
                           string? format = "G",
                           NumberStyles styles = NumberStyles.Any,
                           bool throwing = true,
                           ushort defaultValue = default)
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

    /// <inheritdoc/>
    public string? Format { get; private set; }

    /// <inheritdoc/>
    public NumberStyles Styles { get; private set; }

    /// <inheritdoc/>
    public TypeConverter<ushort> ToHexConverter()
        => HexConverter.CreateHexConverter<ushort, UInt16Converter>(this);

    void IAsHexConverter.AsHexConverter()
    {
        Styles = HexConverter.ToHexStyle(Styles);
        Format = HexConverter.HEX_FORMAT;
    }

    /// <inheritdoc/>
    public object Clone() => MemberwiseClone();

    /// <inheritdoc/>
    public override string? ConvertToString(ushort value)
        => value.ToString(Format, FormatProvider);

    /// <inheritdoc/>
    public override bool TryParse(ReadOnlySpan<char> value, out ushort result)
#if NET462 || NETSTANDARD2_0
        => ushort.TryParse(value.ToString(), Styles, FormatProvider, out result);
#else
        => ushort.TryParse(value, Styles, FormatProvider, out result);
#endif

    private static void ValidateFormat(string? format)
    {
        if (StringComparer.OrdinalIgnoreCase.Equals("R", format))
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                                                      Res.FormatStringNotSupported,
                                                      format),
                                        nameof(format));
        }
    }
}

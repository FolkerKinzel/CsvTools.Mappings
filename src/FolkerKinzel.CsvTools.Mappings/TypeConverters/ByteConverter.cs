using FolkerKinzel.CsvTools.Mappings.Intls.TypeConverters;
using FolkerKinzel.CsvTools.Mappings.Resources;
using FolkerKinzel.CsvTools.Mappings.TypeConverters.Interfaces;
using System.Globalization;

namespace FolkerKinzel.CsvTools.Mappings.TypeConverters;

/// <summary>
/// <see cref="TypeConverter{T}"/> implementation for <see cref="byte"/>.
/// </summary>
/// 
/// <example>
/// <para>
/// Using <see cref="MultiColumnTypeConverter{T}"/> to create and parse a CSV file.
/// </para>
/// <para>
/// (For the sake of easier readability exception handling has been omitted.)
/// </para>
/// <img src="images\MultiColumnConverter.png"/>
/// <code language="cs" source="../Examples/MultiColumnConverterExample.cs"/>
/// </example>
public sealed class ByteConverter 
    : TypeConverter<byte>, IHexConverter<byte>, ILocalizable, IAsHexConverter
{
    /// <summary>Initializes a new <see cref="ByteConverter"/> instance.</summary>
    /// <param name="formatProvider">
    /// An <see cref="IFormatProvider"/> instance that provides culture-specific 
    /// formatting information, or <c>null</c> for <see cref="CultureInfo.InvariantCulture"/>.
    /// </param>
    /// <param name="format">
    /// A format string that is used for the <see cref="string"/> output of <see cref="byte"/>
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
    /// <example>
    /// <para>
    /// Using <see cref="MultiColumnTypeConverter{T}"/> to create and parse a CSV file.
    /// </para>
    /// <para>
    /// (For the sake of easier readability exception handling has been omitted.)
    /// </para>
    /// <img src="images\MultiColumnConverter.png"/>
    /// <code language="cs" source="../Examples/MultiColumnConverterExample.cs"/>
    /// </example>
    /// 
    /// <exception cref="ArgumentException"><paramref name="format"/> is "R" or "r".</exception>
    public ByteConverter(IFormatProvider? formatProvider = null,
#if !(NET462 || NETSTANDARD2_0 || NETSTANDARD2_1)
        [StringSyntax(StringSyntaxAttribute.NumericFormat)]
#endif
                         string? format = "G",
                         NumberStyles styles = NumberStyles.Any,
                         bool throwing = true,
                         byte defaultValue = default)
        : base(throwing, defaultValue)
    {
        ValidateFormat(format);
        FormatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        Format = format;
        Styles = styles;
    }

    /// <inheritdoc/>
    public IFormatProvider FormatProvider { get; }

    /// <inheritdoc/>
    public string? Format { get; private set; }

    /// <inheritdoc/>
    public NumberStyles Styles { get; private set; }

    /// <inheritdoc/>
    /// 
    /// <example>
    /// <para>
    /// Using <see cref="MultiColumnTypeConverter{T}"/> to create and parse a CSV file.
    /// </para>
    /// <para>
    /// (For the sake of easier readability exception handling has been omitted.)
    /// </para>
    /// <img src="images\MultiColumnConverter.png"/>
    /// <code language="cs" source="../Examples/MultiColumnConverterExample.cs"/>
    /// </example>
    public TypeConverter<byte> ToHexConverter()
        => HexConverter.CreateHexConverter<byte, ByteConverter>(this);

    void IAsHexConverter.AsHexConverter()
    {
        Styles = HexConverter.ToHexStyle(Styles);
        Format = HexConverter.HEX_FORMAT;
    }

    /// <inheritdoc/>
    public object Clone() => MemberwiseClone();

    /// <inheritdoc/>
    public override bool AcceptsNull => false;

    /// <inheritdoc/>
    public override string? ConvertToString(byte value)
        => value.ToString(Format, FormatProvider);

    /// <inheritdoc/>
    public override bool TryParse(ReadOnlySpan<char> value, out byte result)
#if NET462 || NETSTANDARD2_0
        => byte.TryParse(value.ToString(), Styles, FormatProvider, out result);
#else
        => byte.TryParse(value, Styles, FormatProvider, out result);
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

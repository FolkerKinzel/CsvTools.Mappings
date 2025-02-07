using FolkerKinzel.CsvTools.Mappings.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace FolkerKinzel.CsvTools.Mappings.Converters;

/// <summary>
/// <see cref="TypeConverter{T}"/> implementation for <see cref="Guid"/>.
/// </summary>
public sealed class GuidConverter : TypeConverter<Guid>
{
    /// <summary>
    /// Initializes a new <see cref="GuidConverter"/> instance and allows
    /// to specify a format string.
    /// </summary>
    /// <param name="format">A format string that is used for the <see cref="string"/> output 
    /// of <see cref="Guid"/> values. The accepted values 
    /// are "N", "D", "B", "P", "X", <c>null</c> and <see cref="string.Empty"/>.</param>
    /// <param name="throwing">Sets the value of the 
    /// <see cref="TypeConverter{T}.Throwing"/> property.</param>
    /// <exception cref="ArgumentException">
    /// <paramref name="format"/> is not valid.
    /// </exception>
    public GuidConverter(
#if !(NET462 || NETSTANDARD2_0 || NETSTANDARD2_1)
        [StringSyntax(StringSyntaxAttribute.GuidFormat)]
#endif
        string? format = "D",
        bool throwing = true) : base(default, throwing)
    {
        ValidateFormat(format);
        Format = format;
    }

    /// <inheritdoc/>
    public override bool AcceptsNull => false;

    /// <summary>
    /// The format string to use.
    /// </summary>
    public string? Format { get; }

    /// <inheritdoc/>
    public override string? ConvertToString(Guid value) => value.ToString(Format, CultureInfo.InvariantCulture);

    /// <inheritdoc/>
    public override bool TryParseValue(ReadOnlySpan<char> value, out Guid result)
    { 
#if NET462 || NETSTANDARD2_0
        return Guid.TryParse(value.ToString(), out result);
#else
        return Guid.TryParse(value, out result);
#endif
    }

    private static void ValidateFormat(string? format)
    {
        switch (format)
        {
            case "N":
            case "D":
            case "B":
            case "P":
            case "X":
            case null:
            case "":
                return;
            default:
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Res.FormatStringNotSupported, format), nameof(format));
        }
    }
}

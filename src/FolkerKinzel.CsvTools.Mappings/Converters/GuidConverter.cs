using FolkerKinzel.CsvTools.Mappings.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace FolkerKinzel.CsvTools.Mappings.Converters;

/// <summary>
/// <see cref="TypeConverter{T}"/> implementation for <see cref="Guid"/>.
/// </summary>
public sealed class GuidConverter : TypeConverter<Guid>
{
    private const string DEFAULT_FORMAT = "D";

    /// <summary>
    /// Initializes a new <see cref="GuidConverter"/> instance.
    /// </summary>
    /// <param name="throwing">Sets the value of the 
    /// <see cref="TypeConverter{T}.Throwing"/> property.</param>
    /// <remarks>
    /// Instances initialized with this constructor use the format string "D".
    /// This constructor is much faster than its overload.
    /// </remarks>
    public GuidConverter(bool throwing = true) : base(throwing, default) { }

    /// <summary>
    /// Initializes a new <see cref="GuidConverter"/> instance and allows
    /// to specify a format string.
    /// </summary>
    /// <param name="format">A format string or <c>null</c> for "D".</param>
    /// <param name="throwing">Sets the value of the 
    /// <see cref="TypeConverter{T}.Throwing"/> property.</param>
    /// <exception cref="ArgumentException">
    /// <paramref name="format"/> is not valid.
    /// </exception>
    public GuidConverter(
        string? format,
        bool throwing = true) : base(throwing, default)
    {
        Format = format;
        ExamineFormat(nameof(format));
    }

    /// <inheritdoc/>
    public override bool AllowsNull => false;

    /// <summary>
    /// The format string to use.
    /// </summary>
    public string? Format { get; } = DEFAULT_FORMAT;

    /// <inheritdoc/>
    public override string? ConvertToString(Guid value) => value.ToString(Format, CultureInfo.InvariantCulture);

    /// <inheritdoc/>
    public override bool TryParseValue(ReadOnlySpan<char> value, out Guid result)
    { 
#if NET462 || NETSTANDARD2_0
        result = default;
        return !value.IsWhiteSpace() && Guid.TryParse(value.ToString(), out result);
#else
        return Guid.TryParse(value, out result);
#endif
    }

    private void ExamineFormat(string parameterName)
    {
        switch (Format)
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
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Res.FormatStringNotSupported, Format), parameterName);
        }
    }
}

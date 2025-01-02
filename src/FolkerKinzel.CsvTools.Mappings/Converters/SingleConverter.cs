using System.Globalization;

namespace FolkerKinzel.CsvTools.Mappings.Converters;

/// <summary>
/// <see cref="TypeConverter{T}"/> implementation for <see cref="float"/>.
/// </summary>
/// <param name="throwing">Sets the value of the 
/// <see cref="TypeConverter{T}.Throwing"/> property.</param>
/// <param name="formatProvider">
/// An <see cref="IFormatProvider"/> instance that provides culture-specific formatting information, or <c>null</c> for 
/// <see cref="CultureInfo.InvariantCulture"/>.
/// </param>
public sealed class SingleConverter(bool throwing = true, IFormatProvider? formatProvider = null)
    : TypeConverter<float>(throwing)
{
    private readonly IFormatProvider? _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;

    private const string FORMAT = "G9";
    private const NumberStyles STYLE = NumberStyles.Any;

    /// <inheritdoc/>
    public override bool AllowsNull => false;

    /// <inheritdoc/>
    public override string? ConvertToString(float value) 
        => value.ToString(FORMAT, _formatProvider);

    /// <inheritdoc/>
    public override bool TryParseValue(ReadOnlySpan<char> value, out float result)
#if NET462 || NETSTANDARD2_0
        => float.TryParse(value.ToString(), STYLE, _formatProvider, out result);
#else
        => float.TryParse(value, STYLE, _formatProvider, out result);
#endif
}

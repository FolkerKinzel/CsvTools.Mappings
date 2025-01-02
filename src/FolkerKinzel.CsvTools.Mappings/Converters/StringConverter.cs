using System.Diagnostics.CodeAnalysis;

namespace FolkerKinzel.CsvTools.Mappings.Converters;

/// <summary>
/// <see cref="TypeConverter{T}"/> implementation for <see cref="string"/>.
/// </summary>
public sealed class StringConverter : TypeConverter<string?>
{
    public static TypeConverter<string?> CreateNullable() => new StringConverter(null);
    public static TypeConverter<string> CreateNonNullable() => new StringConverter("")!;

    /// Initializes a new <see cref="StringConverter"/> instance.
    /// <param name="nullable"><c>true</c> to set <see cref="TypeConverter{T}.FallbackValue"/>
    /// to <c>null</c>; <c>false</c> to have <see cref="string.Empty"/> as 
    /// <see cref="TypeConverter{T}.FallbackValue"/>.</param>
    private StringConverter(string? fallbackValue)
        : base(false, fallbackValue) { }

    /// <inheritdoc/>
    public override bool AllowsNull => true;

    /// <inheritdoc/>
    public override string? ConvertToString(string? value) => value;

    /// <inheritdoc/>
    public override bool TryParseValue(ReadOnlySpan<char> value, out string? result)
    {
        if (value.IsEmpty)
        {
            result = FallbackValue;
            return false;
        }

        result = value.ToString();
        return true;
    }
}

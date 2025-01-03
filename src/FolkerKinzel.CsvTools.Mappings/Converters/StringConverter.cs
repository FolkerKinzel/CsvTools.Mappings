using System.Diagnostics.CodeAnalysis;

namespace FolkerKinzel.CsvTools.Mappings.Converters;

/// <summary>
/// <see cref="TypeConverter{T}"/> implementation for <see cref="string"/>.
/// </summary>
public sealed class StringConverter : TypeConverter<string?>
{
    /// Initializes a new <see cref="StringConverter"/> instance.
    /// <param name="fallbackValue">The value of <see cref="TypeConverter{T}.FallbackValue"/>.</param>
    private StringConverter(string? fallbackValue)
        : base(false, fallbackValue) { }

    /// <summary>
    /// Creates a new <see cref="TypeConverter{T}">TypeConverter&lt;String?&gt;</see> instance.
    /// </summary>
    /// <returns>The newly created <see cref="TypeConverter{T}">TypeConverter&lt;String?&gt;</see>
    /// instance. Its <see cref="ITypeConverter{T}.FallbackValue"/> will be <c>null</c>.</returns>
    public static TypeConverter<string?> CreateNullable() => new StringConverter(null);

    /// <summary>
    /// Creates a new <see cref="TypeConverter{T}">TypeConverter&lt;String&gt;</see> instance.
    /// </summary>
    /// <returns>The newly created <see cref="TypeConverter{T}">TypeConverter&lt;String&gt;</see>
    /// instance. Its <see cref="ITypeConverter{T}.FallbackValue"/> will be <see cref="string.Empty"/>.
    /// </returns>
    public static TypeConverter<string> CreateNonNullable() => new StringConverter("")!;

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

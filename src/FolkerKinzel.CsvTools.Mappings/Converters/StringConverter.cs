using System.Diagnostics.CodeAnalysis;

namespace FolkerKinzel.CsvTools.Mappings.Converters;

/// <summary>
/// <see cref="TypeConverter{T}"/> implementation for <see cref="string"/>.
/// </summary>
/// <param name="nullable"><c>true</c> to set <see cref="TypeConverter{T}.FallbackValue"/>
/// to <c>null</c>; <c>false</c> to have <see cref="string.Empty"/> as 
/// <see cref="TypeConverter{T}.FallbackValue"/>.</param>
public sealed class StringConverter(bool nullable = true) 
    : TypeConverter<string?>(false, nullable ? null : string.Empty)
{
    /// <inheritdoc/>
    public override bool AllowsNull => true;

    /// <inheritdoc/>
    public override string? ConvertToString(string? value) => value;

    /// <inheritdoc/>
    protected override bool CsvHasValue(ReadOnlySpan<char> csvInput) => !csvInput.IsEmpty;

    /// <inheritdoc/>
    public override bool TryParseValue(ReadOnlySpan<char> value, out string? result)
    {
        result = value.ToString();
        return true;
    }
}

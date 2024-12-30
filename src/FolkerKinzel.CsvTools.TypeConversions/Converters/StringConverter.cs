using System.Diagnostics.CodeAnalysis;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

/// <summary>
/// <see cref="CsvTypeConverter{T}"/> implementation for <see cref="string"/>.
/// </summary>
public sealed class StringConverter(bool nullable = true) 
    : CsvTypeConverter<string?>(false, nullable ? null : string.Empty)
{

    /// <inheritdoc/>
    public override bool AcceptsNull => true;

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

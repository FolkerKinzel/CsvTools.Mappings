using System.Diagnostics.CodeAnalysis;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

public sealed class StringConverter : CsvTypeConverter<string?>
{
    public StringConverter(bool nullable = true) : base(false, nullable ? null : string.Empty) { }

    /// <inheritdoc/>
    public override bool AcceptsNull => true;

    /// <inheritdoc/>
    protected override string? DoConvertToString(string? value) => value;

    /// <inheritdoc/>
    protected override bool CsvHasValue(ReadOnlySpan<char> csvInput) => !csvInput.IsEmpty;

    /// <inheritdoc/>
    public override bool TryParseValue(ReadOnlySpan<char> value, out string? result)
    {
        result = value.ToString();
        return true;
    }
}

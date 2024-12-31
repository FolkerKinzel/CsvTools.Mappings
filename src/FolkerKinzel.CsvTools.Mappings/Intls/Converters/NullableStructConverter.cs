using FolkerKinzel.CsvTools.Mappings.Converters;
using System.Diagnostics.CodeAnalysis;

namespace FolkerKinzel.CsvTools.Mappings.Intls.Converters;


internal sealed class NullableStructConverter<T> : CsvTypeConverter<T?> where T : struct
{
    private readonly CsvTypeConverter<T> _typeConverter;

    public override bool AcceptsNull => true;

    internal NullableStructConverter(CsvTypeConverter<T> converter)
        : base((converter ?? throw new ArgumentNullException(nameof(converter))).Throwing) => _typeConverter = converter;

    public override string? ConvertToString(T? value) => value.HasValue ? _typeConverter.ConvertToString(value.Value) : null;

    public override bool TryParseValue(ReadOnlySpan<char> value, [NotNullWhen(true)] out T? result)
    {
        if (_typeConverter.TryParseValue(value, out T tmp))
        {
            result = tmp;
            return true;
        }

        result = FallbackValue;
        return false;
    }
}

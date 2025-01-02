using FolkerKinzel.CsvTools.Mappings.Converters;

namespace FolkerKinzel.CsvTools.Mappings.Intls.Converters;

internal sealed class DBNullConverter<T> : TypeConverter<object>
{
    private readonly TypeConverter<T> _valueConverter;

    internal DBNullConverter(TypeConverter<T> converter)
        : base(converter.Throwing, DBNull.Value) => _valueConverter = converter;

    public override bool AllowsNull => _valueConverter.AllowsNull;

    public override string? ConvertToString(object value)
        => Convert.IsDBNull(value)
            ? null
            : _valueConverter.ConvertToString((T)value);

    public override bool TryParseValue(ReadOnlySpan<char> value, out object? result)
    {
        if(value.IsEmpty)
        {
            result = FallbackValue;
            return true;
        }

        if (_valueConverter.TryParseValue(value, out T? tmp))
        {
            result = tmp ?? FallbackValue;
            return true;
        }

        result = FallbackValue;
        return false;
    }
}

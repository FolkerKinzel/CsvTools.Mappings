using FolkerKinzel.CsvTools.Mappings.Converters;

namespace FolkerKinzel.CsvTools.Mappings.Intls.Converters;

internal sealed class DBNullConverter<T> : TypeConverter<object>
{
    private readonly TypeConverter<T> _valueConverter;

    internal DBNullConverter(TypeConverter<T> converter)
        : base(DBNull.Value, converter.Throwing) => _valueConverter = converter;

    public override bool AcceptsNull => _valueConverter.AcceptsNull;

    public override string? ConvertToString(object value)
        => Convert.IsDBNull(value)
            ? null
            : _valueConverter.ConvertToString((T)value);

    public override bool TryParseValue(ReadOnlySpan<char> value, out object? result)
    {
        if(value.IsEmpty)
        {
            result = DefaultValue;
            return true;
        }

        if (_valueConverter.TryParseValue(value, out T? tmp))
        {
            result = tmp ?? DefaultValue;
            return true;
        }

        result = DefaultValue;
        return false;
    }
}

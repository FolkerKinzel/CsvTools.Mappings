using FolkerKinzel.CsvTools.Mappings.TypeConverters;

namespace FolkerKinzel.CsvTools.Mappings.Intls.Converters;

internal sealed class DBNullConverter<T> : TypeConverter<object>
{
    private readonly TypeConverter<T> _valueConverter;

    internal DBNullConverter(TypeConverter<T> converter)
        : base(converter.Throwing, DBNull.Value) => _valueConverter = converter;

    public override bool AcceptsNull => _valueConverter.AcceptsNull;

    public override string? ConvertToString(object value)
        => Convert.IsDBNull(value)
            ? null
            : _valueConverter.ConvertToString((T)value);

    public override bool TryParse(ReadOnlySpan<char> value, out object? result)
    {
        Debug.Assert(!value.IsEmpty);

        if (_valueConverter.TryParse(value, out T? tmp))
        {
            result = tmp ?? DefaultValue;
            return true;
        }

        result = DefaultValue;
        return false;
    }
}

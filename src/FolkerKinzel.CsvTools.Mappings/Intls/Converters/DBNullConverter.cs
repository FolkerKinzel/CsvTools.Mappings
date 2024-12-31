using FolkerKinzel.CsvTools.Mappings.Converters;

namespace FolkerKinzel.CsvTools.Mappings.Intls.Converters;

internal sealed class DBNullConverter<T> : TypeConverter<object>
{
    private readonly TypeConverter<T> _valueConverter;

    public override bool AcceptsNull => _valueConverter.AcceptsNull;

    internal DBNullConverter(TypeConverter<T> converter)
        : base((converter ?? throw new ArgumentNullException(nameof(converter))).Throwing, DBNull.Value)
        => _valueConverter = converter;

    public override string? ConvertToString(object value)
        => value == DBNull.Value
            ? null
            : _valueConverter.ConvertToString((T)value);

    public override bool TryParseValue(ReadOnlySpan<char> value, out object result)
    {
        if (_valueConverter.TryParseValue(value, out T tmp))
        {
            result = tmp ?? FallbackValue!;
            return true;
        }

        result = FallbackValue!;
        return false;
    }
}

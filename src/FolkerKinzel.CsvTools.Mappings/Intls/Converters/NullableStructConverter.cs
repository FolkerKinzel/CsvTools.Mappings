using FolkerKinzel.CsvTools.Mappings.TypeConverters;
using System.Diagnostics.CodeAnalysis;

namespace FolkerKinzel.CsvTools.Mappings.Intls.Converters;

internal sealed class NullableStructConverter<T> : TypeConverter<T?> where T : struct
{
    private readonly TypeConverter<T> _typeConverter;

    public override bool AcceptsNull => true;

    /// <summary>
    /// Initializes a new <see cref="NullableStructConverter{T}"/> instance.
    /// </summary>
    /// <param name="converter">The <see cref="TypeConverter{T}"/> instance that is used as template.</param>
    /// <returns>The newly created <see cref="TypeConverter{T}"/> instance that converts <see cref="Nullable{T}"/>
    /// instead of <typeparamref name="T"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="converter"/> is <c>null</c>.</exception>
    internal NullableStructConverter(TypeConverter<T> converter)
        : base(default, (converter ?? throw new ArgumentNullException(nameof(converter))).Throwing)
        => _typeConverter = converter;

    //internal TypeConverter<T?> Create(TypeConverter<T> converter) => ((object)converter.DefaultValue) == null ? converter : new NullableStructConverter<T>(converter);

    public override string? ConvertToString(T? value) => value.HasValue ? _typeConverter.ConvertToString(value.Value) : null;

    public override bool TryParseValue(ReadOnlySpan<char> value, out T? result)
    {
        if (value.IsWhiteSpace())
        {
            result = DefaultValue;
            return true;
        }

        if (_typeConverter.TryParseValue(value, out T tmp))
        {
            result = tmp;
            return true;
        }

        result = DefaultValue;
        return false;
    }
}

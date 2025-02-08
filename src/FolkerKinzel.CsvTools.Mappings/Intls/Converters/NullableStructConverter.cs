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
        : base(default, converter.Throwing)
        => _typeConverter = converter;

    public override string? ConvertToString(T? value) => value.HasValue ? _typeConverter.ConvertToString(value.Value) : null;

    public override bool TryParse(ReadOnlySpan<char> value, out T? result)
    {
        Debug.Assert(!value.IsEmpty);

        if (_typeConverter.TryParse(value, out T tmp))
        {
            result = tmp;
            return true;
        }

        result = DefaultValue;
        return false;
    }
}

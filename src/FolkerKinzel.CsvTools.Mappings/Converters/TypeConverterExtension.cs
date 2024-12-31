using FolkerKinzel.CsvTools.Mappings.Intls.Converters;

namespace FolkerKinzel.CsvTools.Mappings.Converters;

/// <summary>
/// Extension methods for <see cref="TypeConverter{T}"/>.
/// </summary>
public static class TypeConverterExtension
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TypeConverter<object> ToDBNullConverter<T>(this TypeConverter<T> converter)
    {
        return converter is TypeConverter<object> result && Convert.IsDBNull(result.FallbackValue)
            ? result
            : new DBNullConverter<T>(converter);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TypeConverter<T?> ToNullableConverter<T>(this TypeConverter<T> converter)
        where T : struct => new NullableStructConverter<T>(converter);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TypeConverter<IEnumerable<TItem?>?> ToIEnumerableConverter<TItem>(this TypeConverter<TItem?> itemsConverter,
                                                                                 bool nullable = true,
                                                                                 char fieldSeparator = ',')
        => new IEnumerableConverter<TItem>(itemsConverter, nullable, fieldSeparator);



    //internal static ICsvTypeConverter HandleNullableAndDBNullAcceptance<T>(this CsvTypeConverter<T> converter, bool nullable, bool dbNullEnabled) where T : struct
    //{
    //    if (nullable)
    //    {
    //        CsvTypeConverter<T?> nullableConv = converter.AsNullableConverter();

    //        return dbNullEnabled ? nullableConv.AsDBNullEnabled() : nullableConv;
    //    }

    //    return dbNullEnabled ? converter.AsDBNullEnabled() : converter;
    //}

}

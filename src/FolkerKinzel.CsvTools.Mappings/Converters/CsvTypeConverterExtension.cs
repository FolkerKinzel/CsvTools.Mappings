using FolkerKinzel.CsvTools.Mappings.Intls.Converters;

namespace FolkerKinzel.CsvTools.Mappings.Converters;

/// <summary>
/// Extension methods for <see cref="CsvTypeConverter{T}"/>.
/// </summary>
public static class CsvTypeConverterExtension
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CsvTypeConverter<object> AsDBNullEnabled<T>(this CsvTypeConverter<T> converter)
    {
        return converter is CsvTypeConverter<object> result && Convert.IsDBNull(result.FallbackValue)
            ? result
            : new DBNullConverter<T>(converter);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CsvTypeConverter<T?> AsNullableConverter<T>(this CsvTypeConverter<T> converter)
        where T : struct => new NullableStructConverter<T>(converter);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CsvTypeConverter<IEnumerable<TItem?>?> AsIEnumerableConverter<TItem>(this CsvTypeConverter<TItem?> itemsConverter,
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

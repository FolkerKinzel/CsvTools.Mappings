using FolkerKinzel.CsvTools.Mappings.Intls;
using FolkerKinzel.CsvTools.Mappings.Intls.Converters;

namespace FolkerKinzel.CsvTools.Mappings.Converters;

/// <summary>
/// Extension methods for <see cref="TypeConverter{T}"/>.
/// </summary>
public static class TypeConverterExtension
{
    /// <summary>
    /// Returns a <see cref="TypeConverter{T}">TypeConverter&lt;<see cref="object"/>&gt;</see>
    /// instance
    /// that converts and accepts <typeparamref name="T"/> as well as <see cref="DBNull.Value"/>.
    /// <see cref="DBNull.Value"/> is the <see cref="TypeConverter{T}.FallbackValue"/> of this
    /// <see cref="TypeConverter{T}"/> instance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="converter"></param>
    /// <returns>
    /// A <see cref="TypeConverter{T}">TypeConverter&lt;<see cref="object"/>&gt;</see>
    /// instance
    /// that converts and accepts <typeparamref name="T"/> as well as <see cref="DBNull.Value"/>.
    /// <see cref="DBNull.Value"/> is the <see cref="TypeConverter{T}.FallbackValue"/> of this new
    /// <see cref="TypeConverter{T}"/> instance.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="converter"/> is <c>null</c>.</exception>
    /// <remarks>
    /// The method initializes not in any case a new instance: If <paramref name="converter"/> already fits
    /// the requirements, it returns <paramref name="converter"/>.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TypeConverter<object> ToDBNullConverter<T>(this TypeConverter<T> converter)
    {
        _ArgumentNullException.ThrowIfNull(converter, nameof(converter));
        return converter is TypeConverter<object> result && Convert.IsDBNull(result.FallbackValue)
            ? result
            : new DBNullConverter<T>(converter);
    }

    /// <summary>
    /// Creates a new <see cref="Nullable{T}"/> converter instance that's based on <paramref name="converter"/>.
    /// </summary>
    /// <typeparam name="T">The data type that <paramref name="converter"/> can convert. 
    /// <typeparamref name="T"/> must be a <see cref="ValueType"/>.</typeparam>
    /// <param name="converter">The <see cref="TypeConverter{T}"/> instance that is used as template.</param>
    /// <returns>The newly created <see cref="TypeConverter{T}"/> instance that converts <see cref="Nullable{T}"/>
    /// instead of <typeparamref name="T"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="converter"/> is <c>null</c>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TypeConverter<Nullable<T>> ToNullableConverter<T>(this TypeConverter<T> converter)
        where T : struct => new NullableStructConverter<T>(converter);

    /// <summary>
    /// Creates a new <see cref="IEnumerable{T}"/> converter instance whose items will be converted by 
    /// <paramref name="itemsConverter"/>.
    /// </summary>
    /// <typeparam name="TItem">The <see cref="Type"/> of the items of the <see cref="IEnumerable{T}"/>
    /// objects that the newly created converter can convert.</typeparam>
    /// <param name="itemsConverter">A <see cref="TypeConverter{T}"/> instance that converts the items.</param>
    /// <param name="separator">A <see cref="string"/> that separates the items in field of the CSV file. When parsing
    /// the CSV, <paramref name="separator"/> will not be part of the results.</param>    
    /// <param name="nullable"><c>true</c> to set <see cref="TypeConverter{T}.FallbackValue"/>
    /// to <c>null</c>; <c>false</c> to have <see cref="Enumerable.Empty{TResult}"/> as 
    /// <see cref="TypeConverter{T}.FallbackValue"/>.
    /// </param>
    /// <returns>
    /// A new <see cref="TypeConverter{T}"/> instance that converts <see cref="IEnumerable{T}"/>. The value of its 
    /// <see cref="TypeConverter{T}.Throwing"/> property is derived from <paramref name="itemsConverter"/>.
    /// </returns>
    /// 
    /// <remarks>
    /// The converter the method creates uses a simple string split and join operation. For nested CSV better use a
    /// simple <see cref="StringConverter"/> and initialize a separate <see cref="CsvEnumerator"/>, <see cref="CsvWriter"/>,
    /// and <see cref="CsvRecordMapping"/> to handle the nested items.
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="itemsConverter"/> or <paramref name="separator"/>
    /// is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="separator"/> is an <see cref="string.Empty"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TypeConverter<IEnumerable<TItem?>?> ToIEnumerableConverter<TItem>(this TypeConverter<TItem?> itemsConverter,
                                                                                    string separator,
                                                                                    bool nullable = true)
        => new IEnumerableConverter<TItem>(itemsConverter, separator, nullable);



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

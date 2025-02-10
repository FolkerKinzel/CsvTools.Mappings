using FolkerKinzel.CsvTools.Mappings.Intls;
using FolkerKinzel.CsvTools.Mappings.Intls.Converters;
using FolkerKinzel.CsvTools.Mappings.Resources;
using FolkerKinzel.CsvTools.Mappings.TypeConverters;
using FolkerKinzel.CsvTools.Mappings.TypeConverters.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;

namespace FolkerKinzel.CsvTools.Mappings;

/// <summary>
/// Extension methods for <see cref="TypeConverter{T}"/>.
/// </summary>
public static class TypeConverterExtension
{
    // IMPORTANT: This class MUST remain in this namespace because the
    // FolkerKinzel.CsvTools.Mappings.TypeConverters namespace cannot always been published
    // because some type names conflict with System.Component.Model

    /// <summary>
    /// Returns a <see cref="TypeConverter{T}">TypeConverter&lt;<see cref="object"/>&gt;</see>
    /// instance
    /// that converts and accepts <typeparamref name="T"/> as well as 
    /// <see cref="DBNull.Value">DBNull.Value</see>.
    /// <see cref="DBNull.Value">DBNull.Value</see> is the 
    /// <see cref="TypeConverter{T}.DefaultValue"/> of this <see cref="TypeConverter{T}"/> instance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="converter"></param>
    /// 
    /// <returns>
    /// A <see cref="TypeConverter{T}">TypeConverter&lt;<see cref="object"/>&gt;</see> instance
    /// that converts and accepts <typeparamref name="T"/> as well as <see cref="DBNull.Value"/>.
    /// <see cref="DBNull.Value"/> is the <see cref="TypeConverter{T}.DefaultValue"/> of this new
    /// <see cref="TypeConverter{T}"/> instance.
    /// </returns>
    /// 
    /// <remarks>
    /// <para>
    /// The <see cref="ITypeConverter{T}.DefaultValue"/> of <paramref name="converter"/> MUST be
    /// <c>null</c>.
    /// </para>
    /// <para>
    /// Use the <c>CreateNullable()</c> methods for reference type converters and the 
    /// <see cref="ToNullableConverter{T}(TypeConverter{T})"/> extension method for value type converters
    /// to create a <see cref="TypeConverter{T}"/> instance whose <see cref="ITypeConverter{T}.DefaultValue"/>
    /// is <c>null</c>.
    /// </para>
    /// </remarks>
    /// 
    /// <example>
    /// <note type="note">In the following code examples - for easier readability - exception handling has been omitted.</note>
    /// <para>
    /// Saving the contents of a <see cref="DataTable"/> as a CSV file and importing data from a CSV file into a 
    /// <see cref="DataTable"/>: </para>
    /// <code language="cs" source="..\Examples\DataTableExample.cs"/>
    /// </example>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="converter"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// The <see cref="ITypeConverter{T}.DefaultValue"/> of <paramref name="converter"/> is not <c>null</c>.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TypeConverter<object> ToDBNullConverter<T>(this TypeConverter<T> converter)
    {
        _ArgumentNullException.ThrowIfNull(converter, nameof(converter));

        return converter.DefaultValue is not null
            ? throw new ArgumentException(Res.DefaultValueNotNull, nameof(converter))
            : (TypeConverter<object>)new DBNullConverter<T>(converter);
    }

    /// <summary>
    /// Creates a new <see cref="Nullable{T}"/> converter instance.
    /// </summary>
    /// <typeparam name="T">The data type that <paramref name="converter"/> can convert. 
    /// <typeparamref name="T"/> must be a <see cref="System.ValueType"/>.</typeparam>
    /// <param name="converter">The <see cref="TypeConverter{T}"/> instance that is used as template.</param>
    /// <returns>The newly created <see cref="TypeConverter{T}"/> instance that converts <see cref="Nullable{T}"/>
    /// instead of <typeparamref name="T"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="converter"/> is <c>null</c>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TypeConverter<T?> ToNullableConverter<T>(this TypeConverter<T> converter)
        where T : struct => new NullableStructConverter<T>(converter);

    /// <summary>
    /// Creates a new <see cref="IEnumerable{T}"/> converter instance.
    /// </summary>
    /// <typeparam name="TItem">The <see cref="Type"/> of the items of the <see cref="IEnumerable{T}"/>
    /// objects that the newly created converter can convert.</typeparam>
    /// <param name="itemsConverter">A <see cref="TypeConverter{T}"/> instance that converts the items.</param>
    /// <param name="separator">A <see cref="string"/> that separates the items in a field of the CSV file. When parsing
    /// the CSV, <paramref name="separator"/> will not be part of the results.</param>    
    /// <param name="nullable"><c>true</c> to set <see cref="TypeConverter{T}.DefaultValue"/>
    /// to <c>null</c>; <c>false</c> to have <see cref="Enumerable.Empty{TResult}"/> as 
    /// <see cref="TypeConverter{T}.DefaultValue"/>.
    /// </param>
    /// <returns>
    /// A new <see cref="TypeConverter{T}"/> instance that converts <see cref="IEnumerable{T}"/>. The value of its 
    /// <see cref="TypeConverter{T}.Throwing"/> property is derived from <paramref name="itemsConverter"/>.
    /// </returns>
    /// 
    /// <remarks>
    /// The converter the method creates uses a simple string split and join operation. For nested CSV better use a
    /// simple <see cref="StringConverter"/> and initialize a separate <see cref="CsvReader"/>, <see cref="CsvWriter"/>,
    /// and <see cref="CsvMapping"/> to handle the nested items.
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


    /// <summary>
    /// Creates a new <see cref="IList{T}"/> converter instance.
    /// </summary>
    /// <typeparam name="TItem">The <see cref="Type"/> of the items of the <see cref="IList{T}"/>
    /// objects that the newly created converter can convert.</typeparam>
    /// <param name="itemsConverter">A <see cref="TypeConverter{T}"/> instance that converts the items.</param>
    /// <param name="separator">A <see cref="string"/> that separates the items in a field of the CSV file. When parsing
    /// the CSV, <paramref name="separator"/> will not be part of the results.</param>    
    /// <param name="nullable"><c>true</c> to set <see cref="TypeConverter{T}.DefaultValue"/>
    /// to <c>null</c>; <c>false</c> to have an empty <see cref="IList{T}"/> as 
    /// <see cref="TypeConverter{T}.DefaultValue"/>.
    /// </param>
    /// <returns>
    /// A new <see cref="TypeConverter{T}"/> instance that converts <see cref="IList{T}"/>. The value of its 
    /// <see cref="TypeConverter{T}.Throwing"/> property is derived from <paramref name="itemsConverter"/>.
    /// </returns>
    /// 
    /// <remarks>
    /// The converter the method creates uses a simple string split and join operation. For nested CSV better use a
    /// simple <see cref="StringConverter"/> and initialize a separate <see cref="CsvReader"/>, <see cref="CsvWriter"/>,
    /// and <see cref="CsvMapping"/> to handle the nested items.
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="itemsConverter"/> or <paramref name="separator"/>
    /// is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="separator"/> is an <see cref="string.Empty"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TypeConverter<IList<TItem?>?> ToIListConverter<TItem>(this TypeConverter<TItem?> itemsConverter,
                                                                        string separator,
                                                                        bool nullable = true)
        => new IListConverter<TItem>(itemsConverter, separator, nullable);

    /// <summary>
    /// Creates a new <see cref="ICollection{T}"/> converter instance.
    /// </summary>
    /// <typeparam name="TItem">The <see cref="Type"/> of the items of the <see cref="ICollection{T}"/>
    /// objects that the newly created converter can convert.</typeparam>
    /// <param name="itemsConverter">A <see cref="TypeConverter{T}"/> instance that converts the items.</param>
    /// <param name="separator">A <see cref="string"/> that separates the items in a field of the CSV file. When parsing
    /// the CSV, <paramref name="separator"/> will not be part of the results.</param>    
    /// <param name="nullable"><c>true</c> to set <see cref="TypeConverter{T}.DefaultValue"/>
    /// to <c>null</c>; <c>false</c> to have an empty <see cref="ICollection{T}"/> as 
    /// <see cref="TypeConverter{T}.DefaultValue"/>.
    /// </param>
    /// <returns>
    /// A new <see cref="TypeConverter{T}"/> instance that converts <see cref="ICollection{T}"/>. The value of its 
    /// <see cref="TypeConverter{T}.Throwing"/> property is derived from <paramref name="itemsConverter"/>.
    /// </returns>
    /// 
    /// <remarks>
    /// The converter the method creates uses a simple string split and join operation. For nested CSV better use a
    /// simple <see cref="StringConverter"/> and initialize a separate <see cref="CsvReader"/>, <see cref="CsvWriter"/>,
    /// and <see cref="CsvMapping"/> to handle the nested items.
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="itemsConverter"/> or <paramref name="separator"/>
    /// is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="separator"/> is an <see cref="string.Empty"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TypeConverter<ICollection<TItem?>?> ToICollectionConverter<TItem>(this TypeConverter<TItem?> itemsConverter,
                                                                                    string separator,
                                                                                    bool nullable = true)
        => new ICollectionConverter<TItem>(itemsConverter, separator, nullable);

    /// <summary>
    /// Creates a new <see cref="IReadOnlyList{T}"/> converter instance.
    /// </summary>
    /// <typeparam name="TItem">The <see cref="Type"/> of the items of the <see cref="IReadOnlyList{T}"/>
    /// objects that the newly created converter can convert.</typeparam>
    /// <param name="itemsConverter">A <see cref="TypeConverter{T}"/> instance that converts the items.</param>
    /// <param name="separator">A <see cref="string"/> that separates the items in a field of the CSV file. When parsing
    /// the CSV, <paramref name="separator"/> will not be part of the results.</param>    
    /// <param name="nullable"><c>true</c> to set <see cref="TypeConverter{T}.DefaultValue"/>
    /// to <c>null</c>; <c>false</c> to have an empty <see cref="IReadOnlyList{T}"/> as 
    /// <see cref="TypeConverter{T}.DefaultValue"/>.
    /// </param>
    /// <returns>
    /// A new <see cref="TypeConverter{T}"/> instance that converts <see cref="IReadOnlyList{T}"/>. The value of its 
    /// <see cref="TypeConverter{T}.Throwing"/> property is derived from <paramref name="itemsConverter"/>.
    /// </returns>
    /// 
    /// <remarks>
    /// The converter the method creates uses a simple string split and join operation. For nested CSV better use a
    /// simple <see cref="StringConverter"/> and initialize a separate <see cref="CsvReader"/>, <see cref="CsvWriter"/>,
    /// and <see cref="CsvMapping"/> to handle the nested items.
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="itemsConverter"/> or <paramref name="separator"/>
    /// is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="separator"/> is an <see cref="string.Empty"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TypeConverter<IReadOnlyList<TItem?>?> ToIReadOnlyListConverter<TItem>(this TypeConverter<TItem?> itemsConverter,
                                                                                        string separator,
                                                                                        bool nullable = true)
        => new IReadOnlyListConverter<TItem>(itemsConverter, separator, nullable);

    /// <summary>
    /// Creates a new <see cref="IReadOnlyCollection{T}"/> converter instance.
    /// </summary>
    /// <typeparam name="TItem">The <see cref="Type"/> of the items of the <see cref="IReadOnlyCollection{T}"/>
    /// objects that the newly created converter can convert.</typeparam>
    /// <param name="itemsConverter">A <see cref="TypeConverter{T}"/> instance that converts the items.</param>
    /// <param name="separator">A <see cref="string"/> that separates the items in a field of the CSV file. When parsing
    /// the CSV, <paramref name="separator"/> will not be part of the results.</param>    
    /// <param name="nullable"><c>true</c> to set <see cref="TypeConverter{T}.DefaultValue"/>
    /// to <c>null</c>; <c>false</c> to have an empty <see cref="IReadOnlyCollection{T}"/> as 
    /// <see cref="TypeConverter{T}.DefaultValue"/>.
    /// </param>
    /// <returns>
    /// A new <see cref="TypeConverter{T}"/> instance that converts <see cref="IReadOnlyCollection{T}"/>. The value of its 
    /// <see cref="TypeConverter{T}.Throwing"/> property is derived from <paramref name="itemsConverter"/>.
    /// </returns>
    /// 
    /// <remarks>
    /// The converter the method creates uses a simple string split and join operation. For nested CSV better use a
    /// simple <see cref="StringConverter"/> and initialize a separate <see cref="CsvReader"/>, <see cref="CsvWriter"/>,
    /// and <see cref="CsvMapping"/> to handle the nested items.
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="itemsConverter"/> or <paramref name="separator"/>
    /// is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="separator"/> is an <see cref="string.Empty"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TypeConverter<IReadOnlyCollection<TItem?>?>
        ToIReadOnlyCollectionConverter<TItem>(this TypeConverter<TItem?> itemsConverter,
                                              string separator,
                                              bool nullable = true)
        => new IReadOnlyCollectionConverter<TItem>(itemsConverter, separator, nullable);

    /// <summary>
    /// Creates a new <see cref="List{T}"/> converter instance.
    /// </summary>
    /// <typeparam name="TItem">The <see cref="Type"/> of the items of the <see cref="List{T}"/>
    /// objects that the newly created converter can convert.</typeparam>
    /// <param name="itemsConverter">A <see cref="TypeConverter{T}"/> instance that converts the items.</param>
    /// <param name="separator">A <see cref="string"/> that separates the items in a field of the CSV file. When parsing
    /// the CSV, <paramref name="separator"/> will not be part of the results.</param>    
    /// <param name="nullable"><c>true</c> to set <see cref="TypeConverter{T}.DefaultValue"/>
    /// to <c>null</c>; <c>false</c> to have an empty <see cref="List{T}"/> as 
    /// <see cref="TypeConverter{T}.DefaultValue"/>.
    /// </param>
    /// <returns>
    /// A new <see cref="TypeConverter{T}"/> instance that converts <see cref="List{T}"/>. The value of its 
    /// <see cref="TypeConverter{T}.Throwing"/> property is derived from <paramref name="itemsConverter"/>.
    /// </returns>
    /// 
    /// <remarks>
    /// The converter the method creates uses a simple string split and join operation. For nested CSV better use a
    /// simple <see cref="StringConverter"/> and initialize a separate <see cref="CsvReader"/>, <see cref="CsvWriter"/>,
    /// and <see cref="CsvMapping"/> to handle the nested items.
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="itemsConverter"/> or <paramref name="separator"/>
    /// is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="separator"/> is an <see cref="string.Empty"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TypeConverter<List<TItem?>?> ToListConverter<TItem>(this TypeConverter<TItem?> itemsConverter,
                                                                      string separator,
                                                                      bool nullable = true)
        => new ListConverter<TItem>(itemsConverter, separator, nullable);

    /// <summary>
    /// Creates a new <see cref="ReadOnlyCollection{T}"/> converter instance.
    /// </summary>
    /// <typeparam name="TItem">The <see cref="Type"/> of the items of the <see cref="ReadOnlyCollection{T}"/>
    /// objects that the newly created converter can convert.</typeparam>
    /// <param name="itemsConverter">A <see cref="TypeConverter{T}"/> instance that converts the items.</param>
    /// <param name="separator">A <see cref="string"/> that separates the items in a field of the CSV file. When parsing
    /// the CSV, <paramref name="separator"/> will not be part of the results.</param>    
    /// <param name="nullable"><c>true</c> to set <see cref="TypeConverter{T}.DefaultValue"/>
    /// to <c>null</c>; <c>false</c> to have an empty <see cref="ReadOnlyCollection{T}"/> as 
    /// <see cref="TypeConverter{T}.DefaultValue"/>.
    /// </param>
    /// <returns>
    /// A new <see cref="TypeConverter{T}"/> instance that converts <see cref="ReadOnlyCollection{T}"/>. The value of its 
    /// <see cref="TypeConverter{T}.Throwing"/> property is derived from <paramref name="itemsConverter"/>.
    /// </returns>
    /// 
    /// <remarks>
    /// The converter the method creates uses a simple string split and join operation. For nested CSV better use a
    /// simple <see cref="StringConverter"/> and initialize a separate <see cref="CsvReader"/>, <see cref="CsvWriter"/>,
    /// and <see cref="CsvMapping"/> to handle the nested items.
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="itemsConverter"/> or <paramref name="separator"/>
    /// is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="separator"/> is an <see cref="string.Empty"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TypeConverter<ReadOnlyCollection<TItem?>?> ToReadOnlyCollectionConverter<TItem>(this TypeConverter<TItem?> itemsConverter,
                                                                                                  string separator,
                                                                                                  bool nullable = true)
        => new ReadOnlyCollectionConverter<TItem>(itemsConverter, separator, nullable);

    /// <summary>
    /// Creates a new array converter instance.
    /// </summary>
    /// <typeparam name="TItem">The <see cref="Type"/> of the items of the arrays
    /// that the newly created converter can convert.</typeparam>
    /// <param name="itemsConverter">A <see cref="TypeConverter{T}"/> instance that converts the items.</param>
    /// <param name="separator">A <see cref="string"/> that separates the items in a field of the CSV file. 
    /// When parsing
    /// the CSV, <paramref name="separator"/> will not be part of the results.</param>    
    /// <param name="nullable"><c>true</c> to set <see cref="TypeConverter{T}.DefaultValue"/>
    /// to <c>null</c>; <c>false</c> to have <see cref="Array.Empty{T}"/> as 
    /// <see cref="TypeConverter{T}.DefaultValue"/>.
    /// </param>
    /// <returns>
    /// A new <see cref="TypeConverter{T}"/> instance that converts arrays of type <typeparamref name="TItem"/>. 
    /// The value of its 
    /// <see cref="TypeConverter{T}.Throwing"/> property is derived from <paramref name="itemsConverter"/>.
    /// </returns>
    /// 
    /// <remarks>
    /// The converter the method creates uses a simple string split and join operation. For nested CSV better use a
    /// simple <see cref="StringConverter"/> and initialize a separate <see cref="CsvReader"/>, <see cref="CsvWriter"/>,
    /// and <see cref="CsvMapping"/> to handle the nested items.
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="itemsConverter"/> or <paramref name="separator"/>
    /// is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="separator"/> is an <see cref="string.Empty"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TypeConverter<TItem?[]?> ToArrayConverter<TItem>(this TypeConverter<TItem?> itemsConverter,
                                                                   string separator,
                                                                   bool nullable = true)
        => new ArrayConverter<TItem>(itemsConverter, separator, nullable);
}

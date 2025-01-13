using FolkerKinzel.CsvTools.Mappings.Converters;
using FolkerKinzel.CsvTools.Mappings.Intls;
using FolkerKinzel.CsvTools.Mappings.Intls.MappingProperties;
using System.Text.RegularExpressions;

namespace FolkerKinzel.CsvTools.Mappings;

/// <summary>
/// Extension methods for <see cref="Mapping"/>.
/// </summary>
public static class MappingExtension
{
    /// <summary>
    /// Removes all <see cref="DynamicProperty"/> instances from the <see cref="Mapping"/>.
    /// </summary>
    /// <param name="mapping">The <see cref="Mapping"/> whose content has to be removed.</param>
    /// <returns><paramref name="mapping"/> after all its <see cref="DynamicProperty"/> instances have 
    /// been removed (to chain calls).</returns>
    public static Mapping Clear(this Mapping mapping)
    {
        _ArgumentNullException.ThrowIfNull(mapping, nameof(mapping));
        mapping.Clear();
        return mapping;
    }

    /// <summary>
    /// Adds a new <see cref="DynamicProperty"/> instance to the <see cref="Mapping"/>, which accesses 
    /// a single column of the CSV file with its index.
    /// </summary>
    /// <typeparam name="T">The .NET data type of the dynamic property.</typeparam>
    /// <param name="mapping">The <see cref="Mapping"/> to add the <see cref="DynamicProperty"/> to.</param>
    /// <param name="propertyName">The identifier of the dynamic .NET property. It must follow the 
    /// rules for C# identifiers. Only ASCII characters are accepted.
    /// </param>
    /// <param name="csvIndex">Zero-based index of the column in the CSV file.
    /// If this index doesn't exist, the <see cref="DynamicProperty"/> is ignored 
    /// when writing. When reading, in this case, <see cref="TypeConverter{T}.FallbackValue"/> is returned.</param>
    /// <param name="converter">The <see cref="TypeConverter{T}"/> that does the type conversion.</param>
    /// 
    /// <returns><paramref name="mapping"/> with the added <see cref="DynamicProperty"/> to chain calls.</returns>
    /// 
    /// <remarks>Use this method if a CSV file has no header, or, or performance reasons, if the CSV column 
    /// index is known.</remarks>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="mapping"/>, 
    /// or <paramref name="propertyName"/>, or <paramref name="converter"/> is <c>null</c>.</exception>
    /// 
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="csvIndex"/>  is less than Zero.</exception>
    /// <exception cref="ArgumentException">
    /// <para>
    /// <paramref name="propertyName"/> does not conform to the rules for C# 
    /// identifiers (only ASCII characters)
    /// </para>
    /// <para>- or -</para>
    /// <para>
    /// a <see cref="DynamicProperty"/> with the same <see cref="DynamicProperty.PropertyName"/> has already been added.
    /// Check this beforehand with <see cref="Mapping.Contains(string)"/>!
    /// </para>
    /// </exception>
    /// <exception cref="RegexMatchTimeoutException">
    /// Validating of <paramref name="propertyName"/> takes longer than <see cref="Mapping.MaxRegexTimeout"/>.
    /// </exception>
    public static Mapping AddProperty<T>(this Mapping mapping,
                                         string propertyName,
                                         int csvIndex,
                                         TypeConverter<T> converter)
    {
        _ArgumentNullException.ThrowIfNull(mapping, nameof(mapping));

        mapping.Add(new IndexProperty<T>(propertyName, csvIndex, converter));
        return mapping;
    }

    /// <summary>
    /// Adds a new <see cref="DynamicProperty"/> instance to the <see cref="Mapping"/>, which accesses a 
    /// single column of a CSV file with header with a collection of column name aliases.
    /// </summary>
    /// <typeparam name="T">The .NET data type of the dynamic property.</typeparam>
    /// <param name="mapping">The <see cref="Mapping"/> to add the <see cref="DynamicProperty"/> to.</param>
    /// <param name="propertyName">The identifier under which the property is addressed. It must follow the 
    /// rules for C# identifiers. Only ASCII characters are accepted.
    /// </param>
    /// <param name="columnNameAliases">
    /// <para>
    /// Column names of the CSV file that the <see cref="DynamicProperty"/> can access. The aliases can use the 
    /// wildcard characters '*' and '?'. 
    /// </para>
    /// <para>
    /// The first alias, which is a match with a column name of the CSV file, is used. If a wildcard alias matches 
    /// several columns in the CSV file, the column with the lowest index is referenced.
    /// </para>
    /// <para>
    /// The collection will be copied.
    /// </para>
    /// </param>
    /// <param name="converter">The <see cref="TypeConverter{T}"/> that does the type conversion.</param>
    /// 
    ///  
    /// <returns><paramref name="mapping"/> with the added <see cref="DynamicProperty"/> to chain calls.</returns>
    /// 
    /// <remarks>
    /// <para>
    /// Use this method if a CSV column name doesn't match the requirements of C# 
    /// identifiers, or if the CSV column name is unknown.
    /// </para>
    /// <para>
    /// In the very special case where '*' is a letter of the column name, replace '*' with '?'.
    /// </para>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="mapping"/>, 
    /// or <paramref name="propertyName"/>, or <paramref name="columnNameAliases"/>, or <paramref name="converter"/>
    /// is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// <para>
    /// <paramref name="propertyName"/> does not conform to the rules for C# 
    /// identifiers (only ASCII characters)
    /// </para>
    /// <para>- or -</para>
    /// <para>
    /// a <see cref="DynamicProperty"/> with the same <see cref="DynamicProperty.PropertyName"/> has already been added.
    /// Check this beforehand with <see cref="Mapping.Contains(string)"/>!
    /// </para>
    /// </exception>
    /// <exception cref="RegexMatchTimeoutException">
    /// Validating of <paramref name="propertyName"/> takes longer than <see cref="Mapping.MaxRegexTimeout"/>.
    /// </exception>
    public static Mapping AddProperty<T>(this Mapping mapping,
                                         string propertyName,
                                         IEnumerable<string?> columnNameAliases,
                                         TypeConverter<T> converter)
    {
        _ArgumentNullException.ThrowIfNull(mapping, nameof(mapping));

        mapping.Add(new ColumnNameProperty<T>(propertyName, columnNameAliases, converter));
        return mapping;
    }

    /// <summary>
    /// Adds a new <see cref="DynamicProperty"/> instance to the <see cref="Mapping"/>, which accesses a single 
    /// column of a CSV file with header with the column name.
    /// 
    /// </summary>
    /// <typeparam name="T">The .NET data type of the dynamic property.</typeparam>
    /// <param name="mapping">The <see cref="Mapping"/> to add the <see cref="DynamicProperty"/> to.</param>
    /// <param name="propertyName">The identifier of the dynamic .NET property and the corresponding column of the 
    /// CSV file. The value of the argument must follow the rules for C# identifiers. Only ASCII characters are accepted.
    /// </param>
    /// <param name="converter">The <see cref="TypeConverter{T}"/> that does the type conversion.</param>
    /// 
    ///  
    /// <returns><paramref name="mapping"/> with the added <see cref="DynamicProperty"/> to chain calls.</returns>
    /// 
    /// <remarks>
    /// When using this method, <paramref name="propertyName"/> and the referenced CSV column name must match, 
    /// and <paramref name="propertyName"/> must meet the requirements for C# identifiers. Use the 
    /// <see cref="CsvRecord.HasCaseSensitiveColumnNames"/> property to determine whether the comparison is 
    /// case-sensitive.
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="mapping"/>, 
    /// or <paramref name="propertyName"/>, or <paramref name="converter"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// <para>
    /// <paramref name="propertyName"/> does not conform to the rules for C# identifiers (only ASCII characters)
    /// </para>
    /// <para>- or -</para>
    /// <para>
    /// a <see cref="DynamicProperty"/> with the same <see cref="DynamicProperty.PropertyName"/> has already been added.
    /// Check this beforehand with <see cref="Mapping.Contains(string)"/>!
    /// </para>
    /// </exception>
    /// <exception cref="RegexMatchTimeoutException">
    /// Validating of <paramref name="propertyName"/> takes longer than <see cref="Mapping.MaxRegexTimeout"/>.
    /// </exception>
    public static Mapping AddProperty<T>(this Mapping mapping,
                                         string propertyName,
                                         TypeConverter<T> converter)
        => AddProperty(mapping, propertyName, [propertyName], converter);

    /// <summary>
    /// Adds a new <see cref="DynamicProperty"/> instance to the <see cref="Mapping"/>, which accesses 
    /// several columns of a CSV file.
    /// </summary>
    /// <typeparam name="T">The .NET data type of the dynamic property.</typeparam>
    /// <param name="mapping">The <see cref="Mapping"/> to add the <see cref="DynamicProperty"/> to.</param>
    /// <param name="propertyName">The identifier of the dynamic .NET property. It must follow the 
    /// rules for C# identifiers. Only ASCII characters are accepted.
    /// </param>
    /// <param name="converter">An instance derived from the abstract <see cref="MultiColumnTypeConverter{T}"/> 
    /// class. You have to write this class yourself because it depends on the CSV file.</param>
    /// 
    /// <returns><paramref name="mapping"/> with the added <see cref="DynamicProperty"/> to chain calls.</returns>
    /// 
    /// <remarks>
    /// Use this method if the dynamic property is based on several columns of the CSV file.
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    /// Using <see cref="MultiColumnTypeConverter{T}"/> to create and parse a CSV file.
    /// </para>
    /// <para>
    /// (For the sake of easier readability exception handling has been omitted.)
    /// </para>
    /// <img src="\images\MultiColumnConverter.png"/>
    /// <code language="cs" source="../Examples/MultiColumnConverterExample.cs"/>
    /// </example>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="mapping"/>, 
    /// or <paramref name="propertyName"/>, or <paramref name="converter"/> is <c>null</c>.</exception>
    /// 
    /// <exception cref="ArgumentException">
    /// <para>
    /// <paramref name="propertyName"/> does not conform to the rules for C# 
    /// identifiers (only ASCII characters)
    /// </para>
    /// <para>- or -</para>
    /// <para>
    /// a <see cref="DynamicProperty"/> with the same <see cref="DynamicProperty.PropertyName"/> has already been added.
    /// Check this beforehand with <see cref="Mapping.Contains(string)"/>!
    /// </para>
    /// </exception>
    /// <exception cref="RegexMatchTimeoutException">
    /// Validating of <paramref name="propertyName"/> takes longer than <see cref="Mapping.MaxRegexTimeout"/>.
    /// </exception>
    public static Mapping AddProperty<T>(this Mapping mapping,
                                         string propertyName,
                                         MultiColumnTypeConverter<T> converter)
    {
        _ArgumentNullException.ThrowIfNull(mapping, nameof(mapping));

        mapping.Add(new MultiColumnProperty<T>(propertyName, converter));
        return mapping;
    }
}

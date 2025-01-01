using FolkerKinzel.CsvTools.Mappings.Converters;
using FolkerKinzel.CsvTools.Mappings.Intls;
using FolkerKinzel.CsvTools.Mappings.Intls.MappingProperties;
using System.Text.RegularExpressions;

namespace FolkerKinzel.CsvTools.Mappings;

/// <summary>
/// Extension methods for <see cref="CsvRecordMapping"/>.
/// </summary>
public static class CsvRecordMappingExtension
{
    /// <summary>
    /// Adds a new <see cref="MappingProperty"/> instance to <paramref name="mapping"/>, which accesses a single
    /// column of the CSV file.
    /// </summary>
    /// <typeparam name="T">The .NET data type of the dynamic property.</typeparam>
    /// <param name="mapping">The <see cref="CsvRecordMapping"/> to add the <see cref="MappingProperty"/> to.</param>
    /// <param name="propertyName">The identifier of the dynamic .NET property. It must follow the 
    /// rules for C# identifiers. Only ASCII characters are accepted.
    /// </param>
    /// <param name="csvIndex">Zero-based index of the column in the CSV file.
    /// If this index doesn't exist, the <see cref="IndexProperty{T}"/> is ignored 
    /// when writing. When reading, in this case, <see cref="TypeConverter{T}.FallbackValue"/> is returned.</param>
    /// <param name="converter">The <see cref="TypeConverter{T}"/> that does the type conversion.</param>
    /// 
    /// <returns><paramref name="mapping"/> with the added <see cref="MappingProperty"/> to chain calls.</returns>
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
    /// a <see cref="MappingProperty"/> with the same <see cref="MappingProperty.PropertyName"/> has already been added.
    /// Check this beforehand with <see cref="CsvRecordMapping.Contains(string)"/>!
    /// </para>
    /// </exception>
    /// <exception cref="RegexMatchTimeoutException">
    /// Validating of <paramref name="propertyName"/> takes longer than <see cref="CsvRecordMapping.MaxRegexTimeout"/>.
    /// </exception>
    public static CsvRecordMapping AddSingleColumnProperty<T>(this CsvRecordMapping mapping,
                                                              string propertyName,
                                                              int csvIndex,
                                                              TypeConverter<T> converter)
    {
        _ArgumentNullException.ThrowIfNull(mapping, nameof(mapping));

        mapping.Add(new IndexProperty<T>(propertyName, csvIndex, converter));
        return mapping;
    }

    /// <summary>
    /// Adds a new <see cref="MappingProperty"/> instance to <paramref name="mapping"/>, which accesses a single
    /// column of a CSV file with header.
    /// </summary>
    /// <typeparam name="T">The .NET data type of the dynamic property.</typeparam>
    /// <param name="mapping">The <see cref="CsvRecordMapping"/> to add the <see cref="MappingProperty"/> to.</param>
    /// <param name="propertyName">The identifier under which the property is addressed. It must follow the 
    /// rules for C# identifiers. Only ASCII characters are accepted.
    /// </param>
    /// <param name="columnNameAliases">
    /// <para>
    /// Column names of the CSV file that the <see cref="MappingProperty"/> can access. The first alias that is a match 
    /// with a column name of the CSV file is used. The alias <see cref="string"/>s may contain the 
    /// wildcard characters * and ?. 
    /// </para>
    /// <para>
    /// If a wildcard alias matches several columns in the CSV file, the column with the lowest index is referenced.
    /// </para>
    /// <para>
    /// The collection will be copied.
    /// </para>
    /// </param>
    /// <param name="converter">The <see cref="TypeConverter{T}"/> that does the type conversion.</param>
    /// 
    ///  
    /// <returns><paramref name="mapping"/> with the added <see cref="MappingProperty"/> to chain calls.</returns>
    /// 
    /// <remarks>
    /// Use this method if a CSV column name doesn't match the requirements of C# 
    /// identifiers, or if the CSV column name is unknown.
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
    /// a <see cref="MappingProperty"/> with the same <see cref="MappingProperty.PropertyName"/> has already been added.
    /// Check this beforehand with <see cref="CsvRecordMapping.Contains(string)"/>!
    /// </para>
    /// </exception>
    /// <exception cref="RegexMatchTimeoutException">
    /// Validating of <paramref name="propertyName"/> takes longer than <see cref="CsvRecordMapping.MaxRegexTimeout"/>.
    /// </exception>
    public static CsvRecordMapping AddSingleColumnProperty<T>(this CsvRecordMapping mapping,
                                                              string propertyName,
                                                              IEnumerable<string?> columnNameAliases,
                                                              TypeConverter<T> converter)
    {
        _ArgumentNullException.ThrowIfNull(mapping, nameof(mapping));

        mapping.Add(new ColumnNameProperty<T>(propertyName, columnNameAliases, converter));
        return mapping;
    }

    /// <summary>
    /// Adds a new <see cref="MappingProperty"/> instance to <paramref name="mapping"/>, which accesses a single
    /// column of a CSV file with header.
    /// </summary>
    /// <typeparam name="T">The .NET data type of the dynamic property.</typeparam>
    /// <param name="mapping">The <see cref="CsvRecordMapping"/> to add the <see cref="MappingProperty"/> to.</param>
    /// <param name="propertyName">The identifier of the dynamic .NET property and the corresponding column of the 
    /// CSV file. The value of the argument must follow the rules for C# identifiers. Only ASCII characters are accepted.
    /// </param>
    /// <param name="converter">The <see cref="TypeConverter{T}"/> that does the type conversion.</param>
    /// 
    ///  
    /// <returns><paramref name="mapping"/> with the added <see cref="MappingProperty"/> to chain calls.</returns>
    /// 
    /// <remarks>
    /// Use this method if the CSV column name is known and if it matches the requirements of C# identifiers.
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
    /// a <see cref="MappingProperty"/> with the same <see cref="MappingProperty.PropertyName"/> has already been added.
    /// Check this beforehand with <see cref="CsvRecordMapping.Contains(string)"/>!
    /// </para>
    /// </exception>
    /// <exception cref="RegexMatchTimeoutException">
    /// Validating of <paramref name="propertyName"/> takes longer than <see cref="CsvRecordMapping.MaxRegexTimeout"/>.
    /// </exception>
    public static CsvRecordMapping AddSingleColumnProperty<T>(this CsvRecordMapping mapping,
                                                              string propertyName,
                                                              TypeConverter<T> converter)
        => AddSingleColumnProperty(mapping, propertyName, [propertyName], converter);

    /// <summary>
    /// Adds a new <see cref="MappingProperty"/> instance to <paramref name="mapping"/>, which accesses a several
    /// columns of the CSV file.
    /// </summary>
    /// <typeparam name="T">The .NET data type of the dynamic property.</typeparam>
    /// <param name="mapping">The <see cref="CsvRecordMapping"/> to add the <see cref="MappingProperty"/> to.</param>
    /// <param name="propertyName">The identifier of the dynamic .NET property. It must follow the 
    /// rules for C# identifiers. Only ASCII characters are accepted.
    /// </param>
    /// <param name="converter">The <see cref="MultiColumnTypeConverter{T}"/> that does the type conversion.</param>
    /// 
    /// <returns><paramref name="mapping"/> with the added <see cref="MappingProperty"/> to chain calls.</returns>
    /// 
    /// <remarks>
    /// Use this method if the dynamic property is based on several columns of the CSV file.
    /// </remarks>
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
    /// a <see cref="MappingProperty"/> with the same <see cref="MappingProperty.PropertyName"/> has already been added.
    /// Check this beforehand with <see cref="CsvRecordMapping.Contains(string)"/>!
    /// </para>
    /// </exception>
    /// <exception cref="RegexMatchTimeoutException">
    /// Validating of <paramref name="propertyName"/> takes longer than <see cref="CsvRecordMapping.MaxRegexTimeout"/>.
    /// </exception>
    public static CsvRecordMapping AddMultiColumnProperty<T>(this CsvRecordMapping mapping,
                                                              string propertyName,
                                                              MultiColumnTypeConverter<T> converter)
    {
        _ArgumentNullException.ThrowIfNull(mapping, nameof(mapping));

        mapping.Add(new MultiColumnProperty<T>(propertyName, converter));
        return mapping;
    }
}

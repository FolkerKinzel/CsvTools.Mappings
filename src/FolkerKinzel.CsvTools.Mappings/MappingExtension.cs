using FolkerKinzel.CsvTools.Mappings.Converters;
using FolkerKinzel.CsvTools.Mappings.Intls;
using FolkerKinzel.CsvTools.Mappings.Intls.Extensions;
using FolkerKinzel.CsvTools.Mappings.Intls.MappingProperties;
using FolkerKinzel.CsvTools.Mappings.Resources;
using System.Data;
using System.Globalization;
using System.Text;
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
    /// when writing. When reading, in this case, <see cref="TypeConverter{T}.DefaultValue"/> is returned.</param>
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
    /// <example>
    /// <note type="note">In the following code examples - for easier readability - exception handling has been omitted.</note>
    /// <para>
    /// Saving the contents of a <see cref="DataTable"/> as a CSV file and importing data from a CSV file into a 
    /// <see cref="DataTable"/>: </para>
    /// <code language="cs" source="..\Examples\DataTableExample.cs"/>
    /// <para>Object serialization with CSV:</para>
    /// <code language="cs" source="..\Examples\ObjectSerializationExample.cs"/>
    /// </example>
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
    /// <example>
    /// <note type="note">In the following code examples - for easier readability - exception handling has been omitted.</note>
    /// <para>
    /// Saving the contents of a <see cref="DataTable"/> as a CSV file and importing data from a CSV file into a 
    /// <see cref="DataTable"/>: </para>
    /// <code language="cs" source="..\Examples\DataTableExample.cs"/>
    /// </example>
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
    /// <img src="images\MultiColumnConverter.png"/>
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

    /// <summary>
    /// Fills <paramref name="mapping"/> with the items of 
    /// a collection. 
    /// </summary>
    /// <param name="mapping">The <see cref="Mapping"/> to fill.</param>
    /// <param name="data">The collection whose content is used to fill 
    /// <paramref name="mapping"/>.</param>
    /// <param name="resetExcess">
    /// If <paramref name="data"/> has fewer items than <paramref name="mapping"/> has
    /// <see cref="DynamicProperty"/> instances and this parameter is <c>true</c>, the surplus 
    /// properties in record will be reset to their <see cref="DynamicProperty.DefaultValue"/>. 
    /// For performance reasons this parameter can be set to <c>false</c> when writing CSV because 
    /// <see cref="CsvWriter.WriteRecord"/> resets all fields in <paramref name="mapping"/>.
    /// </param>
    /// 
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="mapping"/> or <paramref name="data"/>
    /// is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="data"/> has more items than the
    /// number of <see cref="DynamicProperty"/> instances in <paramref name="mapping"/>.</exception>
    /// <exception cref="InvalidOperationException"> The <see cref="Mapping.Record"/> property of
    /// <paramref name="mapping"/> is <c>null</c>. Assign a 
    /// <see cref="CsvRecord"/> instance to <paramref name="mapping"/> before calling 
    /// this method.
    /// </exception>
    /// <exception cref="InvalidCastException">
    /// An item in <paramref name="data"/> does not match the expected data type.
    /// </exception>
    /// <exception cref="FormatException">
    /// One of the <see cref="TypeConverter{T}"/> instances uses an invalid format string.
    /// </exception>
    public static void FillWith(this Mapping mapping,
                                IEnumerable<object?> data,
                                bool resetExcess = true)
    {
        _ArgumentNullException.ThrowIfNull(mapping, nameof(mapping));
        _ArgumentNullException.ThrowIfNull(data, nameof(data));

        int i = 0;

        foreach (object? item in data)
        {
            if (i >= mapping.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(data));
            }

            mapping[i++].Value = item;
        }

        if (resetExcess)
        {
            for (; i < mapping.Count; i++)
            {
                DynamicProperty prop = mapping[i];
                prop.Value = prop.DefaultValue;
            }
        }
    }

    /// <summary>
    /// Fills <paramref name="mapping"/> with the fields of 
    /// a <see cref="DataRow"/>.
    /// </summary>
    /// <param name="mapping">The <see cref="Mapping"/> to fill.</param>
    /// <param name="dataRow">The <see cref="DataRow"/> whose content is used to fill 
    /// <paramref name="mapping"/>.</param>
    /// 
    /// <remarks>
    /// <para>
    /// Each <see cref="DynamicProperty.PropertyName"/> in <paramref name="mapping"/>
    /// MUST have a corresponding <see cref="DataColumn.ColumnName"/> in <paramref name="dataRow"/>.
    /// The corresponding columns have to match in theír data type too.
    /// </para>
    /// <para>
    /// Since <see cref="Mapping"/> uses case-sensitiv property names and the column names in 
    /// <paramref name="dataRow"/> are case-insensitive, effort must be taken that the 
    /// <see cref="DynamicProperty.PropertyName"/>s in <paramref name="mapping"/> are unique, 
    /// even when treated case-insensitive.
    /// </para>
    /// <para>
    /// The <see cref="DynamicProperty"/> instances in <paramref name="mapping"/> don't need to match 
    /// all columns in <paramref name="dataRow"/> (neither in number nor in order).
    /// </para>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="mapping"/> or <paramref name="dataRow"/>
    /// is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// There is a <see cref="DynamicProperty"/> in <paramref name="mapping"/> whose 
    /// <see cref="DynamicProperty.PropertyName"/> finds no corresponding <see cref="DataColumn.ColumnName"/>
    /// in <paramref name="dataRow"/>.
    /// </exception>
    /// <exception cref="InvalidCastException">
    /// A value in <paramref name="dataRow"/> does not match the expected data type in 
    /// <paramref name="mapping"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException"> The <see cref="Mapping.Record"/> property of
    /// <paramref name="mapping"/> is <c>null</c>. Assign a 
    /// <see cref="CsvRecord"/> instance to <paramref name="mapping"/> before calling 
    /// this method.
    /// </exception>
    /// <exception cref="FormatException">
    /// One of the <see cref="TypeConverter{T}"/> instances uses an invalid format string.
    /// </exception>
    public static void FillWith(this Mapping mapping, DataRow dataRow)
    {
        _ArgumentNullException.ThrowIfNull(mapping, nameof(mapping));
        _ArgumentNullException.ThrowIfNull(dataRow, nameof(dataRow));
        FillWithIntl(mapping, dataRow);
    }

    /// <summary>
    /// Fills <paramref name="mapping"/> with the fields of 
    /// a <see cref="DataRow"/>.
    /// </summary>
    /// <param name="mapping">The <see cref="Mapping"/> to fill.</param>
    /// <param name="dataRow">The <see cref="DataRow"/> whose content is used to fill 
    /// <paramref name="mapping"/>.</param>
    /// 
    /// <remarks>
    /// <para>
    /// Each <see cref="DynamicProperty.PropertyName"/> in <paramref name="mapping"/>
    /// MUST have a corresponding <see cref="DataColumn.ColumnName"/> in <paramref name="dataRow"/>.
    /// The corresponding columns have to match in theír data type too.
    /// </para>
    /// <para>
    /// Since <see cref="Mapping"/> uses case-sensitiv property names and the column names in 
    /// <paramref name="dataRow"/> are case-insensitive, effort must be taken that the 
    /// <see cref="DynamicProperty.PropertyName"/>s in <paramref name="mapping"/> are unique, 
    /// even when treated case-insensitive.
    /// </para>
    /// <para>
    /// The <see cref="DynamicProperty"/> instances in <paramref name="mapping"/> don't need to match 
    /// all columns in <paramref name="dataRow"/> (neither in number nor in order).
    /// </para>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentException">
    /// There is a <see cref="DynamicProperty"/> in <paramref name="mapping"/> whose 
    /// <see cref="DynamicProperty.PropertyName"/> finds no corresponding <see cref="DataColumn.ColumnName"/>
    /// in <paramref name="dataRow"/>.
    /// </exception>
    /// <exception cref="InvalidCastException">
    /// A value in <paramref name="dataRow"/> does not match the expected data type in 
    /// <paramref name="mapping"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException"> The <see cref="Mapping.Record"/> property of
    /// <paramref name="mapping"/> is <c>null</c>. Assign a 
    /// <see cref="CsvRecord"/> instance to <paramref name="mapping"/> before calling 
    /// this method.
    /// </exception>
    /// <exception cref="FormatException">
    /// One of the <see cref="TypeConverter{T}"/> instances uses an invalid format string.
    /// </exception>
    internal static void FillWithIntl(this Mapping mapping, DataRow dataRow)
    {
        if (dataRow.RowState == DataRowState.Deleted)
        {
            if(mapping.Record is null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Res.InstanceIsNull, nameof(Mapping.Record)));
            }

            mapping.Record.Clear();
            return;
        }

        int i = 0;

        for (; i < mapping.Count; i++)
        {
            mapping[i].Value = dataRow[mapping[i].PropertyName];
        }
    }
}

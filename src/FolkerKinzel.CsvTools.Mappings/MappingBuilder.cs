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
/// Builder for <see cref="Mapping"/> instances.
/// </summary>
/// <remarks>
/// <para>
/// An instance of the <see cref="DynamicProperty"/> class represents a dynamic property ("late binding") of the <see cref="Mapping"/> object
/// that can be used like a regular .NET property if the <see cref="Mapping"/> instance is assigned to a variable that is declared with the keyword
/// <c>dynamic</c>.
/// </para>
/// <para>
/// The <c>AddProperty</c> methods of <see cref="MappingBuilder"/> allow to create and add <see cref="DynamicProperty"/> instances. 
/// The order, in which the <see cref="DynamicProperty"/> instances are added, determines their index in 
/// the newly created <see cref="Mapping"/> instance. These indexes may differ from the indexes of the columns of the CSV file that these 
/// <see cref="DynamicProperty"/> instances access.
/// </para>
/// </remarks>
public sealed class MappingBuilder
{
    // The builder pattern allows to avoid circular references in the object tree
    // of Mapping.
    // All DynamicProperty instances are unique since they can only be
    // instantiated and assigned with Mapping builder.
    // As long as MultiColumnTypeConverter<T> takes a MappingBuilder as argument rather
    // than a mapping, all Sub-Mappings in the object tree are unique.

    private Mapping? _mapping;

    private MappingBuilder() { }

    /// <summary>
    /// Creates a new <see cref="MappingBuilder"/> instance.
    /// </summary>
    /// <returns>The newly created <see cref="MappingBuilder"/> instance.</returns>
    public static MappingBuilder Create() => new();

    /// <summary>
    /// Builds a new <see cref="Mapping"/> instance from the contents of <see cref="MappingBuilder"/>
    /// and deletes all contents of <see cref="MappingBuilder"/> on return.
    /// </summary>
    /// <returns>The newly created <see cref="Mapping"/> instance.</returns>
    public Mapping Build()
    {
        Mapping? mapping = _mapping;
        _mapping = null;
        return mapping ?? new Mapping();
    }

    /// <summary>
    /// Adds a new <see cref="DynamicProperty"/> instance that accesses 
    /// a single column of the CSV file with its index.
    /// </summary>
    /// 
    /// <typeparam name="T">The .NET data type of the dynamic property.</typeparam>
    /// 
    /// <param name="propertyName">The identifier of the dynamic .NET property. It must follow the 
    /// rules for C# identifiers. Only ASCII characters are accepted.
    /// </param>
    /// <param name="csvIndex">Zero-based index of the column in the CSV file.
    /// If this index doesn't exist, the <see cref="DynamicProperty"/> is ignored 
    /// when writing. When reading, in this case, <see cref="TypeConverter{T}.DefaultValue"/> is returned.</param>
    /// <param name="converter">The <see cref="TypeConverter{T}"/> that does the type conversion.</param>
    /// 
    /// <returns>The <see cref="MappingBuilder"/> to chain calls.</returns>
    /// 
    /// <remarks>Use this method if a CSV file has no header, or, or performance reasons, if the CSV column 
    /// index is known.</remarks>
    /// 
    /// <exception cref="ArgumentNullException">
    /// <paramref name="propertyName"/> or <paramref name="converter"/> is <c>null</c>.</exception>
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
    /// </para>
    /// </exception>
    /// <exception cref="RegexMatchTimeoutException">
    /// Validating of <paramref name="propertyName"/> takes too long.
    /// </exception>
    public MappingBuilder AddProperty<T>(string propertyName,
                                         int csvIndex,
                                         TypeConverter<T> converter)
    {
        _mapping ??= new Mapping();
        _mapping.AddProperty(new IndexProperty<T>(propertyName, csvIndex, converter));
        return this;
    }

    /// <summary>
    /// Adds a new <see cref="DynamicProperty"/> instance that accesses a 
    /// single column of a CSV file with header using a collection of column name aliases.
    /// </summary>
    /// <typeparam name="T">The .NET data type of the dynamic property.</typeparam>
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
    /// <returns>The <see cref="MappingBuilder"/> to chain calls.</returns>
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
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/>, or <paramref name="columnNameAliases"/>, 
    /// or <paramref name="converter"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// <para>
    /// <paramref name="propertyName"/> does not conform to the rules for C# 
    /// identifiers (only ASCII characters)
    /// </para>
    /// <para>- or -</para>
    /// <para>
    /// a <see cref="DynamicProperty"/> with the same <see cref="DynamicProperty.PropertyName"/> has already been added.
    /// </para>
    /// </exception>
    /// <exception cref="RegexMatchTimeoutException">
    /// Validating of <paramref name="propertyName"/> takes too long.
    /// </exception>
    public MappingBuilder AddProperty<T>(string propertyName,
                                         IEnumerable<string?> columnNameAliases,
                                         TypeConverter<T> converter)
    {
        _mapping ??= new Mapping();

        _mapping.AddProperty(new ColumnNameProperty<T>(propertyName, columnNameAliases, converter));
        return this;
    }

    /// <summary>
    /// Adds a new <see cref="DynamicProperty"/> instance that accesses a single 
    /// column of a CSV file with header using the column name.
    /// 
    /// </summary>
    /// <typeparam name="T">The .NET data type of the dynamic property.</typeparam>
    /// <param name="propertyName">The identifier of the dynamic .NET property and the corresponding column of the 
    /// CSV file. The value of the argument must follow the rules for C# identifiers. Only ASCII characters are accepted.
    /// </param>
    /// <param name="converter">The <see cref="TypeConverter{T}"/> that does the type conversion.</param>
    /// 
    ///  
    /// <returns>The <see cref="MappingBuilder"/> to chain calls.</returns>
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
    /// <exception cref="ArgumentNullException">
    /// <paramref name="propertyName"/> or <paramref name="converter"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// <para>
    /// <paramref name="propertyName"/> does not conform to the rules for C# identifiers (only ASCII characters)
    /// </para>
    /// <para>- or -</para>
    /// <para>
    /// a <see cref="DynamicProperty"/> with the same <see cref="DynamicProperty.PropertyName"/> has already been added.
    /// </para>
    /// </exception>
    /// <exception cref="RegexMatchTimeoutException">
    /// Validating of <paramref name="propertyName"/> takes too long.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MappingBuilder AddProperty<T>(string propertyName,
                                         TypeConverter<T> converter) 
        => AddProperty(propertyName, [propertyName], converter);

    /// <summary>
    /// Adds a new <see cref="DynamicProperty"/> instance that accesses 
    /// several columns of a CSV file.
    /// </summary>
    /// <typeparam name="T">The .NET data type of the dynamic property.</typeparam>
    /// <param name="propertyName">The identifier of the dynamic .NET property. It must follow the 
    /// rules for C# identifiers. Only ASCII characters are accepted.
    /// </param>
    /// <param name="converter">An instance derived from the abstract <see cref="MultiColumnTypeConverter{T}"/> 
    /// class. You have to write this class yourself because it depends on the CSV file.</param>
    /// 
    /// <returns>The <see cref="MappingBuilder"/> to chain calls.</returns>
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
    /// <exception cref="ArgumentNullException">
    /// <paramref name="propertyName"/> or <paramref name="converter"/> is <c>null</c>.</exception>
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
    /// Validating of <paramref name="propertyName"/> takes too long.
    /// </exception>
    public MappingBuilder AddProperty<T>(string propertyName,
                                         MultiColumnTypeConverter<T> converter)
    {
        _mapping ??= new Mapping();

        _mapping.AddProperty(new MultiColumnProperty<T>(propertyName, converter));
        return this;
    }
}

using FolkerKinzel.CsvTools.Mappings.TypeConverters.Interfaces;
using FolkerKinzel.CsvTools.Mappings.Intls.Converters;
using System.Data;

namespace FolkerKinzel.CsvTools.Mappings.TypeConverters;

/// <summary>
/// Static class that contains methods to create <see cref="TypeConverter{T}"/> instances for the 
/// <see cref="string"/> class.
/// </summary>
/// <example>
/// <note type="note">In the following code examples - for easier readability - exception handling has been omitted.</note>
/// <para>
/// Saving the contents of a <see cref="DataTable"/> as a CSV file and importing data from a CSV file into a 
/// <see cref="DataTable"/>: </para>
/// <code language="cs" source="..\Examples\DataTableExample.cs"/>
/// <para>Object serialization with CSV:</para>
/// <code language="cs" source="..\Examples\ObjectSerializationExample.cs"/>
/// </example>
public static class StringConverter
{
    /// <summary>
    /// Creates a new <see cref="TypeConverter{T}">TypeConverter&lt;String?&gt;</see> instance.
    /// </summary>
    /// <returns>The newly created <see cref="TypeConverter{T}">TypeConverter&lt;String?&gt;</see>
    /// instance. Its <see cref="ITypeConverter{T}.DefaultValue"/> will be <c>null</c>.</returns>
    /// <example>
    /// <note type="note">In the following code examples - for easier readability - exception handling has been omitted.</note>
    /// <para>
    /// Saving the contents of a <see cref="DataTable"/> as a CSV file and importing data from a CSV file into a 
    /// <see cref="DataTable"/>: </para>
    /// <code language="cs" source="..\Examples\DataTableExample.cs"/>
    /// <para>Object serialization with CSV:</para>
    /// <code language="cs" source="..\Examples\ObjectSerializationExample.cs"/>
    /// </example>
    public static TypeConverter<string?> CreateNullable() => new StringConverterIntl(null);

    /// <summary>
    /// Creates a new <see cref="TypeConverter{T}">TypeConverter&lt;String&gt;</see> instance.
    /// </summary>
    /// <returns>The newly created <see cref="TypeConverter{T}">TypeConverter&lt;String&gt;</see>
    /// instance. Its <see cref="ITypeConverter{T}.DefaultValue"/> will be <see cref="string.Empty"/>.
    /// </returns>
    public static TypeConverter<string> CreateNonNullable() => new StringConverterIntl("")!;
}

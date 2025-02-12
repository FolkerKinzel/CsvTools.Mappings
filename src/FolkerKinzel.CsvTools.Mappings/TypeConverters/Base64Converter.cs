using FolkerKinzel.CsvTools.Mappings.Intls.TypeConverters;
using FolkerKinzel.CsvTools.Mappings.TypeConverters.Interfaces;

namespace FolkerKinzel.CsvTools.Mappings.TypeConverters;

/// <summary>
/// Static class that contains methods to create <see cref="TypeConverter{T}"/> 
/// instances for <see cref="byte"/> arrays that serialize/deserialize the data
/// in Base64 format.
/// </summary>
/// <threadsafety static="true" instance="true"/>
public static class Base64Converter
{
    /// <summary>
    /// Creates a new <see cref="TypeConverter{T}">TypeConverter&lt;byte[]?&gt;</see> 
    /// instance.
    /// </summary>
    /// <param name="throwing">Sets the value of the <see cref="TypeConverter{T}.Throwing"/>
    /// property.</param>
    /// <returns>The newly created <see cref="TypeConverter{T}">TypeConverter&lt;byte[]?&gt;</see>
    /// instance. Its <see cref="ITypeConverter{T}.DefaultValue"/> will be <c>null</c>. </returns>
    public static TypeConverter<byte[]?> CreateNullable(bool throwing = true)
        => new Base64ConverterIntl(throwing, null);

    /// <summary>
    /// Creates a new <see cref="TypeConverter{T}">TypeConverter&lt;byte[]&gt;</see> instance.
    /// </summary>
    /// <param name="throwing">Sets the value of the <see cref="TypeConverter{T}.Throwing"/> 
    /// property.</param>"/>
    /// <returns>The newly created <see cref="TypeConverter{T}">TypeConverter&lt;byte[]?&gt;</see>
    /// instance. Its <see cref="ITypeConverter{T}.DefaultValue"/> will be an empty 
    /// <see cref="byte"/> array.</returns>
    public static TypeConverter<byte[]> CreateNonNullable(bool throwing = true)
        => new Base64ConverterIntl(throwing, [])!;
}

using FolkerKinzel.CsvTools.Mappings.TypeConverters.Interfaces;
using FolkerKinzel.CsvTools.Mappings.Intls.Converters;

namespace FolkerKinzel.CsvTools.Mappings.TypeConverters;

/// <summary>
/// Static class that contains methods to create <see cref="TypeConverter{T}"/> instances for
/// <see cref="byte"/> arrays.
/// </summary>
public static class ByteArrayConverter
{
    /// <summary>
    /// Creates a new <see cref="TypeConverter{T}">TypeConverter&lt;byte[]?&gt;</see> instance.
    /// </summary>
    /// <param name="throwing">Sets the value of the <see cref="TypeConverter{T}.Throwing"/> property.</param>"/>
    /// <returns>The newly created <see cref="TypeConverter{T}">TypeConverter&lt;byte[]?&gt;</see>
    /// instance. Its <see cref="ITypeConverter{T}.DefaultValue"/> will be <c>null</c>. </returns>
    public static TypeConverter<byte[]?> CreateNullable(bool throwing = true)
        => new ByteArrayConverterIntl(throwing, null);

    /// <summary>
    /// Creates a new <see cref="TypeConverter{T}">TypeConverter&lt;byte[]&gt;</see> instance.
    /// </summary>
    /// <param name="throwing">Sets the value of the <see cref="TypeConverter{T}.Throwing"/> property.</param>"/>
    /// <returns>The newly created <see cref="TypeConverter{T}">TypeConverter&lt;byte[]?&gt;</see>
    /// instance. Its <see cref="ITypeConverter{T}.DefaultValue"/> will be an empty <see cref="byte"/>
    /// array.</returns>
    /// <remarks>
    /// <note type="tip">
    /// If you plan to call <see cref="TypeConverterExtension.ToDBNullConverter{T}(TypeConverter{T})"/> 
    /// on the return value, it's recommended to use <see cref="CreateNullable(bool)"/> instead.
    /// </note>
    /// </remarks>
    public static TypeConverter<byte[]> CreateNonNullable(bool throwing = true)
        => new ByteArrayConverterIntl(throwing, [])!;
}

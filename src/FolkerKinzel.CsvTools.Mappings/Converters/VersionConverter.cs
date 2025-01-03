using FolkerKinzel.CsvTools.Mappings.Intls.Converters;

namespace FolkerKinzel.CsvTools.Mappings.Converters;

/// <summary>
/// Static class that contains methods to create <see cref="TypeConverter{T}"/> instances for the 
/// <see cref="Version"/> type.
/// </summary>
public static class VersionConverter
{
    /// <summary>
    /// Creates a new <see cref="TypeConverter{T}">TypeConverter&lt;Version?&gt;</see> instance.
    /// </summary>
    /// <param name="throwing">Sets the value of the <see cref="TypeConverter{T}.Throwing"/> property.</param>"/>
    /// 
    /// <returns>The newly created <see cref="TypeConverter{T}">TypeConverter&lt;Version?&gt;</see>
    /// instance. Its <see cref="ITypeConverter{T}.FallbackValue"/> will be <c>null</c>.</returns>
    public static TypeConverter<Version?> CreateNullable(bool throwing = true)
        => new VersionConverterIntl(throwing, null);

    /// <summary>
    /// Creates a new <see cref="TypeConverter{T}">TypeConverter&lt;Version&gt;</see> instance.
    /// </summary>
    /// <param name="throwing">Sets the value of the <see cref="TypeConverter{T}.Throwing"/> property.
    /// </param>
    /// <returns>The newly created <see cref="TypeConverter{T}">TypeConverter&lt;Version&gt;</see>
    /// instance. Its <see cref="ITypeConverter{T}.FallbackValue"/> will be a <see cref="Version"/> 
    /// instance that is created with <see cref="Version()"/>.</returns>
    /// <remarks>
    /// <note type="tip">
    /// It's recommended to use <see cref="CreateNullable(bool)"/> instead if you plan to call 
    /// <see cref="TypeConverterExtension.ToDBNullConverter{T}(TypeConverter{T})"/> on the return value.
    /// </note>
    /// </remarks>
    public static TypeConverter<Version> CreateNonNullable(bool throwing = true)
        => new VersionConverterIntl(throwing, new Version())!;
}

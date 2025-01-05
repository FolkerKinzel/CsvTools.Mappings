using FolkerKinzel.CsvTools.Mappings.Intls.Converters;

namespace FolkerKinzel.CsvTools.Mappings.Converters;

/// <summary>
/// Static class that contains methods to create <see cref="TypeConverter{T}"/> instances for the 
/// <see cref="string"/> class.
/// </summary>
public static class StringConverter
{
    /// <summary>
    /// Creates a new <see cref="TypeConverter{T}">TypeConverter&lt;String?&gt;</see> instance.
    /// </summary>
    /// <returns>The newly created <see cref="TypeConverter{T}">TypeConverter&lt;String?&gt;</see>
    /// instance. Its <see cref="ITypeConverter{T}.FallbackValue"/> will be <c>null</c>.</returns>
    public static TypeConverter<string?> CreateNullable() => new StringConverterIntl(null);

    /// <summary>
    /// Creates a new <see cref="TypeConverter{T}">TypeConverter&lt;String&gt;</see> instance.
    /// </summary>
    /// <returns>The newly created <see cref="TypeConverter{T}">TypeConverter&lt;String&gt;</see>
    /// instance. Its <see cref="ITypeConverter{T}.FallbackValue"/> will be <see cref="string.Empty"/>.
    /// </returns>
    /// <remarks>
    /// <note type="tip">
    /// If you plan to call <see cref="TypeConverterExtension.ToDBNullConverter{T}(TypeConverter{T})"/> 
    /// on the return value, it's recommended to use <see cref="CreateNullable()"/> instead.
    /// </note>
    /// </remarks>
    public static TypeConverter<string> CreateNonNullable() => new StringConverterIntl("")!;
}

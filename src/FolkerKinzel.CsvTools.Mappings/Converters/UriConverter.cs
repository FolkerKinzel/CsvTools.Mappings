using FolkerKinzel.CsvTools.Mappings.Intls.Converters;

namespace FolkerKinzel.CsvTools.Mappings.Converters;

/// <summary>
/// Static class that contains methods to create <see cref="TypeConverter{T}"/> instances for the 
/// <see cref="Uri"/> class.
/// </summary>
public static class UriConverter
{
    /// <summary>
    /// Creates a new <see cref="TypeConverter{T}">TypeConverter&lt;Uri?&gt;</see> instance.
    /// </summary>
    /// <param name="throwing">Sets the value of the <see cref="TypeConverter{T}.Throwing"/> property.</param>"/>
    /// 
    /// <returns>The newly created <see cref="TypeConverter{T}">TypeConverter&lt;Uri?&gt;</see>
    /// instance. Its <see cref="ITypeConverter{T}.FallbackValue"/> will be <c>null</c>.</returns>
    public static TypeConverter<Uri?> CreateNullable(bool throwing = true)
        => new UriConverterIntl(throwing, null);

    /// <summary>
    /// Creates a new <see cref="TypeConverter{T}">TypeConverter&lt;Uri&gt;</see> instance.
    /// </summary>
    /// <param name="throwing">Sets the value of the <see cref="TypeConverter{T}.Throwing"/> property.
    /// </param>
    /// <returns>The newly created <see cref="TypeConverter{T}">TypeConverter&lt;Uri&gt;</see>
    /// instance. Its <see cref="ITypeConverter{T}.FallbackValue"/> will be a relative <see cref="Uri"/>,
    /// parsed from an empty <see cref="string"/>.</returns>
    /// <remarks>
    /// <note type="tip">
    /// It's recommended to use <see cref="CreateNullable(bool)"/> instead if you plan to call 
    /// <see cref="TypeConverterExtension.ToDBNullConverter{T}(TypeConverter{T})"/> on the return value.
    /// </note>
    /// </remarks>
    public static TypeConverter<Uri> CreateNonNullable(bool throwing = true)
        => new UriConverterIntl(throwing, new Uri("", UriKind.Relative))!;
}

﻿using FolkerKinzel.CsvTools.Mappings.Converters.Interfaces;
using FolkerKinzel.CsvTools.Mappings.Intls;
using FolkerKinzel.CsvTools.Mappings.Intls.Converters;
using FolkerKinzel.CsvTools.Mappings.Resources;

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
    /// <param name="uriKind">The type of the <see cref="Uri"/>.</param>
    /// <param name="throwing">Sets the value of the <see cref="TypeConverter{T}.Throwing"/> property.</param>"/>
    /// 
    /// <returns>The newly created <see cref="TypeConverter{T}">TypeConverter&lt;Uri?&gt;</see>
    /// instance. Its <see cref="ITypeConverter{T}.DefaultValue"/> will be <c>null</c>.</returns>
    public static TypeConverter<Uri?> CreateNullable(UriKind uriKind = UriKind.RelativeOrAbsolute,
                                                     bool throwing = true)
        => new UriConverterIntl(uriKind, throwing, null);

    /// <summary>
    /// Creates a new <see cref="TypeConverter{T}">TypeConverter&lt;Uri&gt;</see> instance.
    /// </summary>
    /// <param name="defaultValue">The value of <see cref="TypeConverter{T}.DefaultValue"/>.</param>
    /// <param name="uriKind">The type of the <see cref="Uri"/>.</param>
    /// <param name="throwing">Sets the value of the <see cref="TypeConverter{T}.Throwing"/> property.
    /// </param>
    /// <returns>The newly created <see cref="TypeConverter{T}">TypeConverter&lt;Uri&gt;</see>
    /// instance. Its <see cref="ITypeConverter{T}.DefaultValue"/> will be a relative <see cref="Uri"/>,
    /// parsed from an empty <see cref="string"/>.</returns>
    /// <remarks>
    /// <note type="tip">
    /// If you plan to call <see cref="TypeConverterExtension.ToDBNullConverter{T}(TypeConverter{T})"/> 
    /// on the return value, it's recommended to use <see cref="CreateNullable(bool)"/> instead .
    /// </note>
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="defaultValue"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="defaultValue"/> does not match the requirements of
    /// <paramref name="uriKind"/>.</exception>
    public static TypeConverter<Uri> CreateNonNullable(Uri defaultValue,
                                                       UriKind uriKind = UriKind.RelativeOrAbsolute,
                                                       bool throwing = true)
    {
        _ArgumentNullException.ThrowIfNull(defaultValue, nameof(defaultValue));

        return (uriKind == UriKind.Absolute && !defaultValue.IsAbsoluteUri)
                || (uriKind == UriKind.Relative && defaultValue.IsAbsoluteUri)
                    ? throw new ArgumentException(Res.IncorrectUriType, nameof(defaultValue))
                    : (TypeConverter<Uri>)new UriConverterIntl(uriKind, throwing, defaultValue)!;
    }
}

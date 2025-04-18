﻿using FolkerKinzel.CsvTools.Mappings.Intls.TypeConverters;
using FolkerKinzel.CsvTools.Mappings.TypeConverters.Interfaces;

namespace FolkerKinzel.CsvTools.Mappings.TypeConverters;

/// <summary>
/// Static class that contains methods to create <see cref="TypeConverter{T}"/> 
/// instances for the <see cref="Version"/> class.
/// </summary>
/// <threadsafety static="true" instance="true"/>
public static class VersionConverter
{
    /// <summary>
    /// Creates a new <see cref="TypeConverter{T}">TypeConverter&lt;Version?&gt;</see>
    /// instance.
    /// </summary>
    /// <param name="throwing">Sets the value of the <see cref="TypeConverter{T}.Throwing"/>
    /// property.</param>"/>
    /// 
    /// <returns>The newly created <see cref="TypeConverter{T}">TypeConverter&lt;Version?&gt;</see>
    /// instance. Its <see cref="ITypeConverter{T}.DefaultValue"/> will be <c>null</c>.</returns>
    public static TypeConverter<Version?> CreateNullable(bool throwing = true)
        => new VersionConverterIntl(throwing, null);

    /// <summary>
    /// Creates a new <see cref="TypeConverter{T}">TypeConverter&lt;Version&gt;</see> instance.
    /// </summary>
    /// <param name="throwing">Sets the value of the <see cref="TypeConverter{T}.Throwing"/> property.
    /// </param>
    /// <returns>The newly created <see cref="TypeConverter{T}">TypeConverter&lt;Version&gt;</see>
    /// instance. Its <see cref="ITypeConverter{T}.DefaultValue"/> will be a <see cref="Version"/> 
    /// instance that is created with <see cref="Version()"/>.</returns>
    public static TypeConverter<Version> CreateNonNullable(bool throwing = true)
        => new VersionConverterIntl(throwing, new Version())!;
}

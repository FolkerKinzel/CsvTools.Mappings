﻿using FolkerKinzel.CsvTools.Mappings.TypeConverters;

namespace FolkerKinzel.CsvTools.Mappings.Intls.TypeConverters;

/// <summary>
/// <see cref="TypeConverter{T}"/> implementation for <see cref="string"/>.
/// </summary>
internal sealed class StringConverterIntl : TypeConverter<string?>
{
    /// Initializes a new <see cref="StringConverter"/> instance.
    /// <param name="defaultValue">The value of 
    /// <see cref="TypeConverter{T}.DefaultValue"/>.</param>
    internal StringConverterIntl(string? defaultValue)
        : base(false, defaultValue) { }

    /// <inheritdoc/>
    public override bool AcceptsNull => true;

    /// <inheritdoc/>
    public override string? ConvertToString(string? value) => value;

    /// <inheritdoc/>
    public override bool TryParse(ReadOnlySpan<char> value, out string? result)
    {
        Debug.Assert(!value.IsEmpty);

        result = value.ToString();
        return true;
    }
}

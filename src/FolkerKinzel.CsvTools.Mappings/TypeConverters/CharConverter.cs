﻿namespace FolkerKinzel.CsvTools.Mappings.TypeConverters;

/// <summary>
/// <see cref="TypeConverter{T}"/> implementation for <see cref="char"/>.
/// </summary>
/// <param name="defaultValue">
/// The <see cref="TypeConverter{T}.DefaultValue"/> to return when a parsing error occurs and
/// the <see cref="TypeConverter{T}.Throwing"/> property is <c>false</c>.
/// </param>
/// <param name="throwing">Sets the value of the 
/// <see cref="TypeConverter{T}.Throwing"/> property.</param>
/// 
/// <example>
/// <note type="note">In the following code examples - for easier readability - exception handling has been omitted.</note>
/// <para>Object serialization with CSV:</para>
/// <code language="cs" source="..\Benchmarks\CalculationWriter_Default.cs"/>
/// </example>
public sealed class CharConverter(char defaultValue = default, bool throwing = true)
    : TypeConverter<char>(defaultValue, throwing)
{
    /// <inheritdoc/>
    public override bool AcceptsNull => false;

    /// <inheritdoc/>
    public override string? ConvertToString(char value) 
        => value.ToString(); // There is an overload that uses IFormatProvider,
                             // but the parameter is not used.

    /// <inheritdoc/>
    public override bool TryParse(ReadOnlySpan<char> value, out char result)
    {
        if (value.Length == 1)
        {
            result = value[0];
            return true;
        }

        result = DefaultValue;
        return false;
    }
}

namespace FolkerKinzel.CsvTools.Mappings.TypeConverters;

/// <summary>
/// <see cref="TypeConverter{T}"/> implementation for <see cref="bool"/>.
/// </summary>
/// <param name="throwing">Sets the value of the 
/// <see cref="TypeConverter{T}.Throwing"/> property.</param>
/// <param name="defaultValue">Sets the value of the <see cref="TypeConverter{T}.DefaultValue"/> property.</param>
public sealed class BooleanConverter(bool throwing = true, bool defaultValue = default)
    : TypeConverter<bool>(throwing, defaultValue)
{
    /// <inheritdoc/>
    public override bool AcceptsNull => false;

    /// <inheritdoc/>
    public override string? ConvertToString(bool value) // There's an overload that uses IFormatProvider
        => value.ToString();                            // but that parameter is reserved and not used.

    /// <inheritdoc/>
    public override bool TryParse(ReadOnlySpan<char> value, out bool result)
    {
#if NET462 || NETSTANDARD2_0
        return bool.TryParse(value.ToString(), out result);
#else
        return bool.TryParse(value, out result);
#endif
    }
}

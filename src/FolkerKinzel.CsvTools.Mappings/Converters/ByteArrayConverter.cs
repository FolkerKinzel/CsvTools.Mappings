using System.Diagnostics.CodeAnalysis;

namespace FolkerKinzel.CsvTools.Mappings.Converters;

/// <summary>
/// <see cref="TypeConverter{T}"/> implementation for <see cref="byte"/> arrays.
/// </summary>
public sealed class ByteArrayConverter : TypeConverter<byte[]?>
{
    /// <summary>Initializes a new <see cref="ByteArrayConverter"/> instance.</summary>
    /// <param name="throwing">Sets the value of the 
    /// <see cref="TypeConverter{T}.Throwing"/> property.</param>
    /// <param name="nullable"><c>true</c> to set <see cref="TypeConverter{T}.FallbackValue"/>
    /// to <c>null</c>; <c>false</c> to have an empty <see cref="byte"/> array as 
    /// <see cref="TypeConverter{T}.FallbackValue"/>.</param>
    private ByteArrayConverter(bool throwing, byte[]? fallbackValue)
        : base(throwing, fallbackValue) { }

    /// <summary>
    /// Creates a new <see cref="TypeConverter{T}">TypeConverter&lt;byte[]?&gt;</see> instance
    /// whose <see cref="ITypeConverter{T}.FallbackValue"/> is <c>null</c>.
    /// </summary>
    /// <param name="throwing">Sets the value of the <see cref="TypeConverter{T}.Throwing"/> property
    /// of the newly created instance.</param>"/>
    /// <returns>The newly created <see cref="TypeConverter{T}">TypeConverter&lt;byte[]?&gt;</see>
    /// instance.</returns>
    public static TypeConverter<byte[]?> CreateNullable(bool throwing = true)
        => new ByteArrayConverter(throwing, null);

    /// <summary>
    /// Creates a new <see cref="TypeConverter{T}">TypeConverter&lt;byte[]&gt;</see> instance
    /// whose <see cref="ITypeConverter{T}.FallbackValue"/> is an empty <see cref="byte"/> array.
    /// </summary>
    /// <param name="throwing">Sets the value of the <see cref="TypeConverter{T}.Throwing"/> property
    /// of the newly created instance.</param>"/>
    /// <returns>The newly created <see cref="TypeConverter{T}">TypeConverter&lt;byte[]?&gt;</see>
    /// instance.</returns>
    public static TypeConverter<byte[]> CreateNonNullable(bool throwing = true)
        => new ByteArrayConverter(throwing, [])!;

    /// <inheritdoc/>
    public override bool AllowsNull => true;

    /// <inheritdoc/>
    public override string? ConvertToString(byte[]? value)
        => value is null 
              ? null 
              : Convert.ToBase64String(value, Base64FormattingOptions.None);

    /// <inheritdoc/>
    public override bool TryParseValue(ReadOnlySpan<char> value, out byte[]? result)
    {
        if (value.IsWhiteSpace())
        {
            result = FallbackValue;
            return true;
        }

        try
        {
            result = Base64.GetBytes(value, Base64ParserOptions.AcceptMissingPadding);
            return true;
        }
        catch (FormatException)
        {
            result = [];
            return false;
        }
    }
}

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
    /// <param name="fallbackValue">The value of <see cref="TypeConverter{T}.FallbackValue"/>.</param>
    private ByteArrayConverter(bool throwing, byte[]? fallbackValue)
        : base(throwing, fallbackValue) { }

    /// <summary>
    /// Creates a new <see cref="TypeConverter{T}">TypeConverter&lt;byte[]?&gt;</see> instance.
    /// </summary>
    /// <param name="throwing">Sets the value of the <see cref="TypeConverter{T}.Throwing"/> property.</param>"/>
    /// <returns>The newly created <see cref="TypeConverter{T}">TypeConverter&lt;byte[]?&gt;</see>
    /// instance. Its <see cref="ITypeConverter{T}.FallbackValue"/> will be <c>null</c>. </returns>
    public static TypeConverter<byte[]?> CreateNullable(bool throwing = true)
        => new ByteArrayConverter(throwing, null);

    /// <summary>
    /// Creates a new <see cref="TypeConverter{T}">TypeConverter&lt;byte[]&gt;</see> instance.
    /// </summary>
    /// <param name="throwing">Sets the value of the <see cref="TypeConverter{T}.Throwing"/> property.</param>"/>
    /// <returns>The newly created <see cref="TypeConverter{T}">TypeConverter&lt;byte[]?&gt;</see>
    /// instance. Its <see cref="ITypeConverter{T}.FallbackValue"/> will be an empty <see cref="byte"/>
    /// array.</returns>
    /// <remarks>
    /// <note type="tip">
    /// It's recommended to use <see cref="CreateNullable(bool)"/> instead if you plan to call 
    /// <see cref="TypeConverterExtension.ToDBNullConverter{T}(TypeConverter{T})"/> on the return value.
    /// </note>
    /// </remarks>
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

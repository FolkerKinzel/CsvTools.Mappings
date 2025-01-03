using System.Diagnostics.CodeAnalysis;

namespace FolkerKinzel.CsvTools.Mappings.Converters;

/// <summary>
/// <see cref="TypeConverter{T}"/> implementation for <see cref="Uri"/>.
/// </summary>
public sealed class UriConverter : TypeConverter<Uri?>
{
    /// <summary>Initializes a new <see cref="UriConverter"/> instance.</summary>
    /// <param name="throwing">Sets the value of the 
    /// <see cref="TypeConverter{T}.Throwing"/> property.</param>
    /// <param name="fallbackValue">The value of <see cref="TypeConverter{T}.FallbackValue"/>.</param>
    private UriConverter(bool throwing, Uri? fallbackValue)
        : base(throwing, fallbackValue) { }

    /// <summary>
    /// Creates a new <see cref="TypeConverter{T}">TypeConverter&lt;Uri?&gt;</see> instance
    /// whose <see cref="ITypeConverter{T}.FallbackValue"/> is <c>null</c>.
    /// </summary>
    /// <param name="throwing">Sets the value of the <see cref="TypeConverter{T}.Throwing"/> property.</param>"/>
    /// 
    /// <returns>The newly created <see cref="TypeConverter{T}">TypeConverter&lt;Uri?&gt;</see>
    /// instance. Its <see cref="ITypeConverter{T}.FallbackValue"/> will be <c>null</c>.</returns>
    public static TypeConverter<Uri?> CreateNullable(bool throwing = true)
        => new UriConverter(throwing, null);

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
    /// Use <see cref="CreateNullable(bool)"/> if you plan to call 
    /// <see cref="TypeConverterExtension.ToDBNullConverter{T}(TypeConverter{T})"/> on the return value!
    /// </note>
    /// </remarks>
    public static TypeConverter<Uri> CreateNonNullable(bool throwing = true)
        => new UriConverter(throwing, new Uri("", UriKind.Relative))!;

    /// <inheritdoc/>
    public override bool AllowsNull => true;

    /// <inheritdoc/>
    public override string? ConvertToString(Uri? value) 
        => value is null 
            ? null 
            : value.IsAbsoluteUri 
                ? value.AbsoluteUri 
                : value.ToString();
              
    /// <inheritdoc/>
    public override bool TryParseValue(ReadOnlySpan<char> value, out Uri? result)
    {
        if (value.IsWhiteSpace())
        {
            result = FallbackValue;
            return true;
        }

        return Uri.TryCreate(value.Trim().ToString(), UriKind.RelativeOrAbsolute, out result);
    }
}

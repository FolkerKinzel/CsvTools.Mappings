using FolkerKinzel.CsvTools.Mappings.Intls;
using FolkerKinzel.CsvTools.Mappings.Intls.TypeConverters;
using FolkerKinzel.CsvTools.Mappings.TypeConverters.Interfaces;
using System.Net.Mail;

namespace FolkerKinzel.CsvTools.Mappings.TypeConverters;

/// <summary>
/// Static class that contains methods to create <see cref="TypeConverter{T}"/>
/// instances for the <see cref="MailAddress"/> class.
/// </summary>
public static class MailAddressConverter
{
    /// <summary>
    /// Creates a new <see cref="TypeConverter{T}"/> instance for the 
    /// <see cref="MailAddress"/> class.
    /// </summary>
    /// <param name="throwing">Sets the value of the <see cref="TypeConverter{T}.Throwing"/>
    /// property.
    /// </param>
    /// 
    /// <returns>The newly created <see cref="TypeConverter{T}"/> instance. Its 
    /// <see cref="ITypeConverter{T}.DefaultValue"/> will be <c>null</c>.</returns>
    public static TypeConverter<MailAddress?> CreateNullable(bool throwing = true)
        => new MailAddressConverterIntl(throwing, null);

    /// <summary>
    /// Creates a new <see cref="TypeConverter{T}"/> instance for the 
    /// <see cref="MailAddress"/> class.
    /// </summary>
    /// <param name="defaultValue">The value of <see cref="TypeConverter{T}.DefaultValue"/>.
    /// The <paramref name="defaultValue"/> must not be <c>null</c>.</param>
    /// <param name="throwing">Sets the value of the <see cref="TypeConverter{T}.Throwing"/>
    /// property.
    /// </param>
    /// <returns>The newly created <see cref="TypeConverter{T}"/> instance.</returns>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="defaultValue"/> is <c>null</c>.
    /// </exception>
    public static TypeConverter<MailAddress> CreateNonNullable(MailAddress defaultValue,
                                                               bool throwing = true)
    {
        _ArgumentNullException.ThrowIfNull(defaultValue, nameof(defaultValue));

        return new MailAddressConverterIntl(throwing, defaultValue)!;
    }
}

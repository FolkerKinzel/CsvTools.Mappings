using FolkerKinzel.CsvTools.Mappings.TypeConverters;
using System.Net.Mail;

namespace FolkerKinzel.CsvTools.Mappings.Intls.Converters;

/// <summary>
/// <see cref="TypeConverter{T}"/> implementation for <see cref="MailAddress"/>.
/// </summary>
internal sealed class MailAddressConverterIntl : TypeConverter<MailAddress?>
{
    /// <summary>Initializes a new <see cref="MailAddressConverterIntl"/> instance.</summary>
    /// <param name="throwing">Sets the value of the 
    /// <see cref="TypeConverter{T}.Throwing"/> property.</param>
    /// <param name="defaultValue">The value of <see cref="TypeConverter{T}.DefaultValue"/>.</param>
    internal MailAddressConverterIntl(bool throwing, MailAddress? defaultValue)
        : base(throwing, defaultValue) { }

    /// <inheritdoc/>
    public override bool AcceptsNull => true;

    /// <inheritdoc/>
    public override string? ConvertToString(MailAddress? value) => value?.ToString();

    /// <inheritdoc/>
    public override bool TryParse(ReadOnlySpan<char> value, out MailAddress? result)
    {
        Debug.Assert(!value.IsEmpty);

#if NET462 || NETSTANDARD2_0 || NETSTANDARD2_1
        try
        {
            result = new MailAddress(value.ToString());
            return true;
        }
        catch
        {
            result = DefaultValue;
            return false;
        }
#else
        return MailAddress.TryCreate(value.ToString(), out result);
#endif
    }
}

using System.Text.RegularExpressions;
using FolkerKinzel.CsvTools.Mappings.Intls;
using FolkerKinzel.CsvTools.Mappings.Resources;

namespace FolkerKinzel.CsvTools.Mappings;

/// <summary>
/// Abstract base class for classes that represent a dynamic property of <see cref="CsvRecordMapping"/>.
/// </summary>
public abstract partial class MappingProperty
{
    /// <summary>
    /// Initializes a new <see cref="MappingProperty"/> instance.
    /// </summary>
    /// <param name="propertyName">
    /// The identifier under which the property is addressed. It must follow the rules for C# identifiers. 
    /// Only ASCII characters are accepted.</param>
    /// 
    /// <exception cref="ArgumentException"><paramref name="propertyName"/> does not conform to the rules 
    /// for C# identifiers (only ASCII characters).</exception>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="RegexMatchTimeoutException">
    /// Validating of <paramref name="propertyName"/> takes longer than <see cref="CsvRecordMapping.MaxRegexTimeout"/>.
    /// </exception>
    protected MappingProperty(string propertyName)
    {
        _ArgumentNullException.ThrowIfNull(propertyName, nameof(propertyName));

#if NET8_0_OR_GREATER
        if (!PropertyNameRegex().IsMatch(propertyName))
#else
        if (!PropertyNameRegex.IsMatch(propertyName))
#endif
        {
            throw new ArgumentException(Res.BadIdentifier, nameof(propertyName));
        }

        this.PropertyName = propertyName;
    }

    /// <summary>
    /// Identifier of the dynamic property.
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    /// Gets or sets the value of the dynamic property.
    /// </summary>
    /// <remarks>
    /// If the containing <see cref="CsvRecordMapping"/> instance is assigned to a <c>dynamic</c> 
    /// variable, the runtime will do all the required casting operations automatically.
    /// </remarks>
    /// <exception cref="InvalidOperationException"><see cref="Record"/> is <c>null</c>. Assign a <see cref="CsvRecord"/> instance
    /// to <see cref="CsvRecordMapping.Record"/> first before accessing this property.</exception>
    /// <exception cref="InvalidCastException">
    /// <para>
    /// When setting the value,
    /// <paramref name="value"/> is <c>null</c> and 
    /// <see cref="ITypeConverter{T}.AllowsNull"/> is <c>false</c>,
    /// </para>
    /// <para>- or -</para>
    /// <para>
    /// <paramref name="value"/> does not match the expected data type.
    /// </para>
    /// </exception>
    /// <exception cref="FormatException">When getting the value, parsing fails and <see cref="TypeConverter{T}.Throwing"/>
    /// is <c>true</c>.</exception>
    public object? Value
    { 
        get => GetValue();
        set => SetValue(value);
    }

    /// <summary>
    /// The <see cref="CsvRecord"/> object used to access the CSV file.
    /// </summary>
    protected internal abstract CsvRecord? Record { get; internal set; }

    /// <summary>
    /// Gets the value of the dynamic property.
    /// </summary>
    /// <returns>The value.</returns>
    /// <exception cref="InvalidOperationException"><see cref="Record"/> is <c>null</c>. Assign a <see cref="CsvRecord"/> instance
    /// to <see cref="CsvRecordMapping.Record"/> first before calling this method.</exception>
    /// <exception cref="FormatException">Parsing fails and <see cref="TypeConverter{T}.Throwing"/> is <c>true</c>.</exception>
    protected internal abstract object? GetValue();

    /// <summary>
    /// Sets the value of the dynamic property.
    /// </summary>
    /// <param name="value">The value to set.</param>
    /// <exception cref="InvalidOperationException"><see cref="Record"/> is <c>null</c>. Assign a <see cref="CsvRecord"/> instance
    /// to <see cref="CsvRecordMapping.Record"/> first before calling this method.</exception>
    /// <exception cref="InvalidCastException">
    /// <para>
    /// <paramref name="value"/> is <c>null</c> and 
    /// <see cref="ITypeConverter{T}.AllowsNull"/> is <c>false</c>,
    /// </para>
    /// <para>- or -</para>
    /// <para>
    /// <paramref name="value"/> does not match the expected data type.
    /// </para>
    /// </exception>
    protected internal abstract void SetValue(object? value);

#if NET8_0_OR_GREATER
    [GeneratedRegex("^[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.CultureInvariant, CsvRecordMapping.MaxRegexTimeout)]
    private static partial Regex PropertyNameRegex();
#else
    private static Regex PropertyNameRegex { get; } 
        = new("^[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.Compiled | RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(CsvRecordMapping.MaxRegexTimeout));
#endif
}

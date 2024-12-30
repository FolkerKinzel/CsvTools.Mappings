using System.Text.RegularExpressions;
using FolkerKinzel.CsvTools.TypeConversions.Converters.Intls;
using FolkerKinzel.CsvTools.TypeConversions.Resources;

namespace FolkerKinzel.CsvTools.TypeConversions;

/// <summary>
/// Abstract base class for classes that represent a dynamic property of <see cref="CsvRecordMapping"/>.
/// </summary>
public abstract partial class CsvPropertyBase
{
    /// <summary>
    /// Initializes a new <see cref="CsvPropertyBase"/> instance.
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
    /// Validating of <paramref name="propertyName"/> takes longer than 100 ms.
    /// </exception>
    protected CsvPropertyBase(string propertyName)
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
    /// The <see cref="CsvRecord"/> object used to access the CSV file.
    /// </summary>
    protected internal abstract CsvRecord? Record { get; internal set; }


    public virtual object? Value
    {
        get => GetValue();
        set => SetValue(value);
    }

    /// <summary>
    /// Extracts data of a specific type from <see cref="CsvRecord"/>.
    /// </summary>
    /// <returns>The extracted data.</returns>
    protected internal abstract object? GetValue();

    /// <summary>
    /// Stores data of a specific type in the CSV file./>.
    /// </summary>
    /// <param name="value">The data object to be stored.</param>
    /// <exception cref="InvalidCastException"><paramref name="value"/> does not match the expected data type.</exception>
    protected internal abstract void SetValue(object? value);

#if NET8_0_OR_GREATER
    [GeneratedRegex("^[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.CultureInvariant, 100)]
    private static partial Regex PropertyNameRegex();
#else
    private static Regex PropertyNameRegex { get; } = new("^[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.Compiled | RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(100));
#endif
}

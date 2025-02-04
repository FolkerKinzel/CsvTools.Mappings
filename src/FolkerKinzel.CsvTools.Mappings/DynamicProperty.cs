using FolkerKinzel.CsvTools.Mappings.Converters;
using FolkerKinzel.CsvTools.Mappings.Converters.Interfaces;
using FolkerKinzel.CsvTools.Mappings.Intls;
using FolkerKinzel.CsvTools.Mappings.Resources;
using System.Text.RegularExpressions;

namespace FolkerKinzel.CsvTools.Mappings;

/// <summary>
/// Abstract base class for classes that represent a dynamic property of <see cref="Mapping"/>.
/// </summary>
public abstract partial class DynamicProperty : ICloneable, IDynamicProperty
{
    /// <summary>
    /// Initializes a new <see cref="DynamicProperty"/> instance.
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
    /// Validating of <paramref name="propertyName"/> takes longer than <see cref="Mapping.MaxRegexTimeout"/>.
    /// </exception>
    protected DynamicProperty(string propertyName)
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
    /// Copy constructor.
    /// </summary>
    /// <param name="other">The <see cref="DynamicProperty"/> instance to clone.</param>
    protected DynamicProperty(DynamicProperty other)
    {
        this.PropertyName = other.PropertyName;
        // Record can not be set here because some requirements
        // in derived classes are not given at that point
    }

    /// <inheritdoc/>
    public string PropertyName { get; }

    /// <summary>
    /// Gets or sets the value of the dynamic property.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If the containing <see cref="Mapping"/> instance is assigned to a <c>dynamic</c> 
    /// variable, the runtime will do all the required casting operations automatically.
    /// </para>
    /// <para>
    /// For high-performance scenarios you can do without the convenience of dynamic properties 
    /// and perform the type cast yourself with 
    /// <see cref="DynamicPropertyExtension.AsITypedProperty{T}(DynamicProperty)"/>. (This allows you
    /// to process value types without boxing and unboxing.)
    /// </para>
    /// </remarks>
    /// <seealso cref="Mapping.Record"/>
    /// <seealso cref="ITypedProperty{T}.Value"/>
    /// <seealso cref="DynamicPropertyExtension.AsITypedProperty{T}(DynamicProperty)"/>
    /// 
    /// <exception cref="InvalidOperationException"><see cref="Record"/> is <c>null</c>. Assign a 
    /// <see cref="CsvRecord"/> instance to the containing <see cref="Mapping"/> before accessing 
    /// this property.
    /// </exception>
    /// <exception cref="InvalidCastException">
    /// <para>
    /// When setting the value,
    /// <paramref name="value"/> is <c>null</c> and 
    /// <see cref="ITypeConverter{T}.AcceptsNull"/> is <c>false</c>,
    /// </para>
    /// <para>- or -</para>
    /// <para>
    /// <paramref name="value"/> does not match the expected data type.
    /// </para>
    /// </exception>
    /// <exception cref="FormatException">
    /// <para>
    /// When getting the value, parsing fails and <see cref="TypeConverter{T}.Throwing"/>
    /// is <c>true</c>.
    /// </para>
    /// <para>- or -</para>
    /// <para>
    /// When setting the value, the converter uses an invalid format string.
    /// </para>
    /// </exception>
    public abstract object? Value { get; set; }

    /// <summary>
    /// Gets the value that the <see cref="DynamicProperty"/> returns if parsing 
    /// fails.
    /// </summary>
    public abstract object? DefaultValue { get; }

    /// <inheritdoc/>
    /// <remarks>
    /// <see cref="DynamicProperty"/> gets this instance from the containing 
    /// <see cref="Mapping"/> instance.
    /// </remarks>
    [DisallowNull]
    public abstract CsvRecord? Record { get; protected internal set; }

    /// <inheritdoc />
    public abstract IEnumerable<int> CsvColumnIndexes { get; }

    /// <inheritdoc />
    public IEnumerable<string> CsvColumnNames
        // If an index in the CSV file is accessed, Record is not null:
        => CsvColumnIndexes.Select(x => Record!.ColumnNames[x]);

    /// <inheritdoc/>
    public abstract object Clone();

#if NET8_0_OR_GREATER
    [GeneratedRegex("^[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.CultureInvariant, 100)]
    private static partial Regex PropertyNameRegex();
#else
    private static Regex PropertyNameRegex { get; } 
        = new("^[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.Compiled | RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(100));
#endif
}

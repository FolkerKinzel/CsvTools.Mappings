using System.Text.RegularExpressions;
using FolkerKinzel.CsvTools.Mappings.Converters;
using FolkerKinzel.CsvTools.Mappings.Converters.Interfaces;
using FolkerKinzel.CsvTools.Mappings.Intls;
using FolkerKinzel.CsvTools.Mappings.Resources;

namespace FolkerKinzel.CsvTools.Mappings;

/// <summary>
/// Abstract base class for classes that represent a dynamic property of <see cref="Mapping"/>.
/// </summary>
public abstract partial class DynamicProperty
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
    /// Identifier of the dynamic property.
    /// </summary>
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
    /// <see cref="DynamicPropertyExtension.AsITypedProperty{T}(DynamicProperty)"/>
    /// 
    /// <exception cref="InvalidOperationException"><see cref="Record"/> is <c>null</c>. Assign a 
    /// <see cref="CsvRecord"/> instance to the containing <see cref="Mapping"/> before accessing 
    /// this property.
    /// </exception>
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
    public object? Value
    { 
        get => GetValue();
        set => SetValue(value);
    }

    /// <summary>
    /// The <see cref="CsvRecord"/> object used to access the CSV file.
    /// </summary>
    /// <remarks>
    /// <see cref="DynamicProperty"/> gets this instance from the containing 
    /// <see cref="Mapping"/> instance.
    /// </remarks>
    protected internal abstract CsvRecord? Record { get; internal set; }

    /// <summary>
    /// Gets the column indexes of the CSV file that the <see cref="DynamicProperty"/>
    /// accesses.
    /// </summary>
    public abstract IEnumerable<int> CsvColumnIndexes { get; }

    /// <summary>
    /// Gets the column names of the columns of the CSV file that the <see cref="DynamicProperty"/>
    /// accesses.
    /// </summary>
    /// <remarks>
    /// If the CSV file has no header row, gets the automatically created
    /// column names.
    /// </remarks>
    public IEnumerable<string> CsvColumnNames 
        // If an index in the CSV file is accessed, Record is not null:
        => CsvColumnIndexes.Select(x => Record!.ColumnNames[x]);

    /// <summary>
    /// Gets the value of the dynamic property.
    /// </summary>
    /// <returns>The value.</returns>
    /// <exception cref="InvalidOperationException"><see cref="Record"/> is <c>null</c>. Assign a <see cref="CsvRecord"/> instance
    /// to the containing <see cref="Mapping"/> before calling this method.</exception>
    /// <exception cref="FormatException">Parsing fails and <see cref="ITypeConverter{T}.Throwing"/> is <c>true</c>.</exception>
    protected internal abstract object? GetValue();

    /// <summary>
    /// Sets the value of the dynamic property.
    /// </summary>
    /// <param name="value">The value to set.</param>
    /// <exception cref="InvalidOperationException"><see cref="Record"/> is <c>null</c>. Assign a <see cref="CsvRecord"/> instance
    /// to the containing <see cref="Mapping"/> before calling this method.</exception>
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
    /// <exception cref="FormatException">The converter uses an invalid format string.</exception>
    protected internal abstract void SetValue(object? value);

#if NET8_0_OR_GREATER
    [GeneratedRegex("^[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.CultureInvariant, Mapping.MaxRegexTimeout)]
    private static partial Regex PropertyNameRegex();
#else
    private static Regex PropertyNameRegex { get; } 
        = new("^[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.Compiled | RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(Mapping.MaxRegexTimeout));
#endif
}

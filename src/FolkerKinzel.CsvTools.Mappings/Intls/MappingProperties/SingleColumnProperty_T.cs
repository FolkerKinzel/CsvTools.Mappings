using FolkerKinzel.CsvTools.Mappings.Converters;
using FolkerKinzel.CsvTools.Mappings.Converters.Interfaces;
using FolkerKinzel.CsvTools.Mappings.Resources;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FolkerKinzel.CsvTools.Mappings.Intls.MappingProperties;

/// <summary>
/// Abstract base class for classes that represent a dynamic property of <see cref="Mapping"/>
/// whose data comes from a single column of the CSV file.
/// </summary>
///  <param name="propertyName">
/// The identifier under which the property is addressed. It must follow the rules for C# identifiers. 
/// Only ASCII characters are accepted.</param>
/// 
/// <param name="converter">An object that performs the type conversion.</param>
/// 
/// <exception cref="ArgumentNullException"><paramref name="propertyName"/> or <paramref name="converter"/> is <c>null</c>.
/// </exception>
/// <exception cref="ArgumentException"><paramref name="propertyName"/> does not conform to the rules 
/// for C# identifiers (only ASCII characters).</exception>
/// <exception cref="RegexMatchTimeoutException">
/// Validating of <paramref name="propertyName"/> takes longer than <see cref="Mapping.MaxRegexTimeout"/>.
/// </exception>
internal abstract class SingleColumnProperty<T>(string propertyName, TypeConverter<T> converter)
    : DynamicProperty(propertyName), ITypedProperty<T>
{
    /// <inheritdoc/>
    public new T Value
    {
        // The Type can't be T?. If it were, this property could never return
        // a non-nullable reference type.
        get => GetTypedValue()!;
        set => SetTypedValue(value);
    }

    /// <summary>
    /// The index of the column in the CSV file that <see cref="DynamicProperty"/> actually accesses, 
    /// or <c>null</c> if <see cref="DynamicProperty"/> does not find a target in the CSV file.
    /// </summary>
    /// <remarks>The property is updated on each read or write access.</remarks>
    public int? CsvIndex { get; protected set; }

    /// <summary>
    /// An object that does the <see cref="Type"/> conversion.
    /// </summary>
    public TypeConverter<T> Converter { get; } = converter ?? throw new ArgumentNullException(nameof(converter));

    /// <inheritdoc/>
    protected internal override CsvRecord? Record { get; internal set; }

    /// <summary>
    /// Updates <see cref="CsvIndex"/>.
    /// </summary>
    /// <remarks>
    /// The method is called on each read or write access to check if <see cref="CsvIndex"/> is still up to date.
    /// </remarks>
    protected abstract void UpdateCsvIndex();

    /// <inheritdoc/>
    protected internal override object? GetValue() => GetTypedValue();

    /// <inheritdoc/>
    protected internal override void SetValue(object? value)
    {
        if (value is null && !Converter.AllowsNull)
        {
            throw new InvalidCastException(string.Format(CultureInfo.CurrentCulture, Res.CannotCastNull, typeof(T).FullName));
        }
        else
        {
            SetTypedValue((T?)value);
        }
    }

    /// <summary>
    /// Gets the value of the dynamic property.
    /// </summary>
    /// <returns>The value.</returns>
    /// <exception cref="InvalidOperationException"><see cref="Record"/> is <c>null</c>. Assign a <see cref="CsvRecord"/> instance
    /// to <see cref="Mapping.Record"/> first before calling this method.</exception>
    /// <exception cref="FormatException">Parsing fails and <see cref="TypeConverter{T}.Throwing"/> is <c>true</c>.</exception>
    private T? GetTypedValue()
    {
        if (Record is null)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Res.InstanceIsNull, nameof(Record)));
        }

        UpdateCsvIndex();

        return CsvIndex.HasValue
            ? Converter.Parse(Record.Values[CsvIndex.Value].Span)
            : Converter.FallbackValue;
    }

    /// <summary>
    /// Sets the value of the dynamic property.
    /// </summary>
    /// <param name="value">The value to set.</param>
    /// <exception cref="InvalidOperationException"><see cref="Record"/> is <c>null</c>. Assign a <see cref="CsvRecord"/> instance
    /// to <see cref="Mapping.Record"/> first before calling this method.</exception>
    /// <exception cref="InvalidCastException"><paramref name="value"/> is <c>null</c> and 
    /// <see cref="ITypeConverter{T}.AllowsNull"/> is <c>false</c>.</exception>
    /// <exception cref="FormatException">The converter uses an invalid format string.</exception>
    private void SetTypedValue(T? value)
    {
        if (Record is null)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Res.InstanceIsNull, nameof(Record)));
        }

        string? val = value is null
                ? Converter.AllowsNull ? null : throw new InvalidCastException(string.Format(CultureInfo.CurrentCulture, Res.CannotCastNull, typeof(T).FullName))
                : Converter.ConvertToString(value);

        UpdateCsvIndex();

        if (CsvIndex.HasValue)
        {
            Record.Values[CsvIndex.Value]
                = val.AsMemory();
        }
    }
}

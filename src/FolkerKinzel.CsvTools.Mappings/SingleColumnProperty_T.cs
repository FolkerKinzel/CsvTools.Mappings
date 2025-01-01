using FolkerKinzel.CsvTools.Mappings.Converters;
using FolkerKinzel.CsvTools.Mappings.Intls;
using System.Text.RegularExpressions;

namespace FolkerKinzel.CsvTools.Mappings;

/// <summary>
/// Abstract base class for classes that represent a dynamic property of <see cref="CsvRecordMapping"/>
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
/// Validating of <paramref name="propertyName"/> takes longer than <see cref="CsvRecordMapping.MaxRegexTimeout"/>.
/// </exception>
public abstract class SingleColumnProperty<T>(string propertyName, TypeConverter<T> converter)
    : MappingProperty(propertyName), ITypedProperty<T>
{
    ///// <inheritdoc/>
    //public Type DataType => Converter.DataType;

    /// <inheritdoc/>
    public new T? Value
    {
        get => GetTypedValue();
        set => SetTypedValue(value);
    }

    /// <summary>
    /// The index of the column in the CSV file that <see cref="MappingProperty"/> actually accesses, 
    /// or <c>null</c> if <see cref="MappingProperty"/> does not find a target in the CSV file.
    /// </summary>
    /// <remarks>The property is updated on each read or write access.</remarks>
    public int? ReferredCsvIndex { get; protected set; }

    /// <summary>
    /// An object that does the <see cref="Type"/> conversion.
    /// </summary>
    public TypeConverter<T> Converter { get; } = converter ?? throw new ArgumentNullException(nameof(converter));

    /// <inheritdoc/>
    protected internal override CsvRecord? Record { get; internal set; }

    /// <summary>
    /// Updates <see cref="ReferredCsvIndex"/>.
    /// </summary>
    /// <remarks>
    /// The method is called on each read or write access to check if <see cref="ReferredCsvIndex"/> is still up to date.
    /// </remarks>
    protected abstract void UpdateReferredCsvIndex();

    /// <inheritdoc/>
    protected internal override object? GetValue() => GetTypedValue();

    /// <inheritdoc/>
    protected internal override void SetValue(object? value)
    {
        if (value is null && !Converter.AcceptsNull)
        {
            throw new InvalidCastException(string.Format("Cannot cast null to {0}.", typeof(T).FullName));
        }
        else
        {
            SetTypedValue((T?)value);
        }
    }

    private T? GetTypedValue()
    {
        Debug.Assert(Record != null);
        UpdateReferredCsvIndex();

        return ReferredCsvIndex.HasValue
            ? Converter.Parse(Record.Values[ReferredCsvIndex.Value].Span)
            : Converter.FallbackValue;
    }

    private void SetTypedValue(T? value)
    {
        Debug.Assert(Record != null);

        string? val = value is null
                ? Converter.AcceptsNull ? null : throw new InvalidCastException(string.Format("Cannot cast null to {0}.", typeof(T).FullName))
                : Converter.ConvertToString(value);

        UpdateReferredCsvIndex();

        if (ReferredCsvIndex.HasValue)
        {
            Record.Values[ReferredCsvIndex.Value]
                = val.AsMemory();
        }
    }
}

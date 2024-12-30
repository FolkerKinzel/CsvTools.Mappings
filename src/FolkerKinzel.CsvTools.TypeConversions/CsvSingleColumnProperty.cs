using FolkerKinzel.CsvTools.TypeConversions.Converters;
using FolkerKinzel.CsvTools.TypeConversions.Converters.Intls;
using System.Text.RegularExpressions;

namespace FolkerKinzel.CsvTools.TypeConversions;

/// <summary>
/// Abstract base class for classes that represent a dynamic property of <see cref="CsvRecordMapping"/>
/// whose data comes from a single column of the CSV file.
/// </summary>
public abstract class CsvSingleColumnProperty<T> : CsvPropertyBase
{
    /// <summary>
    /// Initializes a new <see cref="CsvSingleColumnProperty{T}"/> instance.
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
    /// Validating of <paramref name="propertyName"/> takes longer than 100 ms.
    /// </exception>
    protected CsvSingleColumnProperty(string propertyName, CsvTypeConverter<T> converter) : base(propertyName)
    {
        _ArgumentNullException.ThrowIfNull(converter, nameof(converter));
        this.Converter = converter;
    }

    /// <summary>
    /// An object that does the <see cref="Type"/> conversion.
    /// </summary>
    public CsvTypeConverter<T> Converter { get; }

    /// <inheritdoc/>
    protected internal override CsvRecord? Record { get; internal set; }

    /// <summary>
    /// The index of the column in the CSV file that <see cref="CsvPropertyBase"/> actually accesses, 
    /// or <c>null</c> if <see cref="CsvPropertyBase"/> does not find a target in the CSV file.
    /// </summary>
    /// <remarks>The property is updated on each read or write access.</remarks>
    public int? ReferredCsvIndex { get; protected set; }

    /// <summary>
    /// Updates <see cref="ReferredCsvIndex"/>.
    /// </summary>
    /// <remarks>
    /// The method is called on each read or write access to check if <see cref="ReferredCsvIndex"/> is still up to date.
    /// </remarks>
    protected abstract void UpdateReferredCsvIndex();

    /// <summary>
    /// Allows to get and set the value of the referenced field in <see cref="Record"/>
    /// without having to use a dynamic property.
    /// </summary>
    /// <remarks>
    /// This property supports high performance scenarios: boxing and unboxing of 
    /// value types can be omitted in this way.
    /// </remarks>
    public T? Value
    {
        get => GetTypedValue();
        set => SetTypedValue(value);
    }

    private T? GetTypedValue()
    {
        Debug.Assert(Record != null);
        UpdateReferredCsvIndex();

        try
        {
            return ReferredCsvIndex.HasValue 
                ? Converter.Parse(Record.Values[ReferredCsvIndex.Value].Span) 
                : Converter.FallbackValue;
        }
        catch (Exception e)
        {
            throw new FormatException(e.Message, e);
        }
    }

    private void SetTypedValue(T? value)
    {
        Debug.Assert(Record != null);

        string? val = value is null
                ? Converter.AcceptsNull ? null : throw new InvalidCastException(string.Format("Cannot cast null to {0}.", typeof(T)))
                : Converter.ConvertToString(value);

        UpdateReferredCsvIndex();

        if (ReferredCsvIndex.HasValue)
        {
            Record.Values[ReferredCsvIndex.Value]
                = val.AsMemory();
        }
    }

    /// <inheritdoc/>
    protected internal override object? GetValue() => GetTypedValue();

    /// <inheritdoc/>
    protected internal override void SetValue(object? value) => SetTypedValue((T?)value);
    
}

using FolkerKinzel.CsvTools.Mappings.TypeConverters;
using FolkerKinzel.CsvTools.Mappings.TypeConverters.Interfaces;
using FolkerKinzel.CsvTools.Mappings.Resources;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FolkerKinzel.CsvTools.Mappings.Intls.DynamicProperties;

/// <summary>
/// Abstract base class for classes that represent a dynamic property of <see cref="CsvRecordMapping"/>
/// whose data comes from a single column of the CSV file.
/// </summary>
internal abstract class SingleColumnProperty<T> : DynamicProperty, ITypedProperty<T>, ICloneable
{
    private readonly TypeConverter<T> _converter;

    /// <summary>Initializes a new <see cref="SingleColumnProperty{T}"/> instance.</summary>
    /// <param name="propertyName">
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
    /// Validating of <paramref name="propertyName"/> takes too long.
    /// </exception>
    public SingleColumnProperty(string propertyName, TypeConverter<T> converter) : base(propertyName)
    {
        _converter = converter ?? throw new ArgumentNullException(nameof(converter));
    }

    /// <inheritdoc/>
    T ITypedProperty<T>.Value
    {
        // The Type can't be T?. If it were, this property could never return
        // a non-nullable reference type.
        get => GetTypedValue()!;
        set => SetTypedValue(value);
    }

    /// <inheritdoc/>
    public override object? Value { get => GetTypedValue(); set => SetValue(value); }

    /// <inheritdoc/>
    T? ITypedProperty<T>.DefaultValue => _converter.DefaultValue;

    /// <inheritdoc/>
    public override object? DefaultValue { get => _converter.DefaultValue; }

    /// <summary>
    /// Returns the index of the column in the CSV file that <see cref="SingleColumnProperty{T}"/> actually accesses, 
    /// or <c>null</c> if <see cref="SingleColumnProperty{T}"/> does not find a target in the CSV file.
    /// </summary>
    protected internal abstract int? GetCsvIndex();

    /// <inheritdoc/>
    public override IEnumerable<int> CsvColumnIndexes
    {
        get
        {
            int? csvIndex = GetCsvIndex();
            return csvIndex.HasValue ? Enumerable.Repeat(csvIndex.Value, 1) : [];
        }
    }

    /// <inheritdoc/>
    public ITypeConverter<T> Converter => _converter;

    /// <inheritdoc/>
    [DisallowNull]
    public override CsvRecord? Record { get; protected internal set; }

    /// <summary>
    /// Sets the value of the dynamic property.
    /// </summary>
    /// <param name="value">The value to set.</param>
    /// <exception cref="InvalidOperationException"><see cref="Record"/> is <c>null</c>. Assign a <see cref="CsvRecord"/> instance
    /// to the containing <see cref="CsvRecordMapping"/> before calling this method.</exception>
    /// <exception cref="InvalidCastException">
    /// <para>
    /// <paramref name="value"/> is <c>null</c> and 
    /// <see cref="ITypeConverter{T}.AcceptsNull"/> is <c>false</c>,
    /// </para>
    /// <para>- or -</para>
    /// <para>
    /// <paramref name="value"/> does not match the expected data type.
    /// </para>
    /// </exception>
    /// <exception cref="FormatException">The converter uses an invalid format string.</exception>
    private void SetValue(object? value)
    {
        if (value is null && !_converter.AcceptsNull)
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
    /// to <see cref="CsvRecordMapping.Record"/> first before calling this method.</exception>
    /// <exception cref="FormatException">Parsing fails and <see cref="TypeConverter{T}.Throwing"/> is <c>true</c>.</exception>
    private T? GetTypedValue()
    {
        if (Record is null)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Res.InstanceIsNull, nameof(Record)));
        }

        int? csvIndex = GetCsvIndex();

        return csvIndex.HasValue
            ? _converter.Parse(Record.Values[csvIndex.Value].Span)
            : _converter.DefaultValue;
    }

    /// <summary>
    /// Sets the value of the dynamic property.
    /// </summary>
    /// <param name="value">The value to set.</param>
    /// <exception cref="InvalidOperationException"><see cref="Record"/> is <c>null</c>. Assign a <see cref="CsvRecord"/> instance
    /// to <see cref="CsvRecordMapping.Record"/> first before calling this method.</exception>
    /// <exception cref="InvalidCastException"><paramref name="value"/> is <c>null</c> and 
    /// <see cref="ITypeConverter{T}.AcceptsNull"/> is <c>false</c>.</exception>
    /// <exception cref="FormatException">The converter uses an invalid format string.</exception>
    private void SetTypedValue(T? value)
    {
        if (Record is null)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Res.InstanceIsNull, nameof(Record)));
        }

        int? csvIndex = GetCsvIndex();

        if (csvIndex.HasValue)
        {
            string? val = value is null ? null : _converter.ConvertToString(value);
            Record.Values[csvIndex.Value] = val.AsMemory();
        }
    }
}

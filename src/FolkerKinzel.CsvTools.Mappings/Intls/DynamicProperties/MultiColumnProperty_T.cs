﻿using FolkerKinzel.CsvTools.Mappings.TypeConverters;
using FolkerKinzel.CsvTools.Mappings.TypeConverters.Interfaces;
using FolkerKinzel.CsvTools.Mappings.Resources;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FolkerKinzel.CsvTools.Mappings.Intls.DynamicProperties;

/// <summary>
/// Abstract base class for classes representing a dynamic property of <see cref="CsvMapping"/> 
/// whose data comes from multiple columns of the CSV file.
/// </summary>
/// <typeparam name="T">The .NET data type of the dynamic property.</typeparam>
internal sealed class MultiColumnProperty<T> : DynamicProperty, ITypedProperty<T>, ICloneable
{
    private readonly MultiColumnTypeConverter<T> _converter;

    /// <summary> Initializes a new <see cref="MultiColumnProperty{T}"/> instance.</summary>
    /// <param name="propertyName">The identifier under which the property is addressed. It must follow the rules for C# identifiers. 
    /// Only ASCII characters are accepted.</param>
    /// <param name="converter">An object derived from <see cref="MultiColumnTypeConverter{T}"/> that performs the type conversion.</param>
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> or <paramref name="converter"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException"><paramref name="propertyName"/> does not conform to the rules 
    /// for C# identifiers (only ASCII characters).</exception>
    /// <exception cref="RegexMatchTimeoutException">
    /// Validating of <paramref name="propertyName"/> takes too long.
    /// </exception>
    internal MultiColumnProperty(string propertyName, MultiColumnTypeConverter<T> converter) 
        : base(propertyName)
    {
        _converter = converter ?? throw new ArgumentNullException(nameof(converter));
    }

    private MultiColumnProperty(MultiColumnProperty<T> other) : base(other)
    {
        _converter = (MultiColumnTypeConverter<T>) other._converter.Clone();
        // Don't change the order: Converter needs to be initialized first!
        Record = other.Record!;
    }

    /// <inheritdoc/>
    public override object Clone() => new MultiColumnProperty<T>(this);

    /// <inheritdoc/>
    T ITypedProperty<T>.Value
    {
        get => GetTypedValue()!;
        set => SetTypedValue(value);
    }

    /// <inheritdoc/>
    public override object? Value
    {
        get => GetTypedValue();
        set => SetValue(value);
    }

    /// <inheritdoc/>
    T? ITypedProperty<T>.DefaultValue => _converter.DefaultValue;

    public override object? DefaultValue => _converter.DefaultValue;

    /// <inheritdoc/>
    public ITypeConverter<T> Converter => _converter;

    /// <inheritdoc/>
    [DisallowNull]
    public override CsvRecord? Record
    {
        get => _converter.Mapping.Record;
        protected internal set => _converter.Mapping.Record = value;
    }

    /// <inheritdoc/>
    public override IEnumerable<int> CsvColumnIndexes
        // break circular references:
        => _converter.Mapping.Select(x => object.ReferenceEquals(this, x) ? [] : x.CsvColumnIndexes)
                             .SelectMany(x => x)
                             .Distinct();

    private T? GetTypedValue()
    {
        return Record is null
            ? throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Res.InstanceIsNull, nameof(Record)))
            : _converter.Parse();
    }

    /// <inheritdoc/>
    private void SetValue(object? value)
    {
        if (Record is null)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Res.InstanceIsNull, nameof(Record)));
        }

        _converter.ConvertToCsv(value);
    }

    private void SetTypedValue(T? value)
    {
        if (Record is null)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Res.InstanceIsNull, nameof(Record)));
        }

        _converter.ConvertToCsv(value);
    }
}

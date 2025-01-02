using FolkerKinzel.CsvTools.Mappings.Converters;
using FolkerKinzel.CsvTools.Mappings.Resources;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FolkerKinzel.CsvTools.Mappings.Intls.MappingProperties;

/// <summary>
/// Abstract base class for classes representing a dynamic property of <see cref="CsvRecordMapping"/> 
/// whose data comes from multiple columns of the CSV file.
/// </summary>
/// <typeparam name="T">The .NET data type of the dynamic property.</typeparam>
/// <param name="propertyName">The identifier under which the property is addressed. It must follow the rules for C# identifiers. 
/// Only ASCII characters are accepted.</param>
/// <param name="converter">An object derived from <see cref="MultiColumnTypeConverter{T}"/> that performs the type conversion.</param>
/// 
/// <exception cref="ArgumentNullException"><paramref name="propertyName"/> or <paramref name="converter"/> is <c>null</c>.
/// </exception>
/// <exception cref="ArgumentException"><paramref name="propertyName"/> does not conform to the rules 
/// for C# identifiers (only ASCII characters).</exception>
/// <exception cref="RegexMatchTimeoutException">
/// Validating of <paramref name="propertyName"/> takes longer than <see cref="CsvRecordMapping.MaxRegexTimeout"/>.
/// </exception>
internal sealed class MultiColumnProperty<T>(string propertyName, MultiColumnTypeConverter<T> converter)
    : MappingProperty(propertyName), ITypedProperty<T>
{
    /// <inheritdoc/>
    public new T Value
    {
        get => GetTypedValue()!;
        set => SetTypedValue(value);
    }

    /// <summary>
    /// An object derived from <see cref="MultiColumnTypeConverter{T}"/> that performs the type conversion.
    /// </summary>
    public MultiColumnTypeConverter<T> Converter { get; } = converter ?? throw new ArgumentNullException(nameof(converter));

    /// <inheritdoc/>
    protected internal override CsvRecord? Record
    {
        get => Converter.Mapping.Record;
        internal set => Converter.Mapping.Record = value;
    }

    /// <inheritdoc/>
    protected internal override object? GetValue() => GetTypedValue();

    private T? GetTypedValue()
    {
        return Record is null
            ? throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Res.InstanceIsNull, nameof(Record)))
            : Converter.Convert();
    }

    /// <inheritdoc/>
    protected internal override void SetValue(object? value)
    {
        if (Record is null)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Res.InstanceIsNull, nameof(Record)));
        }

        Converter.ConvertToCsv(value);
    }

    private void SetTypedValue(T? value)
    {
        if (Record is null)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Res.InstanceIsNull, nameof(Record)));
        }

        Converter.ConvertToCsv(value);
    }
}

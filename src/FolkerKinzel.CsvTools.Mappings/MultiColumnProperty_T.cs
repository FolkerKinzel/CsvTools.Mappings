using FolkerKinzel.CsvTools.Mappings.Converters;
using System.Text.RegularExpressions;

namespace FolkerKinzel.CsvTools.Mappings;

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
public class MultiColumnProperty<T>(string propertyName, MultiColumnTypeConverter<T> converter)
    : MappingProperty(propertyName), ITypedProperty<T>
{
    /// <inheritdoc/>
    public new T? Value
    {
        get => Converter.Convert();
        set => Converter.ConvertToCsv(value);
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
    protected internal override object? GetValue() => Converter.Convert();

    /// <inheritdoc/>
    protected internal override void SetValue(object? value) => Converter.ConvertToCsv(value);
}

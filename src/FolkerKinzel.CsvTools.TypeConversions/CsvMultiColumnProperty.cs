﻿using FolkerKinzel.CsvTools.TypeConversions.Converters.Intls;
using System.Text.RegularExpressions;

namespace FolkerKinzel.CsvTools.TypeConversions;

/// <summary>
/// Abstract base class for classes representing a dynamic property of <see cref="CsvRecordMapping"/> 
/// whose data comes from multiple columns of the CSV file.
/// </summary>
public class CsvMultiColumnProperty : CsvPropertyBase
{
    /// <summary>
    /// Initializes a new <see cref="CsvMultiColumnProperty"/> instance.
    /// </summary>
    /// <param name="propertyName">The identifier under which the property is addressed. It must follow the rules for C# identifiers. 
    /// Only ASCII characters are accepted.</param>
    /// <param name="converter">An object derived from <see cref="CsvMultiColumnTypeConverter"/> that performs the type conversion.</param>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> or <paramref name="converter"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException"><paramref name="propertyName"/> does not conform to the rules 
    /// for C# identifiers (only ASCII characters).</exception>
    /// <exception cref="RegexMatchTimeoutException">
    /// Validating of <paramref name="propertyName"/> takes longer than 100 ms.
    /// </exception>
    public CsvMultiColumnProperty(string propertyName, CsvMultiColumnTypeConverter converter) : base(propertyName)
    {
        _ArgumentNullException.ThrowIfNull(converter, nameof(converter));
        this.Converter = converter;
    }

    /// <summary>
    /// An object derived from <see cref="CsvMultiColumnTypeConverter"/> that performs the type conversion.
    /// </summary>
    public CsvMultiColumnTypeConverter Converter { get; }

    /// <inheritdoc/>
    protected internal override CsvRecord? Record { get => Converter.Mapping.Record; internal set => Converter.Mapping.Record = value; }

    /// <inheritdoc/>
    protected internal override object? GetValue() => Converter.Create();

    /// <inheritdoc/>
    protected internal override void SetValue(object? value) => Converter.ToCsv(value);
}

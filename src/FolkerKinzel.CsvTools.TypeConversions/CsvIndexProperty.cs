﻿using FolkerKinzel.CsvTools.TypeConversions.Converters;
using FolkerKinzel.CsvTools.TypeConversions.Converters.Intls;

namespace FolkerKinzel.CsvTools.TypeConversions;

/// <summary>
/// Represents a dynamic property of <see cref="CsvRecordMapping"/> ("late binding") for processing CSV files without a header.
/// </summary>
/// <remarks>
/// <see cref="CsvIndexProperty{T}"/> 
/// encapsulates information about access and type conversion, which <see cref="CsvRecordMapping"/> needs to access the data of the underlying
/// <see cref="CsvRecord"/> object with its zero-based column index.
/// </remarks>
public sealed class CsvIndexProperty<T> : CsvSingleColumnProperty<T>
{
    /// <summary>
    /// Initializes a new <see cref="CsvIndexProperty{T}"/> instance.
    /// </summary>
    /// <param name="propertyName">The identifier under which the property is addressed. It must follow the rules for C# identifiers. 
    /// Only ASCII characters are accepted.
    /// </param>
    /// <param name="csvIndex">Zero-based index of the column in the CSV file.
    /// If this index doesn't exist, the <see cref="CsvIndexProperty{T}"/> is ignored 
    /// when writing. When reading, in this case, <see cref="ICsvTypeConverter.FallbackValue"/> is returned.</param>
    /// <param name="converter">The <see cref="ICsvTypeConverter"/> that does the type conversion.</param>
    /// 
    /// <exception cref="ArgumentException"><paramref name="propertyName"/> does not conform to the rules for C# identifiers (only ASCII characters).</exception>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> or 
    /// <paramref name="converter"/> is <c>null</c>.</exception>
    /// 
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="csvIndex"/>  is less than Zero.</exception>
    public CsvIndexProperty(
        string propertyName, int csvIndex, CsvTypeConverter<T> converter) : base(propertyName, converter)
    {
        _ArgumentOutOfRangeException.ThrowIfNegative(csvIndex, nameof(csvIndex));
        this.CsvIndex = csvIndex;
    }

    /// <summary>
    /// The zero-based index of the column in the CSV file that <see cref="CsvIndexProperty{T}"/> would like to access.
    /// </summary>
    public int CsvIndex { get; }

    /// <inheritdoc/>
    protected override void UpdateReferredCsvIndex()
    {
        Debug.Assert(Record is not null);
        ReferredCsvIndex = CsvIndex < Record.Count ? CsvIndex : null;
    }
}

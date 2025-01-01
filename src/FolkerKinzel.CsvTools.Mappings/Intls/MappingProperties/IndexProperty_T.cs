using FolkerKinzel.CsvTools.Mappings.Converters;
using System.Text.RegularExpressions;

namespace FolkerKinzel.CsvTools.Mappings.Intls.MappingProperties;

/// <summary>
/// Represents a dynamic property of <see cref="CsvRecordMapping"/> ("late binding") for processing CSV files without a header.
/// </summary>
/// <typeparam name="T">The .NET data type of the dynamic property.</typeparam>
/// <remarks>
/// <see cref="IndexProperty{T}"/> 
/// encapsulates information about access and type conversion, which <see cref="CsvRecordMapping"/> needs to access the data of the underlying
/// <see cref="CsvRecord"/> object with its zero-based column index.
/// </remarks>
internal sealed class IndexProperty<T> : SingleColumnProperty<T>
{
    /// <summary>
    /// Initializes a new <see cref="IndexProperty{T}"/> instance.
    /// </summary>
    /// <param name="propertyName">The identifier under which the property is addressed. It must follow the 
    /// rules for C# identifiers. Only ASCII characters are accepted.
    /// </param>
    /// <param name="csvIndex">Zero-based index of the column in the CSV file.
    /// If this index doesn't exist, the <see cref="IndexProperty{T}"/> is ignored 
    /// when writing. When reading, in this case, <see cref="TypeConverter{T}.FallbackValue"/> is returned.</param>
    /// <param name="converter">The <see cref="TypeConverter{T}"/> that does the type conversion.</param>
    /// 
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> or 
    /// <paramref name="converter"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="csvIndex"/>  is less than Zero.</exception>
    /// <exception cref="ArgumentException"><paramref name="propertyName"/> does not conform to the rules for C# 
    /// identifiers (only ASCII characters).</exception>
    /// <exception cref="RegexMatchTimeoutException">
    /// Validating of <paramref name="propertyName"/> takes longer than <see cref="CsvRecordMapping.RegexTimeout"/>.
    /// </exception>
    public IndexProperty(
        string propertyName, int csvIndex, TypeConverter<T> converter) : base(propertyName, converter)
    {
        _ArgumentOutOfRangeException.ThrowIfNegative(csvIndex, nameof(csvIndex));
        CsvIndex = csvIndex;
    }

    /// <summary>
    /// The zero-based index of the column in the CSV file that <see cref="IndexProperty{T}"/> would like to access.
    /// </summary>
    public int CsvIndex { get; }

    /// <inheritdoc/>
    protected override void UpdateReferredCsvIndex()
    {
        Debug.Assert(Record is not null);
        ReferredCsvIndex = CsvIndex < Record.Count ? CsvIndex : null;
    }
}

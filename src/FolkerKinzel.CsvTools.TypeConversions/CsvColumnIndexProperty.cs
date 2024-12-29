namespace FolkerKinzel.CsvTools.TypeConversions;

/// <summary>
/// Specialization of <see cref="CsvPropertyBase"/> for processing CSV files without a header.
/// </summary>
/// <remarks>
/// Represents a property that <see cref="CsvRecordMapping"/> implements dynamically at runtime ("late binding"). <see cref="CsvColumnIndexProperty"/> 
/// encapsulates information about access and type conversion, which <see cref="CsvRecordMapping"/> needs to access the data of the underlying
/// <see cref="CsvRecord"/> object with its zero-based column index.
/// </remarks>
public sealed class CsvColumnIndexProperty : CsvSingleColumnProperty
{
    /// <summary>
    /// Initializes a new <see cref="CsvColumnIndexProperty"/> instance.
    /// </summary>
    /// <param name="propertyName">The identifier under which the property is addressed. It must follow the rules for C# identifiers. 
    /// Only ASCII characters are accepted.
    /// </param>
    /// <param name="desiredCsvColumnIndex">Zero-based index of the column in the CSV file.
    /// If this index doesn't exist, the <see cref="CsvColumnIndexProperty"/> is ignored 
    /// when writing. When reading, in this case, <see cref="ICsvTypeConverter.FallbackValue"/> is returned.</param>
    /// <param name="converter">The <see cref="ICsvTypeConverter"/> that does the type conversion.</param>
    /// 
    /// <exception cref="ArgumentException"><paramref name="propertyName"/> does not conform to the rules for C# identifiers (only ASCII characters).</exception>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> or 
    /// <paramref name="converter"/> is <c>null</c>.</exception>
    /// 
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="desiredCsvColumnIndex"/>  is less than Zero.</exception>
    public CsvColumnIndexProperty(
        string propertyName, int desiredCsvColumnIndex, ICsvTypeConverter converter) : base(propertyName, converter)
    {
        if (desiredCsvColumnIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(desiredCsvColumnIndex));
        }

        this.DesiredCsvColumnIndex = desiredCsvColumnIndex;
    }

    /// <summary>
    /// The zero-based index of the column in the CSV file that <see cref="CsvColumnIndexProperty"/> would like to access.
    /// </summary>
    public int DesiredCsvColumnIndex { get; }


    /// <inheritdoc/>
    protected override void UpdateReferredCsvColumnIndex()
    {
        Debug.Assert(Record is not null);
        ReferredCsvColumnIndex = DesiredCsvColumnIndex < Record.Count ? DesiredCsvColumnIndex : null;
    }
}

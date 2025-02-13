namespace FolkerKinzel.CsvTools.Mappings;

/// <summary>
/// Interface of <see cref="DynamicProperty"/> and derived classes.
/// </summary>
public interface IDynamicProperty
{
    /// <summary>
    /// Gets the column indexes of the CSV file that the <see cref="DynamicProperty"/>
    /// accesses.
    /// </summary>
    IEnumerable<int> CsvColumnIndexes { get; }

    /// <summary>
    /// Gets the column names of the columns of the CSV file that the 
    /// <see cref="DynamicProperty"/> accesses.
    /// </summary>
    /// <remarks>
    /// If the CSV file has no header row, gets the automatically created
    /// column names.
    /// </remarks>
    IEnumerable<string> CsvColumnNames { get; }

    /// <summary>
    /// Identifier of the dynamic property.
    /// </summary>
    string PropertyName { get; }

    /// <summary>
    /// The <see cref="CsvRecord"/> object used to access the CSV file.
    /// </summary>
    CsvRecord? Record { get; }
}
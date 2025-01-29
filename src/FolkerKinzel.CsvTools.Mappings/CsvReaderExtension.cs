using FolkerKinzel.CsvTools.Mappings.Intls;

namespace FolkerKinzel.CsvTools.Mappings;


/// <summary>
/// Extension methods for the <see cref="CsvReader"/> class.
/// </summary>
public static class CsvReaderExtension
{
    /// <summary>
    /// Returns an <see cref="IEnumerable{T}"/> of <see cref="Mapping"/> objects that allows to
    /// iterate over the rows of the CSV file.
    /// </summary>
    /// 
    /// <param name="reader">The <see cref="CsvReader"/> to use to read the CSV data.</param>
    /// <param name="mapping">The <see cref="Mapping"/> to use to get the data from the fields of 
    /// the CSV file.</param>
    /// <param name="disableCaching">Setting this parameter to <c>true</c> helps to reduce memory
    /// consumption. In this case, the same <see cref="Mapping"/> instance is returned with each iteration
    /// - filled with the current data. Note, however, that caching the results, e.g., with
    /// <see cref="Enumerable.ToArray{TSource}(IEnumerable{TSource})"/>, will no longer work in this case.</param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of <see cref="Mapping"/> objects.
    /// </returns>
    public static IEnumerable<Mapping> Read(this CsvReader reader, Mapping mapping, bool disableCaching = false)
    {
        foreach (CsvRecord record in reader)
        {
            Mapping clone = disableCaching ? mapping : (Mapping)mapping.Clone();
            clone.Record = record;
            yield return clone;
        }
    }
}
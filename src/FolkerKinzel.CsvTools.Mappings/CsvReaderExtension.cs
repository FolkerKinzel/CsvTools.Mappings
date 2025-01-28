using FolkerKinzel.CsvTools.Mappings.Intls;

namespace FolkerKinzel.CsvTools.Mappings;


/// <summary>
/// Extension methods for the <see cref="CsvReader"/> class.
/// </summary>
internal static class CsvReaderExtension
{
    public static IEnumerable<Mapping> Read(this CsvReader reader, Mapping mapping)
    {
        _ArgumentNullException.ThrowIfNull(mapping, nameof(mapping));
        _ArgumentNullException.ThrowIfNull(reader, nameof(reader));

        return DoReadMapping(mapping, reader);

        static IEnumerable<Mapping> DoReadMapping(Mapping mapping, CsvReader reader)
        {
            foreach (CsvRecord record in reader)
            {
                mapping.Record = record;
                yield return mapping;
            }
        }
    }
}
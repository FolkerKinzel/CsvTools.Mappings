using FolkerKinzel.Helpers.Polyfills;

namespace FolkerKinzel.CsvTools.Mappings;

public abstract class CsvTo<TResult>
{
    protected CsvTo(CsvMapping mapping)
    {
        _ArgumentNullException.ThrowIfNull(mapping, nameof(mapping));

        Mapping = mapping;
    }

    public CsvMapping Mapping { get; }

    public abstract TResult Convert(dynamic mapping);
}

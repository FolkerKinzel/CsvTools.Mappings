using FolkerKinzel.Helpers.Polyfills;

namespace FolkerKinzel.CsvTools.Mappings;

public abstract class ToCsv<TData>
{
    protected ToCsv(CsvMapping mapping)
    {
        _ArgumentNullException.ThrowIfNull(mapping, nameof(mapping));

        Mapping = mapping;
    }

    public CsvMapping Mapping { get; }

    public abstract void FillMapping(TData data);
}

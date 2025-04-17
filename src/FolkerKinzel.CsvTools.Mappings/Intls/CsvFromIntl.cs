using FolkerKinzel.Helpers.Polyfills;

namespace FolkerKinzel.CsvTools.Mappings.Intls;

internal class CsvFromIntl<TSource>(CsvMapping mapping, Action<TSource, dynamic> conversion) 
    : CsvFrom<TSource>(mapping)
{
    private readonly Action<TSource, dynamic> _conversion 
        = conversion ?? throw new ArgumentNullException(nameof(conversion));

    public override void FillMapping(TSource data) => _conversion(data, Mapping);
}

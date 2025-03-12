using FolkerKinzel.Helpers.Polyfills;

namespace FolkerKinzel.CsvTools.Mappings.Intls;

internal class ToCsvIntl<TData>(CsvMapping mapping, Action<TData, dynamic> conversion) 
    : ToCsv<TData>(mapping)
{
    private readonly Action<TData, dynamic> _conversion 
        = conversion ?? throw new ArgumentNullException(nameof(conversion));

    public override void FillMapping(TData data) => _conversion(data, Mapping);
}

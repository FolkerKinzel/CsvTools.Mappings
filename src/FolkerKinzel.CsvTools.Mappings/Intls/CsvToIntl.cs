using FolkerKinzel.Helpers.Polyfills;

namespace FolkerKinzel.CsvTools.Mappings.Intls;

internal class CsvToIntl<TResult>(CsvMapping mapping, Func<dynamic, TResult> conversion) 
    : CsvTo<TResult>(mapping)
{
    private readonly Func<dynamic, TResult> _conversion 
        = conversion ?? throw new ArgumentNullException(nameof(conversion));

    public override TResult Convert(dynamic mapping) => _conversion(mapping);
}

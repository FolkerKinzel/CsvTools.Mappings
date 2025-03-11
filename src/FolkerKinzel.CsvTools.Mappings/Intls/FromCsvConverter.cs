namespace FolkerKinzel.CsvTools.Mappings.Intls;

internal class FromCsvConverter<TResult>(Func<dynamic, TResult> conversion) : IFromCsvConverter<TResult>
{
    private readonly Func<dynamic, TResult> _conversion = conversion ?? throw new ArgumentNullException(nameof(conversion));

    public TResult Convert(dynamic mapping) => _conversion(mapping);
}

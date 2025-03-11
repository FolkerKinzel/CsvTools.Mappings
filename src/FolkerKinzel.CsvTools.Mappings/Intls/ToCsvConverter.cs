namespace FolkerKinzel.CsvTools.Mappings.Intls;

internal class ToCsvConverter<TData>(Action<TData, dynamic> conversion) : IToCsvConverter<TData>
{
    private readonly Action<TData, dynamic> _conversion = conversion ?? throw new ArgumentNullException(nameof(conversion));

    public void Convert(TData data, dynamic mapping) => _conversion(data, mapping);
}

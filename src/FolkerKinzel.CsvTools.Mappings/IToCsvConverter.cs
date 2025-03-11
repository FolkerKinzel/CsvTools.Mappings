namespace FolkerKinzel.CsvTools.Mappings;

public interface IToCsvConverter<TData>
{
    void Convert(TData data, dynamic mapping);
}
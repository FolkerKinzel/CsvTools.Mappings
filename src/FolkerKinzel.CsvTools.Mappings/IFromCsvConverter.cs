namespace FolkerKinzel.CsvTools.Mappings;

public interface IFromCsvConverter<TResult>
{
    TResult Convert(dynamic mapping);
}
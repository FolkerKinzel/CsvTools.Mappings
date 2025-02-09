using FolkerKinzel.CsvTools.Mappings.TypeConverters;

namespace FolkerKinzel.CsvTools.Mappings.Intls.Converters;

internal sealed class IReadOnlyListConverter<TItem> : TypeConverter<IReadOnlyList<TItem?>?>
{
    private readonly ListConverter<TItem> _listConverter;

    internal IReadOnlyListConverter(TypeConverter<TItem?> itemsConverter, string separator, bool nullable)
        : base(nullable ? null : [],
               itemsConverter?.Throwing ?? throw new ArgumentNullException(nameof(itemsConverter)))
    {
        _listConverter = new ListConverter<TItem>(itemsConverter, separator, nullable);
    }

    public override bool AcceptsNull => true;

    public override string? ConvertToString(IReadOnlyList<TItem?>? value)
        => _listConverter.DoConvertToString(value);

    public override bool TryParse(ReadOnlySpan<char> value, out IReadOnlyList<TItem?>? result)
    {
        _ = _listConverter.TryParse(value, out List<TItem?>? list);
        result = list;
        return true;
    }
}


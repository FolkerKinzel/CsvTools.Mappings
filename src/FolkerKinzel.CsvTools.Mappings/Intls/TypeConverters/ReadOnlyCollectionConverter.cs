using FolkerKinzel.CsvTools.Mappings.TypeConverters;
using System.Collections.ObjectModel;

namespace FolkerKinzel.CsvTools.Mappings.Intls.TypeConverters;

internal sealed class ReadOnlyCollectionConverter<TItem>
    : TypeConverter<ReadOnlyCollection<TItem?>?>
{
    private readonly ListConverter<TItem> _listConverter;

    internal ReadOnlyCollectionConverter(TypeConverter<TItem?> itemsConverter,
                                         string separator,
                                         bool nullable)
        : base(itemsConverter?.Throwing ?? throw new ArgumentNullException(nameof(itemsConverter)),
               nullable ? null : new ReadOnlyCollection<TItem?>(Array.Empty<TItem>()))
    {
        _listConverter = new ListConverter<TItem>(itemsConverter, separator, nullable);
    }

    public override bool AcceptsNull => true;

    public override string? ConvertToString(ReadOnlyCollection<TItem?>? value)
        => _listConverter.DoConvertToString(value);

    public override bool TryParse(ReadOnlySpan<char> value, out ReadOnlyCollection<TItem?>? result)
    {
        _ = _listConverter.TryParse(value, out List<TItem?>? list);
        Debug.Assert(list != null);
        result = new ReadOnlyCollection<TItem?>(list);
        return true;
    }
}


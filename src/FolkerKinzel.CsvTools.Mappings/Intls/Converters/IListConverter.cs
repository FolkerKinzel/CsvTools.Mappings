﻿using FolkerKinzel.CsvTools.Mappings.TypeConverters;

namespace FolkerKinzel.CsvTools.Mappings.Intls.Converters;

internal sealed class IListConverter<TItem> : TypeConverter<IList<TItem?>?>
{
    private readonly ListConverter<TItem> _listConverter;

    internal IListConverter(TypeConverter<TItem?> itemsConverter, string separator, bool nullable)
        : base(itemsConverter?.Throwing ?? throw new ArgumentNullException(nameof(itemsConverter)),
               nullable ? null : [])
    {
        _listConverter = new ListConverter<TItem>(itemsConverter, separator, nullable);
    }

    public override bool AcceptsNull => true;

    public override string? ConvertToString(IList<TItem?>? value)
        => _listConverter.DoConvertToString(value);

    public override bool TryParse(ReadOnlySpan<char> value, out IList<TItem?>? result)
    {
        _ = _listConverter.TryParse(value, out List<TItem?>? list);
        result = list;
        return true;
    }
}


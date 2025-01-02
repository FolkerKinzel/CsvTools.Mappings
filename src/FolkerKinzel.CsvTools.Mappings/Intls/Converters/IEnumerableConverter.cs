using FolkerKinzel.CsvTools.Mappings.Converters;
using FolkerKinzel.CsvTools.Mappings.Resources;
using System.Text;

namespace FolkerKinzel.CsvTools.Mappings.Intls.Converters;

internal sealed class IEnumerableConverter<TItem> : TypeConverter<IEnumerable<TItem?>?>
{
    private readonly string _separator;
    private readonly TypeConverter<TItem?> _itemsConverter;

    public override bool AllowsNull => true;

    /// <summary>
    /// Initializes a new <see cref="IEnumerableConverter{TItem}"/> instance.
    /// </summary>
    /// <param name="itemsConverter">A <see cref="TypeConverter{T}"/> instance that converts the items.</param>
    /// <param name="separator">A <see cref="string"/> that separates the items in field of the CSV file. When parsing
    /// the CSV, <paramref name="separator"/> will not be part of the results.</param>    
    /// <param name="nullable"><c>true</c> to set <see cref="TypeConverter{T}.FallbackValue"/>
    /// to <c>null</c>; <c>false</c> to have <see cref="Enumerable.Empty{TResult}"/> as 
    /// <see cref="TypeConverter{T}.FallbackValue"/>.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="itemsConverter"/> or <paramref name="separator"/>
    /// is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="separator"/> is an <see cref="string.Empty"/>.</exception>
    internal IEnumerableConverter(TypeConverter<TItem?> itemsConverter, string separator, bool nullable)
        : base(itemsConverter?.Throwing ?? throw new ArgumentNullException(nameof(itemsConverter)), 
               nullable ? null : [])
    {
        _itemsConverter = itemsConverter;
        _separator = separator ?? throw new ArgumentNullException(nameof(separator));

        if (separator.Length == 0)
        {
            throw new ArgumentException(Res.EmptySeparator, nameof(separator));
        }
    }

    public override string? ConvertToString(IEnumerable<TItem?>? value)
        => value is null || !value.Any() 
            ? null 
            : string.Join(_separator, value.Select(x => _itemsConverter.ConvertToString(x)));

    public override bool TryParseValue(ReadOnlySpan<char> value, out IEnumerable<TItem?>? result)
    {
        Debug.Assert(_separator.Length > 0);   
        Debug.Assert(_itemsConverter.Throwing == Throwing);

        const int notFound = -1;
        var list = new List<TItem?>();

        while (true)
        {
            int idx = value.IndexOf(_separator, StringComparison.Ordinal);

            if (idx == notFound)
            {
                // inherits the Throwing behavior from _itemsConverter:
                list.Add(_itemsConverter.Parse(value));
                result = list;
                return true;
            }

            list.Add(_itemsConverter.Parse(value.Slice(0,idx)));
            idx += _separator.Length;

            value = value.Slice(idx);
        }
    }
}

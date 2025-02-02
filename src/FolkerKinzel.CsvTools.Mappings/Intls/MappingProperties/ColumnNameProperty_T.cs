using FolkerKinzel.CsvTools.Mappings.Converters;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace FolkerKinzel.CsvTools.Mappings.Intls.MappingProperties;

/// <summary>
/// Represents a dynamic property of <see cref="Mapping"/> ("late binding") for processing CSV files with header row.
/// </summary>
/// <typeparam name="T">The .NET data type of the dynamic property.</typeparam>
/// <remarks>
/// <see cref="ColumnNameProperty{T}"/> 
/// encapsulates information about access and type conversion, which <see cref="Mapping"/> needs to access the data of the underlying
/// <see cref="CsvRecord"/> object with its CSV column name.
/// </remarks>
internal sealed class ColumnNameProperty<T> : SingleColumnProperty<T>, ICloneable
{
    /// <summary>
    /// Ein Hashcode, der für alle <see cref="CsvRecord"/>-Objekte, die zum selben Lese- oder Schreibvorgang
    /// gehören, identisch ist. (Wird von <see cref="ColumnNameProperty{T}"/> verwendet, um festzustellen,
    /// ob der Zugriffsindex aktuell ist.)
    /// </summary>
    private int _csvRecordIdentifier;
    private readonly TimeSpan _wildcardTimeout;
    private int? _csvIndex;

    /// <summary>
    /// Initializes a new <see cref="ColumnNameProperty{T}"/> instance.
    /// </summary>
    /// <param name="propertyName">The identifier under which the property is addressed. It must follow the rules for C# 
    /// identifiers. Only ASCII characters are accepted.</param>
    ///<param name="columnNameAliases">
    /// <para>
    /// Column names of the CSV file that the <see cref="DynamicProperty"/> can access. The first alias that is a match 
    /// with a column name of the CSV file is used. The alias <see cref="string"/>s may contain the 
    /// wildcard characters * and ?. 
    /// </para>
    /// <para>
    /// If a wildcard alias matches several columns in the CSV file, the column with the lowest index is referenced.
    /// </para>
    /// <para>
    /// The collection will be copied.
    /// </para>
    /// </param>
    /// <param name="converter">The <see cref="TypeConverter{T}"/> that does the type conversion.</param>
    /// 
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> or <paramref name="columnNameAliases"/>
    /// is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="propertyName"/> does not conform to the rules for C# 
    /// identifiers (only ASCII characters).</exception>
    /// <exception cref="RegexMatchTimeoutException">
    /// Validating of <paramref name="propertyName"/> takes longer than <see cref="Mapping.MaxRegexTimeout"/>.
    /// </exception>
    public ColumnNameProperty(string propertyName,
                              IEnumerable<string?> columnNameAliases,
                              TypeConverter<T> converter)
        : base(propertyName, converter)
    {
        _ArgumentNullException.ThrowIfNull(columnNameAliases, nameof(columnNameAliases));

        _wildcardTimeout = GetTimeout(Mapping.RegexTimeout);
        ColumnNameAliases = columnNameAliases.OfType<string>().ToArray();

        static TimeSpan GetTimeout(int wildcardTimeout)
        {
            return wildcardTimeout == Timeout.Infinite
                            ? Regex.InfiniteMatchTimeout
                            : TimeSpan.FromMilliseconds(wildcardTimeout);
        }
    }

    private ColumnNameProperty(ColumnNameProperty<T> source) : base(source)
    {
        _csvRecordIdentifier = source._csvRecordIdentifier;
        _wildcardTimeout = source._wildcardTimeout;
        _csvIndex = source._csvIndex;
        ColumnNameAliases = source.ColumnNameAliases;
    }

    

    /// <inheritdoc/>
    public override object Clone() => new ColumnNameProperty<T>(this);

    /// <summary>
    /// Collection of alternative column names of the CSV file, which is used by <see cref="Mapping"/> to access
    /// a column of <see cref="CsvRecord"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// In order to be compatible with different CSV files, e.g., different spellings of the same column name, or translations
    /// of the column name into other languages can be specified. An alias can also contain the wildcard characters * and ?. 
    /// </para>
    /// <para>
    /// Since all aliases are taken into account when setting the value of <see cref="ColumnNameProperty{T}"/>, it is not 
    /// recommended to assign the same alias to several <see cref="CsvRecord"/> objects. 
    /// </para>
    /// </remarks>
    public IReadOnlyList<string> ColumnNameAliases { get; }
    

    /// <inheritdoc/>
    internal protected override int? GetCsvIndex()
    {
        if (Record is null) 
        { 
            _csvIndex = null;
        }
        else if (_csvRecordIdentifier != Record.Identifier)
        {
            _csvRecordIdentifier = Record.Identifier;
            _csvIndex = GetReferredIndex();
        }

        return _csvIndex;
    }

    #region private

    private int? GetReferredIndex()
    {
        Debug.Assert(Record is not null);
        Debug.Assert(ColumnNameAliases is string[]);
        Debug.Assert(Record.ColumnNames is string[]);

        ReadOnlySpan<string> columnNames = ((string[])Record.ColumnNames).AsSpan();
        ReadOnlySpan<string> aliases = ((string[])ColumnNameAliases).AsSpan();
        RegexOptions? regexOptions = null;

        foreach (string alias in aliases)
        {
            if (HasWildcard(alias))
            {
                if (!regexOptions.HasValue)
                {
                    regexOptions = InitRegexOptions(Record.HasCaseSensitiveColumnNames);
                }

                string pattern = CreateRegexPattern(alias);

                for (int k = 0; k < columnNames.Length; k++)
                {
                    string columnName = columnNames[k];

                    try
                    {
                        if (Regex.IsMatch(columnName, pattern, regexOptions.Value, _wildcardTimeout))
                        {
                            return k;
                        }
                    }
                    catch (TimeoutException)
                    {
                        Debug.WriteLine(nameof(RegexMatchTimeoutException));
                    }
                }
            }
            else
            {
                for (int j = 0; j < columnNames.Length; j++)
                {
                    if (Record.Comparer.Equals(columnNames[j], alias)) // Es kann in columnNames keine 2 Strings geben, auf die das zutrifft.
                    {
                        return j;
                    }
                }
            }
        }//for

        return null;

        ////////////////////////////////////////////////////////////////////

        static bool HasWildcard(string alias) => alias.AsSpan().ContainsAny('*', '?');

        static RegexOptions InitRegexOptions(bool caseSensitive)
        {
            return caseSensitive
                     ? RegexOptions.CultureInvariant | RegexOptions.Singleline
                     : RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline;
        }

        static string CreateRegexPattern(string alias)
          => $"^{Regex
            .Escape(alias)
            .Replace("\\?", ".", StringComparison.Ordinal)
            .Replace("\\*", ".*?", StringComparison.Ordinal)}$";
    }

    #endregion
}

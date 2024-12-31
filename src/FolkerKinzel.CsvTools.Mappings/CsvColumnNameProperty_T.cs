using FolkerKinzel.CsvTools.Mappings.Converters;
using FolkerKinzel.CsvTools.Mappings.Intls;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace FolkerKinzel.CsvTools.Mappings;

/// <summary>
/// Represents a dynamic property of <see cref="CsvRecordMapping"/> ("late binding") for processing CSV files with header row.
/// </summary>
/// <typeparam name="T">The .NET data type of the dynamic property.</typeparam>
/// <remarks>
/// <see cref="CsvColumnNameProperty{T}"/> 
/// encapsulates information about access and type conversion, which <see cref="CsvRecordMapping"/> needs to access the data of the underlying
/// <see cref="CsvRecord"/> object with its CSV column name.
/// </remarks>
public sealed class CsvColumnNameProperty<T> : CsvSingleColumnProperty<T>
{
    /// <summary>
    /// Maximum time (in milliseconds) that can be used to resolve a column name alias.
    /// </summary>
    public const int MaxWildcardTimeout = 100;

    /// <summary>
    /// Ein Hashcode, der für alle <see cref="CsvRecord"/>-Objekte, die zum selben Lese- oder Schreibvorgang
    /// gehören, identisch ist. (Wird von <see cref="CsvColumnNameProperty{T}"/> verwendet, um festzustellen,
    /// ob der Zugriffsindex aktuell ist.)
    /// </summary>
    private int _csvRecordIdentifier;
    private readonly int _wildcardTimeout;

    /// <summary>
    /// Initializes a new <see cref="CsvColumnNameProperty{T}"/> instance.
    /// </summary>
    /// <param name="propertyName">The identifier under which the property is addressed. It must follow the rules for C# identifiers. 
    /// Only ASCII characters are accepted.</param>
    /// <param name="columnNameAliases">
    /// Column names of the CSV file that <see cref="CsvColumnNameProperty{T}"/> can access. For the access <see cref="CsvColumnNameProperty{T}"/> 
    /// uses the first alias that is a match with a column name of the CSV file. The alias strings may contain the wildcard characters * and ?. 
    /// If a wildcard alias matches several columns in the CSV file, the column with the lowest index is referenced.</param>
    /// <param name="converter">The <see cref="CsvTypeConverter{T}"/> that does the type conversion.</param>
    /// <param name="wildcardTimeout">
    /// Timeout value in milliseconds or 0, for <see cref="Regex.InfiniteMatchTimeout"/>. If the value is greater than <see cref="MaxWildcardTimeout"/>
    /// it is normalized to this value. If an alias in <paramref name="columnNameAliases"/> contains wildcard characters, inside this timeout the program 
    /// tries to resolve the alias. If this does not succeed, <see cref="CsvColumnNameProperty{T}"/> reacts as if it had no target in the columns of the CSV file. 
    /// </param>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> does not conform to the rules for C# identifiers (only ASCII characters).</exception>
    /// 
    /// <exception cref="ArgumentException"><paramref name="propertyName"/> entspricht nicht den Regeln für C#-Bezeichner (nur
    /// ASCII-Zeichen).</exception>
    /// 
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="wildcardTimeout"/> is less than Zero.</exception>
    public CsvColumnNameProperty(string propertyName,
                                 IEnumerable<string> columnNameAliases,
                                 CsvTypeConverter<T> converter,
                                 int wildcardTimeout = 10)
        : base(propertyName, converter)
    {
        _ArgumentNullException.ThrowIfNull(columnNameAliases, nameof(columnNameAliases));
        
        if (wildcardTimeout > MaxWildcardTimeout)
        {
            wildcardTimeout = MaxWildcardTimeout;
        }
        else
        {
            _ArgumentOutOfRangeException.ThrowIfNegative(wildcardTimeout, nameof(wildcardTimeout));
        }

        this.ColumnNameAliases = new ReadOnlyCollection<string>(columnNameAliases.Where(x => x != null).ToArray());
        this._wildcardTimeout = wildcardTimeout;
    }

    /// <summary>
    /// Collection of alternative column names of the CSV file, which is used by <see cref="CsvRecordMapping"/> to access
    /// a column of <see cref="CsvRecord"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// In order to be compatible with different CSV files, e.g., different spellings of the same column name, or translations
    /// of the column name into other languages can be specified. An alias can also contain the wildcard characters * and ?. 
    /// </para>
    /// <para>
    /// Since all aliases are taken into account when setting the value of <see cref="CsvColumnNameProperty{T}"/>, it is not 
    /// recommended to assign the same alias to several <see cref="CsvRecord"/> objects. 
    /// </para>
    /// </remarks>
    public ReadOnlyCollection<string> ColumnNameAliases { get; }

    /// <inheritdoc/>
    protected override void UpdateReferredCsvIndex()
    {
        Debug.Assert(Record is not null);
        if (_csvRecordIdentifier != Record.Identifier)
        {
            _csvRecordIdentifier = Record.Identifier;
            ReferredCsvIndex = GetReferredIndex(); ;
        }
    }

    #region private

    private int? GetReferredIndex()
    {
        Debug.Assert(Record is not null);

        IEqualityComparer<string>? comparer = Record.Comparer;
        IReadOnlyList<string>? columnNames = Record.ColumnNames;

        for (int i = 0; i < ColumnNameAliases.Count; i++)
        {
            string alias = ColumnNameAliases[i];

            if (alias is null)
            {
                continue;
            }

            if (HasWildcard(alias))
            {
                Regex regex = InitRegex(comparer, alias, _wildcardTimeout);

                for (int k = 0; k < columnNames.Count; k++) // Die Wildcard könnte auf alle keys passen.
                {
                    string columnName = columnNames[k];

                    try
                    {
                        if (regex.IsMatch(columnName))
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
                for (int j = 0; j < columnNames.Count; j++)
                {
                    string columnName = columnNames[j];

                    if (comparer.Equals(columnName, alias)) // Es kann in columnNames keine 2 Strings geben, auf die das zutrifft.
                    {
                        return j;
                    }
                }
            }
        }//for
        return null;

        static bool HasWildcard(string alias)
        {
            // Suche Wildcardzeichen im alias
            for (int j = 0; j < alias.Length; j++)
            {
                char c = alias[j];

                if (c is '*' or '?')
                {
                    return true;
                }
            }//for

            return false; // keine Wildcard-Zeichen
        }//HasWildcard
    }

    private static Regex InitRegex(IEqualityComparer<string> comparer, string alias, int wildcardTimeout)
    {
        string pattern = "^" +
            Regex
            .Escape(alias)
            .Replace("\\?", ".", StringComparison.Ordinal)
            .Replace("\\*", ".*?", StringComparison.Ordinal) + "$";

        RegexOptions options = comparer.Equals(StringComparer.OrdinalIgnoreCase) ?
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline :
                                      RegexOptions.CultureInvariant | RegexOptions.Singleline;

        // Da das Regex nicht wiederverwendbar ist, wird die Instanzmethode
        // verwendet.
        return new Regex(pattern,
                         options,
                         wildcardTimeout == 0 ? Regex.InfiniteMatchTimeout
                                              : TimeSpan.FromMilliseconds(wildcardTimeout));
    }

    #endregion
}

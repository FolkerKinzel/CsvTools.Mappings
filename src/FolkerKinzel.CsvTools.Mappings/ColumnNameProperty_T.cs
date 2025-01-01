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
/// <see cref="ColumnNameProperty{T}"/> 
/// encapsulates information about access and type conversion, which <see cref="CsvRecordMapping"/> needs to access the data of the underlying
/// <see cref="CsvRecord"/> object with its CSV column name.
/// </remarks>
public sealed class ColumnNameProperty<T> : SingleColumnProperty<T>
{
    /// <summary>
    /// Ein Hashcode, der für alle <see cref="CsvRecord"/>-Objekte, die zum selben Lese- oder Schreibvorgang
    /// gehören, identisch ist. (Wird von <see cref="ColumnNameProperty{T}"/> verwendet, um festzustellen,
    /// ob der Zugriffsindex aktuell ist.)
    /// </summary>
    private int _csvRecordIdentifier;
    private readonly TimeSpan _wildcardTimeout;

    /// <summary>
    /// Initializes a new <see cref="ColumnNameProperty{T}"/> instance.
    /// </summary>
    /// <param name="propertyName">The identifier under which the property is addressed. It must follow the rules for C# 
    /// identifiers. Only ASCII characters are accepted.</param>
    /// <param name="columnNameAliases">
    /// Column names of the CSV file that <see cref="ColumnNameProperty{T}"/> can access. For the access 
    /// <see cref="ColumnNameProperty{T}"/> 
    /// uses the first alias that is a match with a column name of the CSV file. The alias strings may contain the 
    /// wildcard characters * and ?. 
    /// If a wildcard alias matches several columns in the CSV file, the column with the lowest index is referenced.</param>
    /// <param name="converter">The <see cref="TypeConverter{T}"/> that does the type conversion.</param>
    /// <param name="wildcardTimeout">
    /// <para>
    /// The timeout value in milliseconds used to resolve a single column name alias. Set this value to 
    /// <see cref="Timeout.Infinite"/> to disable the timeout. If the value is greater than 
    /// <see cref="CsvRecordMapping.MaxRegexTimeout"/>, it is normalized to this value.
    /// </para>
    /// <para>
    /// If an alias in <paramref name="columnNameAliases"/> cannot be resolved inside this timeout, 
    /// <see cref="ColumnNameProperty{T}"/> reacts as if it had no target in the columns of the CSV file. 
    /// </para>
    /// </param>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> or <paramref name="columnNameAliases"/>
    /// is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="propertyName"/> does not conform to the rules for C# 
    /// identifiers (only ASCII characters).</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="wildcardTimeout"/> is less than 1 and not 
    /// <see cref="Timeout.Infinite"/>.</exception>
    /// <exception cref="RegexMatchTimeoutException">
    /// Validating of <paramref name="propertyName"/> takes longer than <see cref="CsvRecordMapping.MaxRegexTimeout"/>.
    /// </exception>
    public ColumnNameProperty(string propertyName,
                              IEnumerable<string> columnNameAliases,
                              TypeConverter<T> converter,
                              int wildcardTimeout = 10)
        : base(propertyName, converter)
    {
        _ArgumentNullException.ThrowIfNull(columnNameAliases, nameof(columnNameAliases));

        this._wildcardTimeout = GetTimeout(wildcardTimeout);
        this.ColumnNameAliases = new ReadOnlyCollection<string>(columnNameAliases.Where(x => x != null).ToArray());

        static TimeSpan GetTimeout(int wildcardTimeout)
        {
            wildcardTimeout = TimeoutHelper.NormalizeRegexTimeout(wildcardTimeout, nameof(wildcardTimeout));
            return wildcardTimeout == Timeout.Infinite
                            ? Regex.InfiniteMatchTimeout
                            : TimeSpan.FromMilliseconds(wildcardTimeout);
        }
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
    /// Since all aliases are taken into account when setting the value of <see cref="ColumnNameProperty{T}"/>, it is not 
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

        IReadOnlyList<string>? columnNames = Record.ColumnNames;
        RegexOptions? regexOptions = null;

        for (int i = 0; i < ColumnNameAliases.Count; i++)
        {
            string alias = ColumnNameAliases[i];

            if (alias is null)
            {
                continue;
            }

            if (HasWildcard(alias))
            {
                if(!regexOptions.HasValue)
                {
                    regexOptions = InitRegexOptions(Record.HasCaseSensitiveColumnNames);
                }

                string pattern = CreateRegexPattern(alias);

                for (int k = 0; k < columnNames.Count; k++)
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
                for (int j = 0; j < columnNames.Count; j++)
                {
                    string columnName = columnNames[j];

                    if (Record.Comparer.Equals(columnName, alias)) // Es kann in columnNames keine 2 Strings geben, auf die das zutrifft.
                    {
                        return j;
                    }
                }
            }
        }//for

        return null;

        ////////////////////////////////////////////////////////////////////

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

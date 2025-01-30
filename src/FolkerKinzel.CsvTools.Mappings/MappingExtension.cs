using FolkerKinzel.CsvTools.Mappings.Converters;
using FolkerKinzel.CsvTools.Mappings.Intls;
using FolkerKinzel.CsvTools.Mappings.Intls.Extensions;
using FolkerKinzel.CsvTools.Mappings.Intls.MappingProperties;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace FolkerKinzel.CsvTools.Mappings;

/// <summary>
/// Extension methods for <see cref="Mapping"/>.
/// </summary>
public static class MappingExtension
{
    /// <summary>
    /// Removes all <see cref="DynamicProperty"/> instances from the <see cref="Mapping"/>.
    /// </summary>
    /// <param name="mapping">The <see cref="Mapping"/> whose content has to be removed.</param>
    /// <returns><paramref name="mapping"/> after all its <see cref="DynamicProperty"/> instances have 
    /// been removed (to chain calls).</returns>
    public static Mapping Clear(this Mapping mapping)
    {
        _ArgumentNullException.ThrowIfNull(mapping, nameof(mapping));
        mapping.Clear();
        return mapping;
    }

    /// <summary>
    /// Adds a new <see cref="DynamicProperty"/> instance to the <see cref="Mapping"/>, which accesses 
    /// a single column of the CSV file with its index.
    /// </summary>
    /// <typeparam name="T">The .NET data type of the dynamic property.</typeparam>
    /// <param name="mapping">The <see cref="Mapping"/> to add the <see cref="DynamicProperty"/> to.</param>
    /// <param name="propertyName">The identifier of the dynamic .NET property. It must follow the 
    /// rules for C# identifiers. Only ASCII characters are accepted.
    /// </param>
    /// <param name="csvIndex">Zero-based index of the column in the CSV file.
    /// If this index doesn't exist, the <see cref="DynamicProperty"/> is ignored 
    /// when writing. When reading, in this case, <see cref="TypeConverter{T}.DefaultValue"/> is returned.</param>
    /// <param name="converter">The <see cref="TypeConverter{T}"/> that does the type conversion.</param>
    /// 
    /// <returns><paramref name="mapping"/> with the added <see cref="DynamicProperty"/> to chain calls.</returns>
    /// 
    /// <remarks>Use this method if a CSV file has no header, or, or performance reasons, if the CSV column 
    /// index is known.</remarks>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="mapping"/>, 
    /// or <paramref name="propertyName"/>, or <paramref name="converter"/> is <c>null</c>.</exception>
    /// 
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="csvIndex"/>  is less than Zero.</exception>
    /// <exception cref="ArgumentException">
    /// <para>
    /// <paramref name="propertyName"/> does not conform to the rules for C# 
    /// identifiers (only ASCII characters)
    /// </para>
    /// <para>- or -</para>
    /// <para>
    /// a <see cref="DynamicProperty"/> with the same <see cref="DynamicProperty.PropertyName"/> has already been added.
    /// Check this beforehand with <see cref="Mapping.Contains(string)"/>!
    /// </para>
    /// </exception>
    /// <exception cref="RegexMatchTimeoutException">
    /// Validating of <paramref name="propertyName"/> takes longer than <see cref="Mapping.MaxRegexTimeout"/>.
    /// </exception>
    public static Mapping AddProperty<T>(this Mapping mapping,
                                         string propertyName,
                                         int csvIndex,
                                         TypeConverter<T> converter)
    {
        _ArgumentNullException.ThrowIfNull(mapping, nameof(mapping));

        mapping.Add(new IndexProperty<T>(propertyName, csvIndex, converter));
        return mapping;
    }

    /// <summary>
    /// Adds a new <see cref="DynamicProperty"/> instance to the <see cref="Mapping"/>, which accesses a 
    /// single column of a CSV file with header with a collection of column name aliases.
    /// </summary>
    /// <typeparam name="T">The .NET data type of the dynamic property.</typeparam>
    /// <param name="mapping">The <see cref="Mapping"/> to add the <see cref="DynamicProperty"/> to.</param>
    /// <param name="propertyName">The identifier under which the property is addressed. It must follow the 
    /// rules for C# identifiers. Only ASCII characters are accepted.
    /// </param>
    /// <param name="columnNameAliases">
    /// <para>
    /// Column names of the CSV file that the <see cref="DynamicProperty"/> can access. The aliases can use the 
    /// wildcard characters '*' and '?'. 
    /// </para>
    /// <para>
    /// The first alias, which is a match with a column name of the CSV file, is used. If a wildcard alias matches 
    /// several columns in the CSV file, the column with the lowest index is referenced.
    /// </para>
    /// <para>
    /// The collection will be copied.
    /// </para>
    /// </param>
    /// <param name="converter">The <see cref="TypeConverter{T}"/> that does the type conversion.</param>
    /// 
    ///  
    /// <returns><paramref name="mapping"/> with the added <see cref="DynamicProperty"/> to chain calls.</returns>
    /// 
    /// <remarks>
    /// <para>
    /// Use this method if a CSV column name doesn't match the requirements of C# 
    /// identifiers, or if the CSV column name is unknown.
    /// </para>
    /// <para>
    /// In the very special case where '*' is a letter of the column name, replace '*' with '?'.
    /// </para>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="mapping"/>, 
    /// or <paramref name="propertyName"/>, or <paramref name="columnNameAliases"/>, or <paramref name="converter"/>
    /// is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// <para>
    /// <paramref name="propertyName"/> does not conform to the rules for C# 
    /// identifiers (only ASCII characters)
    /// </para>
    /// <para>- or -</para>
    /// <para>
    /// a <see cref="DynamicProperty"/> with the same <see cref="DynamicProperty.PropertyName"/> has already been added.
    /// Check this beforehand with <see cref="Mapping.Contains(string)"/>!
    /// </para>
    /// </exception>
    /// <exception cref="RegexMatchTimeoutException">
    /// Validating of <paramref name="propertyName"/> takes longer than <see cref="Mapping.MaxRegexTimeout"/>.
    /// </exception>
    public static Mapping AddProperty<T>(this Mapping mapping,
                                         string propertyName,
                                         IEnumerable<string?> columnNameAliases,
                                         TypeConverter<T> converter)
    {
        _ArgumentNullException.ThrowIfNull(mapping, nameof(mapping));

        mapping.Add(new ColumnNameProperty<T>(propertyName, columnNameAliases, converter));
        return mapping;
    }

    /// <summary>
    /// Adds a new <see cref="DynamicProperty"/> instance to the <see cref="Mapping"/>, which accesses a single 
    /// column of a CSV file with header with the column name.
    /// 
    /// </summary>
    /// <typeparam name="T">The .NET data type of the dynamic property.</typeparam>
    /// <param name="mapping">The <see cref="Mapping"/> to add the <see cref="DynamicProperty"/> to.</param>
    /// <param name="propertyName">The identifier of the dynamic .NET property and the corresponding column of the 
    /// CSV file. The value of the argument must follow the rules for C# identifiers. Only ASCII characters are accepted.
    /// </param>
    /// <param name="converter">The <see cref="TypeConverter{T}"/> that does the type conversion.</param>
    /// 
    ///  
    /// <returns><paramref name="mapping"/> with the added <see cref="DynamicProperty"/> to chain calls.</returns>
    /// 
    /// <remarks>
    /// When using this method, <paramref name="propertyName"/> and the referenced CSV column name must match, 
    /// and <paramref name="propertyName"/> must meet the requirements for C# identifiers. Use the 
    /// <see cref="CsvRecord.HasCaseSensitiveColumnNames"/> property to determine whether the comparison is 
    /// case-sensitive.
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="mapping"/>, 
    /// or <paramref name="propertyName"/>, or <paramref name="converter"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// <para>
    /// <paramref name="propertyName"/> does not conform to the rules for C# identifiers (only ASCII characters)
    /// </para>
    /// <para>- or -</para>
    /// <para>
    /// a <see cref="DynamicProperty"/> with the same <see cref="DynamicProperty.PropertyName"/> has already been added.
    /// Check this beforehand with <see cref="Mapping.Contains(string)"/>!
    /// </para>
    /// </exception>
    /// <exception cref="RegexMatchTimeoutException">
    /// Validating of <paramref name="propertyName"/> takes longer than <see cref="Mapping.MaxRegexTimeout"/>.
    /// </exception>
    public static Mapping AddProperty<T>(this Mapping mapping,
                                         string propertyName,
                                         TypeConverter<T> converter)
        => AddProperty(mapping, propertyName, [propertyName], converter);

    /// <summary>
    /// Adds a new <see cref="DynamicProperty"/> instance to the <see cref="Mapping"/>, which accesses 
    /// several columns of a CSV file.
    /// </summary>
    /// <typeparam name="T">The .NET data type of the dynamic property.</typeparam>
    /// <param name="mapping">The <see cref="Mapping"/> to add the <see cref="DynamicProperty"/> to.</param>
    /// <param name="propertyName">The identifier of the dynamic .NET property. It must follow the 
    /// rules for C# identifiers. Only ASCII characters are accepted.
    /// </param>
    /// <param name="converter">An instance derived from the abstract <see cref="MultiColumnTypeConverter{T}"/> 
    /// class. You have to write this class yourself because it depends on the CSV file.</param>
    /// 
    /// <returns><paramref name="mapping"/> with the added <see cref="DynamicProperty"/> to chain calls.</returns>
    /// 
    /// <remarks>
    /// Use this method if the dynamic property is based on several columns of the CSV file.
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    /// Using <see cref="MultiColumnTypeConverter{T}"/> to create and parse a CSV file.
    /// </para>
    /// <para>
    /// (For the sake of easier readability exception handling has been omitted.)
    /// </para>
    /// <img src="images\MultiColumnConverter.png"/>
    /// <code language="cs" source="../Examples/MultiColumnConverterExample.cs"/>
    /// </example>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="mapping"/>, 
    /// or <paramref name="propertyName"/>, or <paramref name="converter"/> is <c>null</c>.</exception>
    /// 
    /// <exception cref="ArgumentException">
    /// <para>
    /// <paramref name="propertyName"/> does not conform to the rules for C# 
    /// identifiers (only ASCII characters)
    /// </para>
    /// <para>- or -</para>
    /// <para>
    /// a <see cref="DynamicProperty"/> with the same <see cref="DynamicProperty.PropertyName"/> has already been added.
    /// Check this beforehand with <see cref="Mapping.Contains(string)"/>!
    /// </para>
    /// </exception>
    /// <exception cref="RegexMatchTimeoutException">
    /// Validating of <paramref name="propertyName"/> takes longer than <see cref="Mapping.MaxRegexTimeout"/>.
    /// </exception>
    public static Mapping AddProperty<T>(this Mapping mapping,
                                         string propertyName,
                                         MultiColumnTypeConverter<T> converter)
    {
        _ArgumentNullException.ThrowIfNull(mapping, nameof(mapping));

        mapping.Add(new MultiColumnProperty<T>(propertyName, converter));
        return mapping;
    }

    /// <summary>
    /// Fills <paramref name="mapping"/> with the items of 
    /// a collection. 
    /// </summary>
    /// <param name="mapping">The <see cref="Mapping"/> to fill.</param>
    /// <param name="data">The collection whose content is used to fill 
    /// <paramref name="mapping"/>.</param>
    /// <param name="resetExcess">
    /// If <paramref name="data"/> has fewer items than <paramref name="mapping"/> has
    /// <see cref="DynamicProperty"/> instances and this parameter is <c>true</c>, the surplus 
    /// properties in record will be reset to their <see cref="DynamicProperty.DefaultValue"/>. 
    /// For performance reasons this parameter can be set to <c>false</c> when writing CSV because 
    /// <see cref="CsvWriter.WriteRecord"/> resets all fields in <paramref name="mapping"/>.
    /// </param>
    /// 
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="mapping"/> or <paramref name="data"/>
    /// is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="data"/> has more items than the
    /// number of <see cref="DynamicProperty"/> instances in <paramref name="mapping"/>.</exception>
    /// <exception cref="InvalidOperationException"> The <see cref="Mapping.Record"/> property of
    /// <paramref name="mapping"/> is <c>null</c>. Assign a 
    /// <see cref="CsvRecord"/> instance to <paramref name="mapping"/> before calling 
    /// this method.
    /// </exception>
    /// <exception cref="InvalidCastException">
    /// An item in <paramref name="data"/> does not match the expected data type.
    /// </exception>
    /// <exception cref="FormatException">
    /// One of the <see cref="TypeConverter{T}"/> instances uses an invalid format string.
    /// </exception>
    public static void FillWith(this Mapping mapping,
                                IEnumerable<object?> data,
                                bool resetExcess = true)
    {
        _ArgumentNullException.ThrowIfNull(mapping, nameof(mapping));
        _ArgumentNullException.ThrowIfNull(data, nameof(data));

        int i = 0;

        foreach (object? item in data)
        {
            if (i >= mapping.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(data));
            }

            mapping[i++].Value = item;
        }

        if (resetExcess)
        {
            for (; i < mapping.Count; i++)
            {
                DynamicProperty prop = mapping[i];
                prop.Value = prop.DefaultValue;
            }
        }
    }

    /// <summary>
    /// Fills <paramref name="mapping"/> with the fields of 
    /// a <see cref="DataRow"/>.
    /// </summary>
    /// <param name="mapping">The <see cref="Mapping"/> to fill.</param>
    /// <param name="dataRow">The <see cref="DataRow"/> whose content is used to fill 
    /// <paramref name="mapping"/>.</param>
    /// <param name="resetExcess">
    /// If <paramref name="dataRow"/> has fewer <see cref="DataColumn"/>s than <paramref name="mapping"/>
    /// has <see cref="DynamicProperty"/> instances and this parameter is <c>true</c>, the surplus 
    /// properties in record will be reset to their <see cref="DynamicProperty.DefaultValue"/>. 
    /// For performance reasons this parameter can be set to <c>false</c> when writing CSV because 
    /// <see cref="CsvWriter.WriteRecord"/> resets all fields in <paramref name="mapping"/>.
    /// </param>
    /// 
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="mapping"/> or <paramref name="dataRow"/>
    /// is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="dataRow"/> has more 
    /// <see cref="DataColumn"/>s than the number of <see cref="DynamicProperty"/> instances 
    /// in <paramref name="mapping"/>.</exception>
    /// <exception cref="InvalidOperationException"> The <see cref="Mapping.Record"/> property of
    /// <paramref name="mapping"/> is <c>null</c>. Assign a 
    /// <see cref="CsvRecord"/> instance to <paramref name="mapping"/> before calling 
    /// this method.
    /// </exception>
    /// <exception cref="InvalidCastException">
    /// A value in <paramref name="dataRow"/> does not match the expected data type.
    /// </exception>
    /// <exception cref="FormatException">
    /// One of the <see cref="TypeConverter{T}"/> instances uses an invalid format string.
    /// </exception>
    public static void FillWith(this Mapping mapping, DataRow dataRow, bool resetExcess = true)
    {
        _ArgumentNullException.ThrowIfNull(mapping, nameof(mapping));
        _ArgumentNullException.ThrowIfNull(dataRow, nameof(dataRow));

        if (dataRow.Table.Columns.Count > mapping.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(dataRow));
        }

        int i = 0;

        for (; i < dataRow.Table.Columns.Count; i++)
        {
            mapping[i].Value = dataRow[i];
        }

        if (resetExcess)
        {
            for (; i < mapping.Count; i++)
            {
                DynamicProperty prop = mapping[i];
                prop.Value = prop.DefaultValue;
            }
        }
    }

    /// <summary>Parses the specified CSV-<see cref="string"/> to an array of a specified 
    /// <see cref="Type"/>.</summary>
    /// <typeparam name="TResult"> Generic type parameter that specifies the <see cref="Type"/>
    /// of the items in the array that the method returns.</typeparam>
    /// <param name="csv">The CSV-<see cref="string"/> to parse.</param>
    /// <param name="converter">
    /// <para>
    /// A function that converts the content of <paramref name="mapping"/>
    /// to an instance of <typeparamref name="TResult"/>. 
    /// </para>
    /// <para>
    /// The function is called for each row in 
    /// <paramref name="csv"/> and gets the <see cref="Mapping"/> as argument, filled with the 
    /// current <see cref="CsvRecord"/> instance. The <see cref="Mapping"/> is passed to the 
    /// function as a <c>dynamic</c> argument: Inside the function the registered 
    /// <see cref="DynamicProperty"/> instances can be used like 
    /// regular .NET properties, but without IntelliSense ("late binding").
    /// </para>
    /// </param>
    /// <param name="isHeaderPresent"> <c>true</c>, to interpret the first line as a header, 
    /// otherwise <c>false</c>.</param>
    /// <param name="options">Parsing options.</param>
    /// <param name="delimiter">The field separator character used in <paramref name="csv"/>.</param>
    /// 
    /// <returns>An array of <typeparamref name="TResult"/> instances, initialized from the parsed 
    /// <paramref name="csv"/>.</returns>
    /// 
    /// <remarks>
    /// <note type="tip">
    /// The optimal parameters can be determined automatically with <see cref="Csv.AnalyzeString(string, Header, int)"/> - 
    /// or use <see cref="ParseAnalyzed{TResult}(Mapping, string, Func{dynamic, TResult}, Header, int, bool)"/>.
    /// </note>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="csv" /> is <c>null</c>.</exception>
    /// <exception cref="CsvFormatException">Invalid CSV. The interpretation depends
    /// on <paramref name="options"/>.</exception>
    /// <exception cref="FormatException">
    /// Parsing fails and <see cref="TypeConverter{T}.Throwing"/> is <c>true</c>.
    /// </exception>
    public static TResult[] Parse<TResult>(this Mapping mapping,
                                           string csv,
                                           Func<dynamic, TResult> converter,
                                           bool isHeaderPresent = true,
                                           CsvOpts options = CsvOpts.Default,
                                           char delimiter = ',')
    {
        _ArgumentNullException.ThrowIfNull(mapping, nameof(mapping));
        _ArgumentNullException.ThrowIfNull(converter, nameof(converter));
        _ArgumentNullException.ThrowIfNull(csv, nameof(csv));

        options = DetermineDisableCaching<TResult>() 
            ? options | CsvOpts.DisableCaching 
            : options.Unset(CsvOpts.DisableCaching);

        using var stringReader = new StringReader(csv);
        using var csvReader = new CsvReader(stringReader, isHeaderPresent, options, delimiter);

        return csvReader.Read(mapping, options.HasFlag(CsvOpts.DisableCaching))
                        .Select(m => converter(m))
                        .ToArray();
    }

    /// <summary>Analyzes the specified CSV-<see cref="string"/>
    /// first and then parses its content to an array of a specified 
    /// <see cref="Type"/>.
    /// </summary>
    ///  <typeparam name="TResult"> Generic type parameter that specifies the <see cref="Type"/>
    /// of the items in the array that the method returns.</typeparam>
    /// <param name="csv">The CSV-<see cref="string"/> to parse.</param>
    /// <param name="converter">
    /// <para>
    /// A function that converts the content of <paramref name="mapping"/>
    /// to an instance of <typeparamref name="TResult"/>. 
    /// </para>
    /// <para>
    /// The function is called for each row in 
    /// <paramref name="csv"/> and gets the <see cref="Mapping"/> as argument, filled with the 
    /// current <see cref="CsvRecord"/> instance. The <see cref="Mapping"/> is passed to the 
    /// function as a <c>dynamic</c> argument: Inside the function the registered 
    /// <see cref="DynamicProperty"/> instances can be used like 
    /// regular .NET properties, but without IntelliSense ("late binding").
    /// </para>
    /// </param>
    /// <param name="header">A supposition that is made about the presence of a header row.</param>
    /// 
    /// <param name="analyzedLines">Maximum number of lines to analyze in <paramref name="csv"/>. The minimum 
    /// value is <see cref="CsvAnalyzer.AnalyzedLinesMinCount" />. If <paramref name="csv"/> has fewer lines than 
    /// <paramref name="analyzedLines" />, it will be analyzed completely. (You can specify 
    /// <see cref="int.MaxValue">Int32.MaxValue</see> to analyze the entire <see cref="string"/> in any case.)</param>
    /// 
    /// <returns>An array of <typeparamref name="TResult"/> instances, initialized from the parsed 
    /// <paramref name="csv"/>.</returns>
    /// 
    /// <remarks>
    /// <para>
    /// <see cref="CsvAnalyzer" /> performs a statistical analysis on <paramref name="csv"/>. The result 
    /// of the analysis is therefore always only an estimate, 
    /// the accuracy of which increases with the number of lines analyzed.
    /// </para>
    /// <para>
    /// The field delimiters COMMA (<c>','</c>, %x2C), SEMICOLON  (<c>';'</c>, %x3B), HASH (<c>'#'</c>, %x23),
    /// TAB (<c>'\t'</c>, %x09), and SPACE (<c>' '</c>, %x20) are recognized automatically.
    /// </para>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="csv" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <para><paramref name="header"/> is not a defined value of 
    /// the <see cref="Header"/> enum.</para>
    /// <para> - or -</para>
    /// <para><paramref name="header"/> is a combination of <see cref="Header"/> values.</para>
    /// </exception>
    /// <exception cref="CsvFormatException">Invalid CSV file. Try to increase the value of 
    /// <paramref name="analyzedLines"/> to get a better analyzer result!</exception>
    /// <exception cref="FormatException">
    /// Parsing fails and <see cref="TypeConverter{T}.Throwing"/> is <c>true</c>.
    /// </exception>
    public static TResult[] ParseAnalyzed<TResult>(this Mapping mapping,
                                                   string csv,
                                                   Func<dynamic, TResult> converter,
                                                   Header header = Header.ProbablyPresent,
                                                   int analyzedLines = CsvAnalyzer.AnalyzedLinesMinCount)
    {
        _ArgumentNullException.ThrowIfNull(mapping, nameof(mapping));
        _ArgumentNullException.ThrowIfNull(converter, nameof(converter));

        CsvAnalyzerResult analyzerResult = CsvAnalyzer.AnalyzeString(csv, header, analyzedLines);
        bool disableCaching = DetermineDisableCaching<TResult>();
        CsvOpts options = disableCaching
            ? analyzerResult.Options | CsvOpts.DisableCaching
            : analyzerResult.Options.Unset(CsvOpts.DisableCaching);

        using var stringReader = new StringReader(csv);
        using var csvReader =
            new CsvReader(stringReader,
                          analyzerResult.IsHeaderPresent,
                          options,
                          analyzerResult.Delimiter);

        return csvReader.Read(mapping, disableCaching)
                        .Select(m => converter(m))
                        .ToArray();
    }
    
    public static TResult[] Read<TResult>(this Mapping mapping,
                                          string filePath,
                                          Func<dynamic, TResult> converter,
                                          bool isHeaderPresent = true,
                                          CsvOpts options = CsvOpts.Default,
                                          char delimiter = ',',
                                          Encoding? textEncoding = null)
    {
        _ArgumentNullException.ThrowIfNull(mapping, nameof(mapping));
        _ArgumentNullException.ThrowIfNull(converter, nameof(converter));
        _ArgumentNullException.ThrowIfNull(filePath, nameof(filePath));

        bool disableCaching = DetermineDisableCaching<TResult>();
        options = disableCaching
            ? options | CsvOpts.DisableCaching
            : options.Unset(CsvOpts.DisableCaching);

        using var csvReader = 
            new CsvReader(filePath, isHeaderPresent, options, delimiter, textEncoding);

        return csvReader.Read(mapping, disableCaching)
                        .Select(m => converter(m))
                        .ToArray();
    }

    public static TResult[] ReadAnalyzed<TResult>(this Mapping mapping,
                                                  string filePath,
                                                  Func<dynamic, TResult> converter,
                                                  Header header = Header.ProbablyPresent,
                                                  Encoding? textEncoding = null,
                                                  int analyzedLines = CsvAnalyzer.AnalyzedLinesMinCount)
    {
        _ArgumentNullException.ThrowIfNull(mapping, nameof(mapping));
        _ArgumentNullException.ThrowIfNull(converter, nameof(converter));

        (CsvAnalyzerResult analyzerResult, Encoding enc) = Csv.AnalyzeFile(filePath, header, textEncoding, analyzedLines);
        bool disableCaching = DetermineDisableCaching<TResult>();
        CsvOpts options = disableCaching
            ? analyzerResult.Options | CsvOpts.DisableCaching
            : analyzerResult.Options.Unset(CsvOpts.DisableCaching);

        using CsvReader csvReader = Csv.OpenRead(filePath,
                                                 analyzerResult.IsHeaderPresent,
                                                 options,
                                                 analyzerResult.Delimiter,
                                                 textEncoding);

        return csvReader.Read(mapping, disableCaching)
                        .Select(m => converter(m))
                        .ToArray();
    }

    private static bool DetermineDisableCaching<TResult>()
    {
        string resultType = typeof(TResult).Name;
        StringComparer comp = StringComparer.Ordinal;
        return !(comp.Equals(resultType, nameof(Mapping)) || comp.Equals(resultType, nameof(CsvRecord)));
    }
}

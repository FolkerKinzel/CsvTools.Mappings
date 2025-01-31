using FolkerKinzel.CsvTools.Mappings.Converters;
using FolkerKinzel.CsvTools.Mappings.Intls;
using FolkerKinzel.CsvTools.Mappings.Intls.Extensions;
using System.Data;
using System.Text;

namespace FolkerKinzel.CsvTools.Mappings;

/// <summary>Static class that provides methods for reading and writing CSV files with
/// <see cref="Mapping"/>s and type conversions.</summary>
public static class CsvMapping
{
    /// <summary>
    /// Initializes a new <see cref="CsvWriter{TData}" /> instance.
    /// </summary>
    /// <typeparam name="TData">
    /// Generic type parameter for the data type that the newly initialized <see cref="CsvWriter{TData}"/> 
    /// can write as CSV row.
    /// </typeparam>
    /// <param name="writer">The <see cref="CsvWriter" /> used for writing.</param>
    /// <param name="mapping">The <see cref="Mapping"/> used to convert a
    /// <typeparamref name="TData"/> instance to a CSV row.</param>
    /// <param name="conversion">
    /// <para>
    /// A method that fills the content of a <typeparamref name="TData"/> instance
    /// into the properties of <paramref name="mapping"/>. 
    /// </para>
    /// <para>
    /// <paramref name="conversion"/> is called with each CSV row to be written and it
    /// gets the <typeparamref name="TData"/> instance and <paramref name="mapping"/> as
    /// arguments. The <see cref="Mapping"/>
    /// is passed to the method as <c>dynamic</c> argument: Inside the method the registered 
    /// <see cref="DynamicProperty"/> instances can be used like 
    /// regular .NET properties, but without IntelliSense ("late binding").
    /// </para>
    /// <para>
    /// With each call all <see cref="DynamicProperty"/> instances in
    /// <paramref name="mapping"/> have been reset to their <see cref="DynamicProperty.DefaultValue"/>.
    /// </para>
    /// </param>
    /// <returns>A <see cref="CsvWriter{TData}"/> instance that allows you to write <typeparamref name="TData"/>
    /// instances as CSV rows.</returns>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="writer"/>, or <paramref name="mapping"/>, 
    /// or <paramref name="conversion"/> is <c>null</c>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CsvWriter<TData> OpenWrite<TData>(CsvWriter writer, Mapping mapping, Action<TData, dynamic> conversion)
        => new(writer, mapping, conversion);

    /// <summary>Initializes a <see cref="CsvReader{TResult}"/> instance to read data 
    /// that is in the CSV format as a collection of <typeparamref name="TResult"/> instances.</summary>
    /// 
    /// <typeparam name="TResult"> Generic type parameter that specifies the <see cref="Type"/>
    /// of the items that the <see cref="CsvReader{TResult}"/> returns.</typeparam>
    /// 
    /// <param name="reader">The <see cref="TextReader" /> with which the CSV data is
    /// read.</param>
    /// <param name="mapping">The <see cref="Mapping"/> used to convert the CSV data.</param>
    /// <param name="conversion">
    /// <para>
    /// A function that converts the content of <paramref name="mapping"/>
    /// to an instance of <typeparamref name="TResult"/>. 
    /// </para>
    /// <para>
    /// The function is called for each row in 
    /// the CSV data and gets the <see cref="Mapping"/> as argument, filled with the 
    /// current <see cref="CsvRecord"/> instance. The <see cref="Mapping"/> is passed to the 
    /// function as <c>dynamic</c> argument: Inside the function the registered 
    /// <see cref="DynamicProperty"/> instances can be used like 
    /// regular .NET properties, but without IntelliSense ("late binding").
    /// </para>
    /// </param>
    /// <param name="isHeaderPresent"> <c>true</c>, to interpret the first line as a header, 
    /// otherwise <c>false</c>.</param>
    /// <param name="options">Options for reading CSV.</param>
    /// <param name="delimiter">The field separator character.</param>
    /// 
    /// <returns>A <see cref="CsvReader{TResult}"/> that allows you to iterate through the
    /// data parsed from the CSV.</returns>
    /// 
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="reader" />, or <paramref name="mapping"/>,
    /// or <paramref name="conversion"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="delimiter"/> is either 
    /// the double quotes <c>"</c> or a line break character ('\r' or  '\n').</exception>
    public static CsvReader<TResult> OpenRead<TResult>(TextReader reader,
                                                       Mapping mapping,
                                                       Func<dynamic, TResult> conversion,
                                                       bool isHeaderPresent = true,
                                                       CsvOpts options = CsvOpts.Default,
                                                       char delimiter = ',')
    {
        options = DetermineDisableCaching<TResult>(options);

        return new CsvReader<TResult>(new CsvReader(reader,
                                                    isHeaderPresent,
                                                    options,
                                                    delimiter), 
                                        mapping,
                                        conversion);
    }

    /// <summary>Opens the CSV file referenced with <paramref name="filePath"/> for reading its
    /// data with a <see cref="CsvReader{TResult}"/>.</summary>
    /// 
    /// <typeparam name="TResult"> Generic type parameter that specifies the <see cref="Type"/>
    /// of the items that the <see cref="CsvReader{TResult}"/> returns.</typeparam>
    /// 
    /// <param name="filePath">File path of the CSV file to read.</param>
    /// <param name="mapping">The <see cref="Mapping"/> used to convert the CSV data.</param>
    /// <param name="conversion">
    /// <para>
    /// A function that converts the content of <paramref name="mapping"/>
    /// to an instance of <typeparamref name="TResult"/>. 
    /// </para>
    /// <para>
    /// The function is called for each row in 
    /// the CSV file and gets the <see cref="Mapping"/> as argument, filled with the 
    /// current <see cref="CsvRecord"/> instance. The <see cref="Mapping"/> is passed to the 
    /// function as <c>dynamic</c> argument: Inside the function the registered 
    /// <see cref="DynamicProperty"/> instances can be used like 
    /// regular .NET properties, but without IntelliSense ("late binding").
    /// </para>
    /// </param>
    /// <param name="isHeaderPresent"> <c>true</c>, to interpret the first line as a header, 
    /// otherwise <c>false</c>.</param>
    /// <param name="options">Options for reading the CSV file.</param>
    /// <param name="delimiter">The field separator character.</param>
    /// <param name="textEncoding">The text encoding to be used to read the CSV file
    /// or <c>null</c> for <see cref="Encoding.UTF8" />.</param>
    /// 
    /// <returns>A <see cref="CsvReader{TResult}"/> that allows you to iterate through the
    /// data parsed from the CSV.</returns>
    /// 
    /// <remarks>
    /// <note type="tip">
    /// The optimal parameters can be determined automatically with <see cref="Csv.AnalyzeFile(string, Header, Encoding?, int)"/> - or use
    /// <see cref="OpenReadAnalyzed{TResult}(string, Mapping, Func{dynamic, TResult}, Header, Encoding?, int)"/>.
    /// </note>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="filePath" />, or <paramref name="mapping"/>, 
    /// or <paramref name="conversion"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"> <paramref name="filePath" /> is not a valid
    /// file path.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="delimiter"/> is either 
    /// the double quotes <c>"</c> or a line break character ('\r' or  '\n').</exception>
    /// <exception cref="IOException">I/O error.</exception>
    public static CsvReader<TResult> OpenRead<TResult>(string filePath,
                                                       Mapping mapping,
                                                       Func<dynamic, TResult> conversion,
                                                       bool isHeaderPresent = true,
                                                       CsvOpts options = CsvOpts.Default,
                                                       char delimiter = ',',
                                                       Encoding? textEncoding = null)
    {
        options = DetermineDisableCaching<TResult>(options);
        return new CsvReader<TResult>(new CsvReader(filePath, isHeaderPresent, options, delimiter, textEncoding),
                                      mapping,
                                      conversion);
    }

    /// <summary>Analyzes the CSV file referenced with <paramref name="filePath"/>
    /// first and then returns a <see cref="CsvReader{TResult}"/> for reading its
    /// data as a collection of <typeparamref name="TResult"/> instances.
    /// </summary>
    /// 
    ///  <typeparam name="TResult"> Generic type parameter that specifies the <see cref="Type"/>
    /// of the items in the array that the method returns.</typeparam>
    /// 
    /// <param name="filePath">File path of the CSV file to read.</param>
    /// <param name="mapping">The <see cref="Mapping"/> used to convert the CSV data.</param>
    /// <param name="conversion">
    /// <para>
    /// A function that converts the content of <paramref name="mapping"/>
    /// to an instance of <typeparamref name="TResult"/>. 
    /// </para>
    /// <para>
    /// The function is called for each row in 
    /// the CSV file and gets the <see cref="Mapping"/> as argument, filled with the 
    /// current <see cref="CsvRecord"/> instance. The <see cref="Mapping"/> is passed to the 
    /// function as <c>dynamic</c> argument: Inside the function the registered 
    /// <see cref="DynamicProperty"/> instances can be used like 
    /// regular .NET properties, but without IntelliSense ("late binding").
    /// </para>
    /// </param>
    /// <param name="header">A supposition that is made about the presence of a header row.</param>
    /// <param name="textEncoding">The text encoding to be used to read the CSV file,
    /// or <c>null</c> to determine the encoding automatically from the byte order mark (BOM).</param>
    /// <param name="analyzedLines">Maximum number of lines to analyze in the CSV file. The minimum 
    /// value is <see cref="CsvAnalyzer.AnalyzedLinesMinCount" />. If the file has fewer lines than 
    /// <paramref name="analyzedLines" />, it will be analyzed completely. (You can specify 
    /// <see cref="int.MaxValue">Int32.MaxValue</see> to analyze the entire file in any case.)</param>
    /// 
    /// <returns>A <see cref="CsvReader{TResult}"/> that allows you to iterate through the
    /// data parsed from the CSV file.</returns>
    /// 
    /// <remarks>
    /// <para>
    /// <see cref="CsvAnalyzer" /> performs a statistical analysis on the CSV file. The result 
    /// of the analysis is therefore always only an estimate, 
    /// the accuracy of which increases with the number of lines analyzed.
    /// </para>
    /// <para>
    /// The field delimiters COMMA (<c>','</c>, %x2C), SEMICOLON  (<c>';'</c>, %x3B), HASH (<c>'#'</c>, %x23),
    /// TAB (<c>'\t'</c>, %x09), and SPACE (<c>' '</c>, %x20) are recognized automatically.
    /// </para>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="filePath"/>, or <paramref name="mapping"/>, 
    /// or <paramref name="conversion"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <para><paramref name="header"/> is not a defined value of 
    /// the <see cref="Header"/> enum.</para>
    /// <para> - or -</para>
    /// <para><paramref name="header"/> is a combination of <see cref="Header"/> values.</para>
    /// </exception>
    /// <exception cref="CsvFormatException">Invalid CSV file. Try to increase the value of 
    /// <paramref name="analyzedLines"/> to get a better analyzer result!</exception>
    public static CsvReader<TResult> OpenReadAnalyzed<TResult>(string filePath,
                                                               Mapping mapping,
                                                               Func<dynamic, TResult> conversion,
                                                               Header header = Header.ProbablyPresent,
                                                               Encoding? textEncoding = null,
                                                               int analyzedLines = CsvAnalyzer.AnalyzedLinesMinCount)
    {
        (CsvAnalyzerResult analyzerResult, Encoding enc) = CsvTools.Csv.AnalyzeFile(filePath, header, textEncoding, analyzedLines);
        CsvOpts options = DetermineDisableCaching<TResult>(analyzerResult.Options);

        return new CsvReader<TResult>(new CsvReader(filePath,
                                                    analyzerResult.IsHeaderPresent,
                                                    options,
                                                    analyzerResult.Delimiter,
                                                    textEncoding),
                                      mapping,
                                      conversion);
    }

    /// <summary>Parses the specified CSV-<see cref="string"/> to an array of a specified 
    /// <see cref="Type"/>.</summary>
    /// 
    /// <typeparam name="TResult"> Generic type parameter that specifies the <see cref="Type"/>
    /// of the items in the array that the method returns.</typeparam>
    /// 
    /// <param name="csv">The CSV-<see cref="string"/> to parse.</param>
    /// <param name="mapping">The <see cref="Mapping"/> used to convert the CSV data.</param>
    /// <param name="conversion">
    /// <para>
    /// A function that converts the content of <paramref name="mapping"/>
    /// to an instance of <typeparamref name="TResult"/>. 
    /// </para>
    /// <para>
    /// The function is called for each row in 
    /// <paramref name="csv"/> and gets the <see cref="Mapping"/> as argument, filled with the 
    /// current <see cref="CsvRecord"/> instance. The <see cref="Mapping"/> is passed to the 
    /// function as <c>dynamic</c> argument: Inside the function the registered 
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
    /// or use <see cref="ParseAnalyzed{TResult}(string, Mapping, Func{dynamic, TResult}, Header, int)"/>.
    /// </note>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="csv" /> is <c>null</c>.</exception>
    /// <exception cref="CsvFormatException">Invalid CSV. The interpretation depends
    /// on <paramref name="options"/>.</exception>
    /// <exception cref="FormatException">
    /// Parsing fails and <see cref="TypeConverter{T}.Throwing"/> is <c>true</c>.
    /// </exception>
    public static TResult[] Parse<TResult>(string csv,
                                           Mapping mapping,
                                           Func<dynamic, TResult> conversion,
                                           bool isHeaderPresent = true,
                                           CsvOpts options = CsvOpts.Default,
                                           char delimiter = ',')
    {
        _ArgumentNullException.ThrowIfNull(csv, nameof(csv));

        options = DetermineDisableCaching<TResult>(options);

        using var stringReader = new StringReader(csv);
        using var csvReader = new CsvReader(stringReader, isHeaderPresent, options, delimiter);
        using var typedReader = new CsvReader<TResult>(csvReader, mapping, conversion);
        return [.. typedReader];
    }

    /// <summary>Analyzes the specified CSV-<see cref="string"/>
    /// first and then parses its content to an array of a specified 
    /// <see cref="Type"/>.
    /// </summary>
    ///  <typeparam name="TResult"> Generic type parameter that specifies the <see cref="Type"/>
    /// of the items in the array that the method returns.</typeparam>
    /// <param name="csv">The CSV-<see cref="string"/> to parse.</param>
    /// <param name="mapping">The <see cref="Mapping"/> used to convert the CSV data.</param>
    /// <param name="conversion">
    /// <para>
    /// A function that converts the content of <paramref name="mapping"/>
    /// to an instance of <typeparamref name="TResult"/>. 
    /// </para>
    /// <para>
    /// The function is called for each row in 
    /// <paramref name="csv"/> and gets the <see cref="Mapping"/> as argument, filled with the 
    /// current <see cref="CsvRecord"/> instance. The <see cref="Mapping"/> is passed to the 
    /// function as <c>dynamic</c> argument: Inside the function the registered 
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
    public static TResult[] ParseAnalyzed<TResult>(string csv,
                                                   Mapping mapping,
                                                   Func<dynamic, TResult> conversion,
                                                   Header header = Header.ProbablyPresent,
                                                   int analyzedLines = CsvAnalyzer.AnalyzedLinesMinCount)
    {
        CsvAnalyzerResult analyzerResult = CsvAnalyzer.AnalyzeString(csv, header, analyzedLines);
        CsvOpts options = DetermineDisableCaching<TResult>(analyzerResult.Options);

        using var stringReader = new StringReader(csv);
        using var csvReader =
            new CsvReader(stringReader,
                          analyzerResult.IsHeaderPresent,
                          options,
                          analyzerResult.Delimiter);
        using var typeReader = new CsvReader<TResult>(csvReader, mapping, conversion);

        return [.. typeReader];
    }

    private static CsvOpts DetermineDisableCaching<TResult>(CsvOpts options)
    {
        string resultType = typeof(TResult).Name;
        StringComparer comp = StringComparer.Ordinal;
        return !options.HasFlag(CsvOpts.DisableCaching) 
               && !(comp.Equals(resultType, nameof(Mapping)) || comp.Equals(resultType, nameof(CsvRecord)))
                        ? options | CsvOpts.DisableCaching
                        : options;
    }
}
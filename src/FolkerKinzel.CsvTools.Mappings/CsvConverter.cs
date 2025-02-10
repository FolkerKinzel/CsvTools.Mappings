using FolkerKinzel.CsvTools.Mappings.TypeConverters;
using FolkerKinzel.CsvTools.Mappings.Intls;
using System.Text;
using System.Data;
using FolkerKinzel.CsvTools.Mappings.Intls.Extensions;
using FolkerKinzel.CsvTools.Mappings.TypeConverters.Interfaces;

namespace FolkerKinzel.CsvTools.Mappings;

/// <summary>Static class that provides methods for CSV serialization of collections
/// of any data type.</summary>
public static class CsvConverter
{
    private static readonly Type _mappingType = typeof(CsvMapping);
    private static readonly Type _recordType = typeof(CsvRecord);
    private static readonly Type _dynamicType = typeof(object);

    /// <summary>
    /// Writes the content of a collection of <typeparamref name="TData"/> instances as CSV.
    /// </summary>
    /// <typeparam name="TData">
    /// Generic type parameter for the data type to write as CSV row.
    /// </typeparam>
    /// <param name="data">The data to write as CSV. Each item will be represented with 
    /// a CSV row.</param>
    /// <param name="writer">The <see cref="CsvWriter" /> used for writing.</param>
    /// <param name="mapping">The <see cref="CsvMapping"/> used to convert a
    /// <typeparamref name="TData"/> instance to a CSV row.</param>
    /// <param name="conversion">
    /// <para>
    /// A method that fills the content of a <typeparamref name="TData"/> instance
    /// into the properties of <paramref name="mapping"/>. 
    /// </para>
    /// <para>
    /// <paramref name="conversion"/> is called with each CSV row to be written and it
    /// gets the <typeparamref name="TData"/> instance and <paramref name="mapping"/> as
    /// arguments. <paramref name="mapping"/>
    /// is passed to the method as <c>dynamic</c> argument: Inside the <paramref name="conversion"/>
    /// method the registered 
    /// <see cref="DynamicProperty"/> instances can be used like 
    /// regular .NET properties, but without IntelliSense ("late binding").
    /// </para>
    /// <para>
    /// With each call of <paramref name="conversion"/> all <see cref="DynamicProperty"/> instances in
    /// <paramref name="mapping"/> are reset to their <see cref="DynamicProperty.DefaultValue"/>.
    /// </para>
    /// </param>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="data"/>, or <paramref name="writer"/>, or 
    /// <paramref name="mapping"/>, or <paramref name="conversion"/> is <c>null</c>.</exception>
    /// <exception cref="IOException">I/O error.</exception>
    /// <exception cref="ObjectDisposedException">The file was already closed.</exception>
    public static void Write<TData>(IEnumerable<TData?> data,
                                    CsvWriter writer,
                                    CsvMapping mapping,
                                    Action<TData, dynamic> conversion)
    {
        _ArgumentNullException.ThrowIfNull(data, nameof(data));

        using var csvWriter = new CsvWriter<TData>(writer, mapping, conversion);

        foreach (TData? item in data)
        {
            csvWriter.Write(item);
        }
    }

    /// <summary>
    /// Writes the content of a <see cref="DataTable"/> as CSV.
    /// </summary>
    /// <param name="dataTable">The <see cref="DataTable"/> whose content is written.</param>
    /// <param name="writer">The <see cref="CsvWriter"/> to be used.</param>
    /// <param name="mapping">The <see cref="CsvMapping"/> to be used.</param>
    /// 
    /// <remarks>
    /// <para>
    /// Each <see cref="DynamicProperty.PropertyName"/> of <paramref name="mapping"/>
    /// MUST have a corresponding <see cref="DataColumn"/> in <paramref name="dataTable"/>
    /// - corresponding in the <see cref="DataColumn.Caption"/> property (case-insensitive)
    /// and the accepted data type.
    /// </para>
    /// <para>
    /// Effort must be taken that the 
    /// <see cref="DynamicProperty.PropertyName"/>s in <paramref name="mapping"/> are unique, 
    /// even when treated case-insensitive.
    /// </para>
    /// <para>
    /// The <see cref="DynamicProperty"/> instances in <paramref name="mapping"/> don't need to match 
    /// all columns of the <see cref="DataTable"/> or all columns of the CSV file (neither in
    /// number nor in order).
    /// </para>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="dataTable"/>, or <paramref name="writer"/>,
    /// or <paramref name="mapping"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// There is a <see cref="DynamicProperty"/> in <paramref name="mapping"/> whose 
    /// <see cref="DynamicProperty.PropertyName"/> finds no corresponding <see cref="DataColumn.ColumnName"/>
    /// in <paramref name="dataTable"/>.
    /// </exception>
    /// <exception cref="InvalidCastException">
    /// A value in <paramref name="dataTable"/> does not match the expected data type in 
    /// <paramref name="mapping"/>.
    /// </exception>
    /// <exception cref="FormatException">
    /// One of the <see cref="TypeConverter{T}"/> instances uses an invalid format string.
    /// </exception>
    /// <exception cref="IOException">I/O error.</exception>
    /// <exception cref="ObjectDisposedException">The file was already closed.</exception>
    public static void Write(DataTable dataTable, CsvWriter writer, CsvMapping mapping)
    {
        _ArgumentNullException.ThrowIfNull(dataTable, nameof(dataTable));
        _ArgumentNullException.ThrowIfNull(writer, nameof(writer));
        _ArgumentNullException.ThrowIfNull(mapping, nameof(mapping));

        mapping.Record = writer.Record;

        Dictionary<string, string> captionDictionary = DataTableHelper.CreateCaptionDictionary(dataTable);

        DataRowCollection rows = dataTable.Rows;

        for (int i = 0; i < rows.Count; i++)
        {
            mapping.FillWith(rows[i], captionDictionary);
            writer.WriteRecord();
        }
    }

    /// <summary>
    /// Saves a collection of <typeparamref name="TData"/> instances as a CSV file
    /// with header row.
    /// </summary>
    /// <typeparam name="TData">
    /// Generic type parameter for the data type to write as CSV row.
    /// </typeparam>
    /// <param name="data">The data to save as CSV file. Each item will be represented with 
    /// a CSV row.
    /// </param>
    /// <param name="filePath">File path of the CSV file.</param>
    /// <param name="columnNames">
    /// <para>
    /// A collection of column names for the header to be written.
    /// </para>
    /// <para>
    /// The collection determines the order in which the columns appear in the CSV file.
    /// </para>
    /// <para>
    /// The collection will be copied. If the collection contains <c>null</c> values, empty strings, or 
    /// white space, these are replaced by automatically generated column names. Column names cannot 
    /// appear twice. By default the comparison is case-sensitive but it will be reset to a case-insensitive
    /// comparison if the column names are also unique when treated case-insensitive.
    /// </para>
    /// </param>
    /// <param name="mapping">The <see cref="CsvMapping"/> used to convert a
    /// <typeparamref name="TData"/> instance to a CSV row.</param>
    /// <param name="conversion">
    /// <para>
    /// A method that fills the content of a <typeparamref name="TData"/> instance
    /// into the properties of <paramref name="mapping"/>. 
    /// </para>
    /// <para>
    /// <paramref name="conversion"/> is called with each CSV row to be written and it
    /// gets the <typeparamref name="TData"/> instance and <paramref name="mapping"/> as
    /// arguments. <paramref name="mapping"/>
    /// is passed to the method as <c>dynamic</c> argument: Inside the <paramref name="conversion"/>
    /// method the registered 
    /// <see cref="DynamicProperty"/> instances can be used like 
    /// regular .NET properties, but without IntelliSense ("late binding").
    /// </para>
    /// <para>
    /// With each call of <paramref name="conversion"/> all <see cref="DynamicProperty"/> instances in
    /// <paramref name="mapping"/> are reset to their <see cref="DynamicProperty.DefaultValue"/>.
    /// </para>
    /// </param>
    /// 
    /// <remarks>
    /// <para>Creates a new CSV file. If the target file already exists, it is truncated and overwritten.
    /// </para>
    /// <para>
    /// This method creates a CSV file that uses the comma ',' (%x2C) as field delimiter.
    /// This complies with the RFC 4180 standard. If another delimiter is required, use the 
    /// <see cref="Write"/> method instead.
    /// </para>
    /// </remarks>
    /// 
    /// <example>
    /// <note type="note">In the following code examples - for easier readability - exception handling 
    /// has been omitted.</note>
    /// <para>Object serialization with CSV:</para>
    /// <code language="cs" source="..\Examples\ObjectSerializationExample.cs"/>
    /// </example>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="filePath"/>, or <paramref name="data"/>, or 
    /// <paramref name="columnNames"/>, or <paramref name="mapping"/>, or <paramref name="conversion"/> 
    /// is <c>null</c>.</exception>
    /// <exception cref="IOException">I/O error.</exception>
    /// <exception cref="ObjectDisposedException">The file was already closed.</exception>
    public static void Save<TData>(IEnumerable<TData?> data,
                                   string filePath,
                                   IReadOnlyCollection<string?> columnNames,
                                   CsvMapping mapping,
                                   Action<TData, dynamic> conversion)
    {
        using CsvWriter csvWriter = Csv.OpenWrite(filePath, columnNames);
        Write(data, csvWriter, mapping, conversion);
    }
    
    /// <summary>
    /// Saves a collection of <typeparamref name="TData"/> instances as a CSV file
    /// without a header row.
    /// </summary>
    /// <typeparam name="TData">
    /// Generic type parameter for the data type to write as CSV row.
    /// </typeparam>
    /// <param name="data">The data to save as CSV file. Each item will be represented with 
    /// a CSV row.</param>
    /// <param name="filePath">File path of the CSV file.</param>
    /// <param name="columnsCount">Number of columns in the CSV file.</param>
    /// <param name="mapping">The <see cref="CsvMapping"/> used to convert a
    /// <typeparamref name="TData"/> instance to a CSV row.</param>
    /// <param name="conversion">
    /// <para>
    /// A method that fills the content of a <typeparamref name="TData"/> instance
    /// into the properties of <paramref name="mapping"/>. 
    /// </para>
    /// <para>
    /// <paramref name="conversion"/> is called with each CSV row to be written and it
    /// gets the <typeparamref name="TData"/> instance and <paramref name="mapping"/> as
    /// arguments. <paramref name="mapping"/>
    /// is passed to the method as <c>dynamic</c> argument: Inside the <paramref name="conversion"/>
    /// method the registered 
    /// <see cref="DynamicProperty"/> instances can be used like 
    /// regular .NET properties, but without IntelliSense ("late binding").
    /// </para>
    /// <para>
    /// With each call of <paramref name="conversion"/> all <see cref="DynamicProperty"/> instances in
    /// <paramref name="mapping"/> are reset to their <see cref="DynamicProperty.DefaultValue"/>.
    /// </para>
    /// </param>
    /// 
    /// <remarks>
    /// <para>Creates a new CSV file. If the target file already exists, it is 
    /// truncated and overwritten.
    /// </para>
    /// <para>
    /// This method creates a CSV file that uses the comma ',' (%x2C) as field delimiter.
    /// This complies with the RFC 4180 standard. If another delimiter is required, use the 
    /// <see cref="Write"/> method instead.
    /// </para>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="filePath"/>, or <paramref name="data"/>, or 
    /// <paramref name="mapping"/>, or <paramref name="conversion"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"> <paramref name="filePath" /> is not a valid
    /// file path.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="columnsCount"/> is negative.</exception>
    /// <exception cref="IOException">I/O error.</exception>
    /// <exception cref="ObjectDisposedException">The file was already closed.</exception>
    public static void Save<TData>(IEnumerable<TData?> data,
                                   string filePath,
                                   int columnsCount,
                                   CsvMapping mapping,
                                   Action<TData, dynamic> conversion)
    {
        using CsvWriter csvWriter = Csv.OpenWrite(filePath, columnsCount);
        Write(data, csvWriter, mapping, conversion);
    }

    /// <summary>
    /// Saves the content of a <see cref="DataTable"/> as a CSV file with header.
    /// </summary>
    /// <param name="dataTable">The <see cref="DataTable"/> whose content is written.</param>
    /// <param name="filePath">File path of the CSV file.</param>
    /// <param name="columnNames">
    /// <para>
    /// A collection of column names for the header to be written.
    /// </para>
    /// <para>
    /// The collection determines the order in which the columns appear in the CSV file.
    /// </para>
    /// <para>
    /// The collection will be copied. If the collection contains <c>null</c> values, empty strings or white space, these 
    /// are replaced by automatically generated column names. Column names cannot appear twice. By default the 
    /// comparison is case-sensitive but it will be reset to a case-insensitive comparison if the column names are 
    /// also unique when treated case-insensitive.
    /// </para>
    /// </param>
    /// <param name="mapping">The <see cref="CsvMapping"/> to be used.</param>
    /// 
    /// <remarks>
    /// <para>Creates a new CSV file. If the target file already exists, it is 
    /// truncated and overwritten.
    /// </para>
    /// <para>
    /// This method initializes a <see cref="CsvWriter"/> instance that uses the comma ',' (%x2C) as field delimiter.
    /// This complies with the RFC 4180 standard. If another delimiter is required, use 
    /// <see cref="Write(DataTable, CsvWriter, CsvMapping)"/> instead.
    /// </para>
    /// <para>
    /// Each <see cref="DynamicProperty.PropertyName"/> of <paramref name="mapping"/>
    /// MUST have a corresponding <see cref="DataColumn"/> in <paramref name="dataTable"/>
    /// - corresponding in the <see cref="DataColumn.Caption"/> property (case-insensitive)
    /// and the accepted data type.
    /// </para>
    /// <para>
    /// Effort must be taken that the 
    /// <see cref="DynamicProperty.PropertyName"/>s in <paramref name="mapping"/> are unique, 
    /// even when treated case-insensitive.
    /// </para>
    /// <para>
    /// The <see cref="DynamicProperty"/> instances in <paramref name="mapping"/> don't need to match 
    /// all columns of the <see cref="DataTable"/> or all columns of the CSV file (neither in
    /// number nor in order).
    /// </para>
    /// </remarks>
    /// 
    /// <example>
    /// <note type="note">In the following code examples - for easier readability - exception handling 
    /// has been omitted.</note>
    /// <para>DataTable serialization with CSV:</para>
    /// <code language="cs" source="..\Examples\DataTableExample.cs"/>
    /// </example>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="dataTable"/>, or <paramref name="filePath"/>,
    /// or <paramref name="columnNames"/>, or <paramref name="mapping"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// <para><paramref name="filePath" /> is not a valid file path.</para>
    /// <para>- or -</para>
    /// <para>
    /// There is a <see cref="DynamicProperty"/> in <paramref name="mapping"/> whose 
    /// <see cref="DynamicProperty.PropertyName"/> finds no corresponding <see cref="DataColumn.ColumnName"/>
    /// in <paramref name="dataTable"/>.
    /// </para>
    /// </exception>
    /// <exception cref="InvalidCastException">
    /// A value in <paramref name="dataTable"/> does not match the expected data type in 
    /// <paramref name="mapping"/>.
    /// </exception>
    /// <exception cref="FormatException">
    /// One of the <see cref="TypeConverter{T}"/> instances uses an invalid format string.
    /// </exception>
    /// <exception cref="IOException">I/O error.</exception>
    /// <exception cref="ObjectDisposedException">The file was already closed.</exception>
    public static void Save(DataTable dataTable,
                            string filePath,
                            IReadOnlyCollection<string?> columnNames,
                            CsvMapping mapping)
    {
        using CsvWriter writer = Csv.OpenWrite(filePath, columnNames);
        Write(dataTable, writer, mapping);
    }

    /// <summary>
    /// Converts a collection of <typeparamref name="TData"/> instances to a CSV 
    /// <see cref="string"/> with header row.
    /// </summary>
    /// <typeparam name="TData">
    /// Generic type parameter for the data type to write as CSV row.
    /// </typeparam>
    /// <param name="data">The data to convert to CSV. Each item will be represented with 
    /// a CSV row.
    /// </param>
    /// <param name="columnNames">
    /// <para>
    /// A collection of column names for the header to be written.
    /// </para>
    /// <para>
    /// The collection determines the order in which the columns appear in the CSV <see cref="string"/>.
    /// </para>
    /// <para>
    /// The collection will be copied. If the collection contains <c>null</c> values, empty strings
    /// or white space, these are replaced by automatically generated column names. Column names cannot 
    /// appear twice. By default the comparison is case-sensitive but it will be reset to a 
    /// case-insensitive comparison if the column names are also unique when treated case-insensitive.
    /// </para>
    /// </param>
    /// <param name="mapping">The <see cref="CsvMapping"/> used to convert a
    /// <typeparamref name="TData"/> instance to a CSV row.</param>
    /// <param name="conversion">
    /// <para>
    /// A method that fills the content of a <typeparamref name="TData"/> instance
    /// into the properties of <paramref name="mapping"/>. 
    /// </para>
    /// <para>
    /// <paramref name="conversion"/> is called with each CSV row to be written and it
    /// gets the <typeparamref name="TData"/> instance and <paramref name="mapping"/> as
    /// arguments. <paramref name="mapping"/>
    /// is passed to the method as <c>dynamic</c> argument: Inside the <paramref name="conversion"/>
    /// method the registered 
    /// <see cref="DynamicProperty"/> instances can be used like 
    /// regular .NET properties, but without IntelliSense ("late binding").
    /// </para>
    /// <para>
    /// With each call of <paramref name="conversion"/> all <see cref="DynamicProperty"/> instances in
    /// <paramref name="mapping"/> are reset to their <see cref="DynamicProperty.DefaultValue"/>.
    /// </para>
    /// </param>
    /// 
    /// <returns>A CSV <see cref="string"/> with header row that contains the contents of 
    /// <paramref name="data"/>.</returns>
    /// 
    /// <remarks>
    /// This method creates a CSV <see cref="string"/> that uses the comma ',' (%x2C) as field 
    /// delimiter.
    /// This complies with the RFC 4180 standard. If another delimiter is required, use the 
    /// <see cref="Write"/> method instead.
    /// </remarks>
    /// 
    /// <example>
    /// <note type="note">In the following code examples - for easier readability - exception handling has been omitted.</note>
    /// <para>Object serialization with CSV:</para>
    /// <code language="cs" source="..\Benchmarks\CalculationWriter_Default.cs"/>
    /// </example>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="data"/>, or <paramref name="columnNames"/>, or 
    /// <paramref name="mapping"/>, or <paramref name="conversion"/> is <c>null</c>.</exception>
    /// <exception cref="IOException">I/O error.</exception>
    /// <exception cref="ObjectDisposedException">The file was already closed.</exception>
    public static string ToCsv<TData>(IEnumerable<TData?> data,
                                      IReadOnlyCollection<string?> columnNames,
                                      CsvMapping mapping,
                                      Action<TData, dynamic> conversion)
    {
        using var stringWriter = new StringWriter();
        using CsvWriter csvWriter = Csv.OpenWrite(stringWriter, columnNames);

        Write(data, csvWriter, mapping, conversion);

        return stringWriter.ToString();
    }

    /// <summary>
    /// Converts a collection of <typeparamref name="TData"/> instances to a CSV 
    /// <see cref="string"/> without a header row.
    /// </summary>
    /// <typeparam name="TData">
    /// Generic type parameter for the data type to write as CSV row.
    /// </typeparam>
    /// <param name="data">The data to convert to CSV. Each item will be represented with 
    /// a CSV row.
    /// </param>
    /// <param name="columnsCount">Number of columns in the CSV file.</param>
    /// <param name="mapping">The <see cref="CsvMapping"/> used to convert a
    /// <typeparamref name="TData"/> instance to a CSV row.</param>
    /// <param name="conversion">
    /// <para>
    /// A method that fills the content of a <typeparamref name="TData"/> instance
    /// into the properties of <paramref name="mapping"/>. 
    /// </para>
    /// <para>
    /// <paramref name="conversion"/> is called with each CSV row to be written and it
    /// gets the <typeparamref name="TData"/> instance and <paramref name="mapping"/> as
    /// arguments. <paramref name="mapping"/>
    /// is passed to the method as <c>dynamic</c> argument: Inside the <paramref name="conversion"/>
    /// method the registered 
    /// <see cref="DynamicProperty"/> instances can be used like 
    /// regular .NET properties, but without IntelliSense ("late binding").
    /// </para>
    /// <para>
    /// With each call of <paramref name="conversion"/> all <see cref="DynamicProperty"/> instances in
    /// <paramref name="mapping"/> are reset to their <see cref="DynamicProperty.DefaultValue"/>.
    /// </para>
    /// </param>
    /// 
    /// <returns>A CSV <see cref="string"/> without header row that contains the contents of 
    /// <paramref name="data"/>.</returns>
    /// 
    /// <remarks>
    /// This method creates a CSV <see cref="string"/> that uses the comma ',' (%x2C) as field delimiter.
    /// This complies with the RFC 4180 standard. If another delimiter is required, use the 
    /// <see cref="Write"/> method instead.
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="data"/>, or 
    /// <paramref name="mapping"/>, or <paramref name="conversion"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="columnsCount"/> is negative.</exception>
    /// <exception cref="IOException">I/O error.</exception>
    /// <exception cref="ObjectDisposedException">The file was already closed.</exception>
    public static string ToCsv<TData>(IEnumerable<TData?> data,
                                      int columnsCount,
                                      CsvMapping mapping,
                                      Action<TData, dynamic> conversion)
    {
        using var stringWriter = new StringWriter();
        using CsvWriter csvWriter = Csv.OpenWrite(stringWriter, columnsCount);

        Write(data, csvWriter, mapping, conversion);

        return stringWriter.ToString();
    }

    /// <summary>Initializes a <see cref="CsvReader{TResult}"/> instance to read data 
    /// that is in the CSV format as a collection of <typeparamref name="TResult"/> instances.</summary>
    /// 
    /// <typeparam name="TResult"> Generic type parameter that specifies the <see cref="Type"/>
    /// of the items that the <see cref="CsvReader{TResult}"/> returns.</typeparam>
    /// 
    /// <param name="reader">The <see cref="TextReader" /> with which the CSV data is
    /// read.</param>
    /// <param name="mapping">The <see cref="CsvMapping"/> used to convert the CSV data.</param>
    /// <param name="conversion">
    /// <para>
    /// A function that converts the content of <paramref name="mapping"/>
    /// to an instance of <typeparamref name="TResult"/>. 
    /// </para>
    /// <para>
    /// The function is called for each row in 
    /// the CSV data and gets the <see cref="CsvMapping"/> as argument, filled with the 
    /// current <see cref="CsvRecord"/> instance. The <see cref="CsvMapping"/> is passed to the 
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
                                                       CsvMapping mapping,
                                                       Func<dynamic, TResult> conversion,
                                                       bool isHeaderPresent = true,
                                                       CsvOpts options = CsvOpts.Default,
                                                       char delimiter = ',')
    {
        bool cloneMapping = DetermineDisableCaching<TResult>(ref options);

        return new CsvReader<TResult>(new CsvReader(reader,
                                                    isHeaderPresent,
                                                    options,
                                                    delimiter),
                                        mapping,
                                        conversion, 
                                        cloneMapping);
    }

    /// <summary>Opens the CSV file referenced with <paramref name="filePath"/> for reading its
    /// data with a <see cref="CsvReader{TResult}"/>.</summary>
    /// 
    /// <typeparam name="TResult"> Generic type parameter that specifies the <see cref="Type"/>
    /// of the items that the <see cref="CsvReader{TResult}"/> returns.</typeparam>
    /// 
    /// <param name="filePath">File path of the CSV file to read.</param>
    /// <param name="mapping">The <see cref="CsvMapping"/> used to convert the CSV data.</param>
    /// <param name="conversion">
    /// <para>
    /// A function that converts the content of <paramref name="mapping"/>
    /// to an instance of <typeparamref name="TResult"/>. 
    /// </para>
    /// <para>
    /// The function is called for each row in 
    /// the CSV file and gets the <see cref="CsvMapping"/> as argument, filled with the 
    /// current <see cref="CsvRecord"/> instance. The <see cref="CsvMapping"/> is passed to the 
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
    /// The optimal parameters can be determined automatically with <see cref="Csv.AnalyzeFile(string, Header, Encoding?, int)"/>
    /// - or use
    /// <see cref="OpenReadAnalyzed{TResult}(string, CsvMapping, Func{dynamic, TResult}, Header, Encoding?, int)"/>.
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
                                                       CsvMapping mapping,
                                                       Func<dynamic, TResult> conversion,
                                                       bool isHeaderPresent = true,
                                                       CsvOpts options = CsvOpts.Default,
                                                       char delimiter = ',',
                                                       Encoding? textEncoding = null)
    {
        bool cloneMapping = DetermineDisableCaching<TResult>(ref options);
        return new CsvReader<TResult>(new CsvReader(filePath, isHeaderPresent, options, delimiter, textEncoding),
                                      mapping,
                                      conversion, 
                                      cloneMapping);
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
    /// <param name="mapping">The <see cref="CsvMapping"/> used to convert the CSV data.</param>
    /// <param name="conversion">
    /// <para>
    /// A function that converts the content of <paramref name="mapping"/>
    /// to an instance of <typeparamref name="TResult"/>. 
    /// </para>
    /// <para>
    /// The function is called for each row in 
    /// the CSV file and gets the <see cref="CsvMapping"/> as argument, filled with the 
    /// current <see cref="CsvRecord"/> instance. The <see cref="CsvMapping"/> is passed to the 
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
    /// <example>
    /// <note type="note">In the following code examples - for easier readability - exception handling has been omitted.</note>
    /// <para>Object serialization with CSV:</para>
    /// <code language="cs" source="..\Examples\ObjectSerializationExample.cs"/>
    /// </example>
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
                                                               CsvMapping mapping,
                                                               Func<dynamic, TResult> conversion,
                                                               Header header = Header.ProbablyPresent,
                                                               Encoding? textEncoding = null,
                                                               int analyzedLines = CsvAnalyzer.AnalyzedLinesMinCount)
    {
        (CsvAnalyzerResult analyzerResult, Encoding enc) = CsvTools.Csv.AnalyzeFile(filePath, header, textEncoding, analyzedLines);
        CsvOpts options = analyzerResult.Options;
        bool cloneMapping = DetermineDisableCaching<TResult>(ref options);

        return new CsvReader<TResult>(new CsvReader(filePath,
                                                    analyzerResult.IsHeaderPresent,
                                                    options,
                                                    analyzerResult.Delimiter,
                                                    textEncoding),
                                      mapping,
                                      conversion,
                                      cloneMapping);
    }

    /// <summary>Parses the specified CSV-<see cref="string"/> to an array of a specified 
    /// <see cref="Type"/>.</summary>
    /// 
    /// <typeparam name="TResult"> Generic type parameter that specifies the <see cref="Type"/>
    /// of the items in the array that the method returns.</typeparam>
    /// 
    /// <param name="csv">The CSV-<see cref="string"/> to parse.</param>
    /// <param name="mapping">The <see cref="CsvMapping"/> used to convert the CSV data.</param>
    /// <param name="conversion">
    /// <para>
    /// A function that converts the content of <paramref name="mapping"/>
    /// to an instance of <typeparamref name="TResult"/>. 
    /// </para>
    /// <para>
    /// The function is called for each row in 
    /// <paramref name="csv"/> and gets the <see cref="CsvMapping"/> as argument, filled with the 
    /// current <see cref="CsvRecord"/> instance. The <see cref="CsvMapping"/> is passed to the 
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
    /// or use <see cref="ParseAnalyzed{TResult}(string, CsvMapping, Func{dynamic, TResult}, Header, int)"/>.
    /// </note>
    /// </remarks>
    /// 
    /// <example>
    /// <note type="note">In the following code examples - for easier readability - exception handling has been omitted.</note>
    /// <para>Object serialization with CSV:</para>
    /// <code language="cs" source="..\Benchmarks\CalculationReader_Default.cs"/>
    /// </example>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="csv" /> is <c>null</c>.</exception>
    /// <exception cref="CsvFormatException">Invalid CSV. The interpretation depends
    /// on <paramref name="options"/>.</exception>
    /// <exception cref="FormatException">
    /// Parsing fails and <see cref="TypeConverter{T}.Throwing"/> is <c>true</c>.
    /// </exception>
    public static TResult[] Parse<TResult>(string csv,
                                           CsvMapping mapping,
                                           Func<dynamic, TResult> conversion,
                                           bool isHeaderPresent = true,
                                           CsvOpts options = CsvOpts.Default,
                                           char delimiter = ',')
    {
        _ArgumentNullException.ThrowIfNull(csv, nameof(csv));

        bool cloneMapping = DetermineDisableCaching<TResult>(ref options);

        using var stringReader = new StringReader(csv);
        using var csvReader = new CsvReader(stringReader, isHeaderPresent, options, delimiter);
        using var typedReader = new CsvReader<TResult>(csvReader, mapping, conversion, cloneMapping);
        return [.. typedReader];
    }

    /// <summary>Analyzes the specified CSV-<see cref="string"/>
    /// first and then parses its content to an array of a specified 
    /// <see cref="Type"/>.
    /// </summary>
    ///  <typeparam name="TResult"> Generic type parameter that specifies the <see cref="Type"/>
    /// of the items in the array that the method returns.</typeparam>
    /// <param name="csv">The CSV-<see cref="string"/> to parse.</param>
    /// <param name="mapping">The <see cref="CsvMapping"/> used to convert the CSV data.</param>
    /// <param name="conversion">
    /// <para>
    /// A function that converts the content of <paramref name="mapping"/>
    /// to an instance of <typeparamref name="TResult"/>. 
    /// </para>
    /// <para>
    /// The function is called for each row in 
    /// <paramref name="csv"/> and gets the <see cref="CsvMapping"/> as argument, filled with the 
    /// current <see cref="CsvRecord"/> instance. The <see cref="CsvMapping"/> is passed to the 
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
                                                   CsvMapping mapping,
                                                   Func<dynamic, TResult> conversion,
                                                   Header header = Header.ProbablyPresent,
                                                   int analyzedLines = CsvAnalyzer.AnalyzedLinesMinCount)
    {
        CsvAnalyzerResult analyzerResult = CsvAnalyzer.AnalyzeString(csv, header, analyzedLines);
        CsvOpts options = analyzerResult.Options;
        bool cloneMapping = DetermineDisableCaching<TResult>(ref options);

        using var stringReader = new StringReader(csv);
        using var csvReader =
            new CsvReader(stringReader,
                          analyzerResult.IsHeaderPresent,
                          options,
                          analyzerResult.Delimiter);
        using var typeReader = new CsvReader<TResult>(csvReader, mapping, conversion, cloneMapping);

        return [.. typeReader];
    }

    /// <summary>
    /// Adds CSV content as <see cref="DataRow"/>s to a <see cref="DataTable"/>.
    /// </summary>
    /// <param name="dataTable">The <see cref="DataTable"/> to which <see cref="DataRow"/>s
    /// are added.</param>
    /// <param name="reader">The <see cref="CsvReader"/> to use.</param>
    /// <param name="mapping">The <see cref="CsvMapping"/> to be used.</param>
    /// 
    /// <remarks>
    /// <para>
    /// Each <see cref="DynamicProperty.PropertyName"/> of <paramref name="mapping"/>
    /// MUST have a corresponding <see cref="DataColumn"/> in <paramref name="dataTable"/>
    /// - corresponding in the <see cref="DataColumn.Caption"/> property (case-insensitive)
    /// and the accepted data type.
    /// </para>
    /// <para>
    /// Effort must be taken that the 
    /// <see cref="DynamicProperty.PropertyName"/>s in <paramref name="mapping"/> are unique, 
    /// even when treated case-insensitive.
    /// </para>
    /// <para>
    /// The <see cref="DynamicProperty"/> instances in <paramref name="mapping"/> don't need to match 
    /// all columns of the <see cref="DataTable"/> or all columns of the CSV file (neither in
    /// number nor in order).
    /// </para>
    /// <para>
    /// It's recommended to initialize <paramref name="reader"/> with the <see cref="CsvOpts.DisableCaching"/>
    /// flag set.
    /// </para>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="dataTable"/>, or <paramref name="reader"/>,
    /// or <paramref name="mapping"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// There is a <see cref="DynamicProperty"/> in <paramref name="mapping"/> whose 
    /// <see cref="DynamicProperty.PropertyName"/> finds no corresponding <see cref="DataColumn.ColumnName"/>
    /// in <paramref name="dataTable"/>.
    /// </exception>
    /// <exception cref="FormatException">Parsing fails and <see cref="ITypeConverter{T}.Throwing"/> is <c>true</c>.</exception>
    /// <exception cref="NoNullAllowedException">The <paramref name="mapping"/> doesn't match the schema of
    /// the <paramref name="dataTable"/>.</exception>
    /// <exception cref="ConstraintException">The parsed CSV data does not match the schema of
    /// the <paramref name="dataTable"/>.</exception>
    /// <exception cref="IOException">I/O error.</exception>
    /// <exception cref="ObjectDisposedException">The file was already closed.</exception>
    public static void Fill(DataTable dataTable, CsvReader reader, CsvMapping mapping)
    {
        _ArgumentNullException.ThrowIfNull(dataTable, nameof(dataTable));
        _ArgumentNullException.ThrowIfNull(reader, nameof(reader));
        _ArgumentNullException.ThrowIfNull(mapping, nameof(mapping));

        Dictionary<string, string> captionDictionary = DataTableHelper.CreateCaptionDictionary(dataTable);

        try
        {
            foreach (CsvRecord record in reader)
            {
                mapping.Record = record;
                DataRow dataRow = dataTable.NewRow();

                for (int i = 0; i < mapping.Count; i++)
                {
                    DynamicProperty prop = mapping[i];
                    dataRow[captionDictionary[prop.PropertyName]] = prop.Value;
                }

                dataTable.Rows.Add(dataRow);
            }
        }
        catch (KeyNotFoundException e)
        {
            throw new ArgumentException(e.Message, nameof(mapping), e);
        }
    }

    /// <summary>
    /// Adds the content of a CSV file as <see cref="DataRow"/>s to a <see cref="DataTable"/>.
    /// </summary>
    /// <param name="dataTable">The <see cref="DataTable"/> to which <see cref="DataRow"/>s
    /// are added.</param>
    /// <param name="filePath">File path of the CSV file.</param>
    /// <param name="mapping">The <see cref="CsvMapping"/> to be used.</param>
    /// <param name="isHeaderPresent"> <c>true</c>, to interpret the first line as a header, 
    /// otherwise <c>false</c>.</param>
    /// <param name="options">Options for reading the CSV file.</param>
    /// <param name="delimiter">The field separator character.</param>
    /// <param name="textEncoding">The text encoding to be used to read the CSV file
    /// or <c>null</c> for <see cref="Encoding.UTF8" />.</param>
    /// 
    /// <remarks>
    /// <para>
    /// Each <see cref="DynamicProperty.PropertyName"/> of <paramref name="mapping"/>
    /// MUST have a corresponding <see cref="DataColumn"/> in <paramref name="dataTable"/>
    /// - corresponding in the <see cref="DataColumn.Caption"/> property (case-insensitive)
    /// and the accepted data type.
    /// </para>
    /// <para>
    /// Effort must be taken that the 
    /// <see cref="DynamicProperty.PropertyName"/>s in <paramref name="mapping"/> are unique, 
    /// even when treated case-insensitive.
    /// </para>
    /// <para>
    /// The <see cref="DynamicProperty"/> instances in <paramref name="mapping"/> don't need to match 
    /// all columns of the <see cref="DataTable"/> or all columns of the CSV file (neither in
    /// number nor in order).
    /// </para>
    /// </remarks>
    /// 
    /// <example>
    /// <note type="note">In the following code examples - for easier readability - exception handling 
    /// has been omitted.</note>
    /// <para>DataTable serialization with CSV:</para>
    /// <code language="cs" source="..\Examples\DataTableExample.cs"/>
    /// </example>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="dataTable"/>, or <paramref name="filePath"/>,
    /// or <paramref name="mapping"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <para><paramref name="filePath" /> is not a valid file path.</para>
    /// <para>- or -</para>
    /// <para>
    /// There is a <see cref="DynamicProperty"/> in <paramref name="mapping"/> whose 
    /// <see cref="DynamicProperty.PropertyName"/> finds no corresponding <see cref="DataColumn.ColumnName"/>
    /// in <paramref name="dataTable"/>.
    /// </para>
    /// </exception>
    /// <exception cref="FormatException">Parsing fails and <see cref="ITypeConverter{T}.Throwing"/> is <c>true</c>.</exception>
    /// <exception cref="NoNullAllowedException">The <paramref name="mapping"/> doesn't match the schema of
    /// the <paramref name="dataTable"/>.</exception>
    /// <exception cref="ConstraintException">The parsed CSV data does not match the schema of
    /// the <paramref name="dataTable"/>.</exception>
    /// <exception cref="IOException">I/O error.</exception>
    /// <exception cref="ObjectDisposedException">The file was already closed.</exception>
    public static void Fill(DataTable dataTable,
                            string filePath,
                            CsvMapping mapping,
                            bool isHeaderPresent = true,
                            CsvOpts options = CsvOpts.Default,
                            char delimiter = ',',
                            Encoding? textEncoding = null)
    {
        using var reader = new CsvReader(filePath,
                                         isHeaderPresent,
                                         options | CsvOpts.DisableCaching,
                                         delimiter,
                                         textEncoding);
        Fill(dataTable, reader, mapping);
    }

    /// <summary>
    /// Adds the content of a CSV file as <see cref="DataRow"/>s to a <see cref="DataTable"/>
    /// after the CSV file had been analyzed.
    /// </summary>
    /// <param name="dataTable">The <see cref="DataTable"/> to which <see cref="DataRow"/>s
    /// are added.</param>
    /// <param name="filePath">File path of the CSV file.</param>
    /// <param name="mapping">The <see cref="CsvMapping"/> to be used.</param>
    /// <param name="header">A supposition that is made about the presence of a header row.</param>
    /// <param name="textEncoding">
    /// The text encoding to be used to read the CSV file, or <c>null</c> to determine the <see cref="Encoding"/>
    /// automatically from the byte order mark (BOM).
    /// </param>
    /// <param name="analyzedLines">Maximum number of lines to analyze in the CSV file. The minimum 
    /// value is <see cref="CsvAnalyzer.AnalyzedLinesMinCount" />. If the file has fewer lines than 
    /// <paramref name="analyzedLines" />, it will be analyzed completely. (You can specify 
    /// <see cref="int.MaxValue">Int32.MaxValue</see> to analyze the entire file in any case.)</param>
    /// 
    /// <remarks>
    /// <para>
    /// Each <see cref="DynamicProperty.PropertyName"/> of <paramref name="mapping"/>
    /// MUST have a corresponding <see cref="DataColumn"/> in <paramref name="dataTable"/>
    /// - corresponding in the <see cref="DataColumn.Caption"/> property (case-insensitive)
    /// and the accepted data type.
    /// </para>
    /// <para>
    /// Effort must be taken that the 
    /// <see cref="DynamicProperty.PropertyName"/>s in <paramref name="mapping"/> are unique, 
    /// even when treated case-insensitive.
    /// </para>
    /// <para>
    /// The <see cref="DynamicProperty"/> instances in <paramref name="mapping"/> don't need to match 
    /// all columns of the <see cref="DataTable"/> or all columns of the CSV file (neither in
    /// number nor in order).
    /// </para>
    /// <para>
    /// The method performs a statistical analysis on the CSV file to find the appropriate 
    /// parameters for reading the file. The result of the analysis is therefore always only an estimate, 
    /// the accuracy of which increases with the number of lines analyzed. The analysis is time-consuming 
    /// because the CSV file has to be accessed for reading.
    /// </para>
    /// <para>
    /// This method also automatically determines the <see cref="Encoding"/> of the CSV file from the
    /// byte order mark (BOM) if the argument of the <paramref name="textEncoding"/> parameter is <c>null</c>.
    /// If the <see cref="Encoding"/> cannot be determined automatically, <see cref="Encoding.UTF8"/> is used.
    /// </para>
    /// <para>
    /// The field delimiters COMMA (<c>','</c>, %x2C), SEMICOLON  (<c>';'</c>, %x3B), HASH (<c>'#'</c>, %x23),
    /// TAB (<c>'\t'</c>, %x09), and SPACE (<c>' '</c>, %x20) are recognized automatically.
    /// </para>
    /// </remarks>
    /// 
    /// <example>
    /// <note type="note">In the following code examples - for easier readability - exception handling 
    /// has been omitted.</note>
    /// <para>DataTable serialization with CSV:</para>
    /// <code language="cs" source="..\Examples\DataTableExample.cs"/>
    /// </example>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="dataTable"/>, or <paramref name="filePath"/>,
    /// or <paramref name="mapping"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <para><paramref name="filePath" /> is not a valid file path.</para>
    /// <para>- or -</para>
    /// <para>
    /// There is a <see cref="DynamicProperty"/> in <paramref name="mapping"/> whose 
    /// <see cref="DynamicProperty.PropertyName"/> finds no corresponding <see cref="DataColumn.ColumnName"/>
    /// in <paramref name="dataTable"/>.
    /// </para>
    /// </exception>
    /// <exception cref="FormatException">Parsing fails and <see cref="ITypeConverter{T}.Throwing"/> is <c>true</c>.</exception>
    /// <exception cref="NoNullAllowedException">The <paramref name="mapping"/> doesn't match the schema of
    /// the <paramref name="dataTable"/>.</exception>
    /// <exception cref="ConstraintException">The parsed CSV data does not match the schema of
    /// the <paramref name="dataTable"/>.</exception>
    /// <exception cref="IOException">I/O error.</exception>
    /// <exception cref="ObjectDisposedException">The file was already closed.</exception>
    public static void FillAnalyzed(DataTable dataTable,
                                    string filePath,
                                    CsvMapping mapping,
                                    Header header = Header.ProbablyPresent,
                                    Encoding? textEncoding = null,
                                    int analyzedLines = CsvAnalyzer.AnalyzedLinesMinCount)
    {
        using CsvReader reader = Csv.OpenReadAnalyzed(filePath, header, textEncoding, analyzedLines, true);
        Fill(dataTable, reader, mapping);
    }

    private static bool DetermineDisableCaching<TResult>(ref CsvOpts options)
    {
        if(options.HasFlag(CsvOpts.DisableCaching))
        { 
            return false;
        }  
        
        Type resultType = typeof(TResult);

        if (resultType == _mappingType || resultType == _dynamicType)
        {
            return true;
        }

        if(resultType != _recordType)
        {
            options |= CsvOpts.DisableCaching;
        }
        
        return false;
    }
}
using FolkerKinzel.CsvTools.Mappings.Intls;
using FolkerKinzel.CsvTools.Mappings.Intls.Extensions;
using FolkerKinzel.CsvTools.Mappings.TypeConverters;
using FolkerKinzel.CsvTools.Mappings.TypeConverters.Interfaces;
using FolkerKinzel.Helpers.Polyfills;
using System.Data;
using System.Text;

namespace FolkerKinzel.CsvTools.Mappings;

/// <summary>Static class that provides methods for CSV serialization of 
/// collections of any data type.</summary>
public static class CsvConverter
{
    private static readonly Type _mappingType = typeof(CsvMapping);
    private static readonly Type _recordType = typeof(CsvRecord);
    private static readonly Type _dynamicType = typeof(object);

    /// <summary>
    /// Writes the content of a collection of <typeparamref name="TData"/> instances 
    /// as CSV. </summary>
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
    /// is passed to the method as <c>dynamic</c> argument: Inside the 
    /// <paramref name="conversion"/> method the registered 
    /// <see cref="DynamicProperty"/> instances can be used like 
    /// regular .NET properties, but without IntelliSense ("late binding").
    /// </para>
    /// <para>
    /// With each call of <paramref name="conversion"/> all <see cref="DynamicProperty"/> 
    /// instances in <paramref name="mapping"/> are reset to their 
    /// <see cref="DynamicProperty.DefaultValue"/>.
    /// </para>
    /// </param>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="data"/>, or 
    /// <paramref name="writer"/>, or <paramref name="mapping"/>, or <paramref name="conversion"/>
    /// is <c>null</c>.</exception>
    /// <exception cref="IOException">I/O error.</exception>
    /// <exception cref="ObjectDisposedException">The file was already closed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write<TData>(IEnumerable<TData?> data,
                                    CsvWriter writer,
                                    CsvMapping mapping,
                                    Action<TData, dynamic> conversion)
        => Write(data, writer, new ToCsvIntl<TData>(mapping, conversion));

    public static void Write<TData>(IEnumerable<TData?> data,
                                    CsvWriter writer,
                                    ToCsv<TData> converter)
    {
        _ArgumentNullException.ThrowIfNull(data, nameof(data));

        using var csvWriter = new CsvWriter<TData>(writer, converter);

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
    /// The <see cref="DynamicProperty"/> instances in <paramref name="mapping"/> don't need to 
    /// match all columns of the <see cref="DataTable"/> or all columns of the CSV file (neither 
    /// in number nor in order).
    /// </para>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="dataTable"/>, or 
    /// <paramref name="writer"/>, or <paramref name="mapping"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// There is a <see cref="DynamicProperty"/> in <paramref name="mapping"/> whose 
    /// <see cref="DynamicProperty.PropertyName"/> finds no corresponding 
    /// <see cref="DataColumn.ColumnName"/> in <paramref name="dataTable"/>.
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
        WriteIntl(dataTable, writer, mapping);
    }

    /// <summary>
    /// Saves a collection of <typeparamref name="TData"/> instances as a CSV file with
    /// header row.
    /// </summary>
    /// <typeparam name="TData">
    /// Generic type parameter for the data type to write as CSV row.
    /// </typeparam>
    /// <param name="data">The data to save as CSV file. Each item will be represented 
    /// with a CSV row.
    /// </param>
    /// <param name="filePath">File path of the CSV file.</param>
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
    /// is passed to the method as <c>dynamic</c> argument: Inside the 
    /// <paramref name="conversion"/> method the registered 
    /// <see cref="DynamicProperty"/> instances can be used like 
    /// regular .NET properties, but without IntelliSense ("late binding").
    /// </para>
    /// <para>
    /// With each call of <paramref name="conversion"/> all <see cref="DynamicProperty"/>
    /// instances in <paramref name="mapping"/> are reset to their 
    /// <see cref="DynamicProperty.DefaultValue"/>.
    /// </para>
    /// </param>
    /// <param name="delimiter">The field separator character.</param>
    /// <param name="textEncoding">
    /// The text encoding to be used or <c>null</c> for <see cref="Encoding.UTF8"/>.
    /// </param>
    /// <param name="columnNames">
    /// <para>
    /// A collection of column names for the header to be written, or <c>null</c> to use the
    /// <see cref="CsvMapping.PropertyNames"/> of <paramref name="mapping"/> as column names.
    /// </para>
    /// <para>
    /// The collection determines the order in which the columns appear in the CSV file.
    /// </para>
    /// <para>
    /// The collection will be copied. If the collection contains <c>null</c> values, empty 
    /// strings, or white space, these are replaced by automatically generated column names.
    /// Column names cannot appear twice. By default the comparison is case-sensitive but 
    /// it will be reset to a case-insensitive comparison if the column names are also 
    /// unique when treated case-insensitive.
    /// </para>
    /// </param>
    /// 
    /// <remarks>
    /// <para>
    /// Creates a new CSV file. If the target file already exists, it is truncated 
    /// and overwritten.
    /// </para>
    /// <para>
    /// When exchanging CSV data with Excel, the appropriate arguments can be determined 
    /// with <see cref="Csv.GetExcelArguments"/>.
    /// </para>
    /// </remarks>
    ///
    /// <exception cref="ArgumentNullException"><paramref name="data"/>, or 
    /// <paramref name="filePath"/>, or <paramref name="mapping"/>, or 
    /// <paramref name="conversion"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// <para>
    /// <paramref name="filePath" /> is not a valid file path.
    /// </para>
    /// <para>
    /// - or -
    /// </para>
    /// <para>
    /// <paramref name="columnNames"/> is not <c>null</c> and a column name in 
    /// <paramref name="columnNames" /> occurs twice.
    /// </para>
    /// </exception>
    /// <exception cref="IOException">I/O error.</exception>
    /// <exception cref="ObjectDisposedException">The file was already closed.</exception>
    public static void Save<TData>(IEnumerable<TData?> data,
                                   string filePath,
                                   CsvMapping mapping,
                                   Action<TData, dynamic> conversion,
                                   char delimiter = ',',
                                   Encoding? textEncoding = null,
                                   IReadOnlyCollection<string?>? columnNames = null)
    {
        using CsvWriter csvWriter = Csv.OpenWrite(
            filePath, GetColumnNames(columnNames, mapping), delimiter, textEncoding);
        Write(data, csvWriter, mapping, conversion);
    }

    /// <summary>
    /// Saves a collection of <typeparamref name="TData"/> instances as a CSV file with
    /// header row.
    /// </summary>
    /// <typeparam name="TData">
    /// Generic type parameter for the data type to write as CSV row.
    /// </typeparam>
    /// <param name="data">The data to save as CSV file. Each item will be represented 
    /// with a CSV row.
    /// </param>
    /// <param name="filePath">File path of the CSV file.</param>
    /// <param name="converter">
    /// An object that converts a <typeparamref name="TData"/> instance to a 
    /// CSV row.
    /// </param>
    /// <param name="delimiter">The field separator character.</param>
    /// <param name="textEncoding">
    /// The text encoding to be used or <c>null</c> for <see cref="Encoding.UTF8"/>.
    /// </param>
    /// <param name="columnNames">
    /// <para>
    /// A collection of column names for the header to be written, or <c>null</c> to use the
    /// <see cref="CsvMapping.PropertyNames"/> of the <see cref="CsvMapping"/> as column names.
    /// </para>
    /// <para>
    /// The collection determines the order in which the columns appear in the CSV file.
    /// </para>
    /// <para>
    /// The collection will be copied. If the collection contains <c>null</c> values, empty 
    /// strings, or white space, these are replaced by automatically generated column names.
    /// Column names cannot appear twice. By default the comparison is case-sensitive but 
    /// it will be reset to a case-insensitive comparison if the column names are also 
    /// unique when treated case-insensitive.
    /// </para>
    /// </param>
    /// 
    /// <remarks>
    /// <para>
    /// Creates a new CSV file. If the target file already exists, it is truncated 
    /// and overwritten.
    /// </para>
    /// <para>
    /// When exchanging CSV data with Excel, the appropriate arguments can be determined 
    /// with <see cref="Csv.GetExcelArguments"/>.
    /// </para>
    /// </remarks>
    ///
    /// <exception cref="ArgumentNullException"><paramref name="data"/>, or 
    /// <paramref name="filePath"/>, or <paramref name="converter"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <para>
    /// <paramref name="filePath" /> is not a valid file path.
    /// </para>
    /// <para>
    /// - or -
    /// </para>
    /// <para>
    /// <paramref name="columnNames"/> is not <c>null</c> and a column name in 
    /// <paramref name="columnNames" /> occurs twice.
    /// </para>
    /// </exception>
    /// <exception cref="IOException">I/O error.</exception>
    public static void Save<TData>(IEnumerable<TData?> data,
                                   string filePath,
                                   ToCsv<TData> converter,
                                   char delimiter = ',',
                                   Encoding? textEncoding = null,
                                   IReadOnlyCollection<string?>? columnNames = null)
    {
        _ArgumentNullException.ThrowIfNull(converter, nameof(converter));

        using CsvWriter csvWriter = Csv.OpenWrite(
            filePath, GetColumnNames(columnNames, converter.Mapping), delimiter, textEncoding);
        Write(data, csvWriter, converter);
    }

    /// <summary>
    /// Saves a collection of <typeparamref name="TData"/> instances as a CSV file
    /// without header row.
    /// </summary>
    /// <typeparam name="TData">
    /// Generic type parameter for the data type to write as CSV row.
    /// </typeparam>
    /// <param name="data">The data to save as CSV file. Each item will be represented 
    /// with a CSV row.</param>
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
    /// is passed to the method as <c>dynamic</c> argument: Inside the 
    /// <paramref name="conversion"/> method the registered 
    /// <see cref="DynamicProperty"/> instances can be used like 
    /// regular .NET properties, but without IntelliSense ("late binding").
    /// </para>
    /// <para>
    /// With each call of <paramref name="conversion"/> all <see cref="DynamicProperty"/>
    /// instances in <paramref name="mapping"/> are reset to their 
    /// <see cref="DynamicProperty.DefaultValue"/>.
    /// </para>
    /// </param>
    /// <param name="delimiter">The field separator character.</param>
    /// <param name="textEncoding">
    /// The text encoding to be used or <c>null</c> for <see cref="Encoding.UTF8"/>.
    /// </param>
    /// 
    /// <remarks>
    /// <para>
    /// Creates a new CSV file. If the target file already exists, it is truncated and 
    /// overwritten.
    /// </para>
    /// <para>
    /// When exchanging CSV data with Excel, the appropriate arguments can be determined 
    /// with <see cref="Csv.GetExcelArguments"/>.
    /// </para>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="filePath"/>, or 
    /// <paramref name="data"/>, or <paramref name="mapping"/>, or <paramref name="conversion"/>
    /// is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"> <paramref name="filePath" /> is not a valid
    /// file path.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="columnsCount"/> is negative.
    /// </exception>
    /// <exception cref="IOException">I/O error.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Save<TData>(IEnumerable<TData?> data,
                                   string filePath,
                                   int columnsCount,
                                   CsvMapping mapping,
                                   Action<TData, dynamic> conversion,
                                   char delimiter = ',',
                                   Encoding? textEncoding = null)
        => Save(data,
                filePath,
                columnsCount,
                new ToCsvIntl<TData>(mapping, conversion),
                delimiter,
                textEncoding);

    /// <summary>
    /// Saves a collection of <typeparamref name="TData"/> instances as a CSV file
    /// without header row.
    /// </summary>
    /// <typeparam name="TData">
    /// Generic type parameter for the data type to write as CSV row.
    /// </typeparam>
    /// <param name="data">The data to save as CSV file. Each item will be represented 
    /// with a CSV row.</param>
    /// <param name="filePath">File path of the CSV file.</param>
    /// <param name="columnsCount">Number of columns in the CSV file.</param>
    /// <param name="converter">
    /// An object that converts a <typeparamref name="TData"/> instance to a 
    /// CSV row.
    /// </param>
    /// <param name="delimiter">The field separator character.</param>
    /// <param name="textEncoding">
    /// The text encoding to be used or <c>null</c> for <see cref="Encoding.UTF8"/>.
    /// </param>
    /// 
    /// <remarks>
    /// <para>
    /// Creates a new CSV file. If the target file already exists, it is truncated and 
    /// overwritten.
    /// </para>
    /// <para>
    /// When exchanging CSV data with Excel, the appropriate arguments can be determined 
    /// with <see cref="Csv.GetExcelArguments"/>.
    /// </para>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="filePath"/>, or 
    /// <paramref name="data"/>, or <paramref name="converter"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"> <paramref name="filePath" /> is not a valid
    /// file path.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="columnsCount"/> is negative.
    /// </exception>
    /// <exception cref="IOException">I/O error.</exception>
    public static void Save<TData>(IEnumerable<TData?> data,
                                   string filePath,
                                   int columnsCount,
                                   ToCsv<TData> converter,
                                   char delimiter = ',',
                                   Encoding? textEncoding = null)
    {
        using CsvWriter csvWriter = new(filePath, columnsCount, delimiter, textEncoding);
        Write(data, csvWriter, converter);
    }

    /// <summary>
    /// Saves the content of a <see cref="DataTable"/> as a CSV file with header.
    /// </summary>
    /// <param name="dataTable">The <see cref="DataTable"/> whose content is written.</param>
    /// <param name="filePath">File path of the CSV file.</param>
    /// <param name="mapping">The <see cref="CsvMapping"/> to be used.</param>
    /// <param name="delimiter">The field separator character.</param>
    /// <param name="textEncoding">
    /// The text encoding to be used or <c>null</c> for <see cref="Encoding.UTF8"/>.
    /// </param>
    /// <param name="csvColumnNames">
    /// <para>
    /// A collection of column names for the CSV header row to be written, or <c>null</c> to 
    /// use the <see cref="DataColumn.ColumnName"/>s of <paramref name="dataTable"/> as CSV 
    /// column names.
    /// </para>
    /// <para>
    /// The collection determines the order in which the columns appear in the CSV file.
    /// </para>
    /// <para>
    /// The collection will be copied. If the collection contains <c>null</c> values, empty 
    /// strings or white space, these are replaced by automatically generated column names. 
    /// Column names cannot appear twice. By default the comparison is case-sensitive but 
    /// it will be reset to a case-insensitive comparison if the column names are also unique
    /// when treated case-insensitive.
    /// </para>
    /// </param>
    /// 
    /// <remarks>
    /// <para>Creates a new CSV file. If the target file already exists, it is 
    /// truncated and overwritten.
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
    /// The <see cref="DynamicProperty"/> instances in <paramref name="mapping"/> don't need 
    /// to match all columns of the <see cref="DataTable"/> or all columns of the CSV file 
    /// (neither in number nor in order).
    /// </para>
    /// <para>
    /// When exchanging CSV data with Excel, the appropriate arguments can be determined 
    /// with <see cref="Csv.GetExcelArguments"/>.
    /// </para>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="dataTable"/>, or 
    /// <paramref name="filePath"/>, or <paramref name="mapping"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// <para><paramref name="filePath" /> is not a valid file path.</para>
    /// <para>- or -</para>
    /// <para>
    /// There is a <see cref="DynamicProperty"/> in <paramref name="mapping"/> whose 
    /// <see cref="DynamicProperty.PropertyName"/> finds no corresponding 
    /// <see cref="DataColumn.ColumnName"/> in <paramref name="dataTable"/>.
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
                            CsvMapping mapping,
                            char delimiter = ',',
                            Encoding? textEncoding = null,
                            IEnumerable<string?>? csvColumnNames = null)
    {
        _ArgumentNullException.ThrowIfNull(dataTable, nameof(dataTable));
        _ArgumentNullException.ThrowIfNull(mapping, nameof(mapping));

        using var writer = new CsvWriter(
            filePath, 
            csvColumnNames ?? dataTable.Columns.Cast<DataColumn>().Select(x => x.ColumnName),
            caseSensitive: false,
            delimiter,
            textEncoding);

        WriteIntl(dataTable, writer, mapping);
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
    /// With each call of <paramref name="conversion"/> all <see cref="DynamicProperty"/> instances
    /// in <paramref name="mapping"/> are reset to their <see cref="DynamicProperty.DefaultValue"/>.
    /// </para>
    /// </param>
    /// <param name="delimiter">The field separator character.</param>
    /// <param name="columnNames">
    /// <para>
    /// A collection of column names for the header to be written, or <c>null</c> to use the
    /// <see cref="CsvMapping.PropertyNames"/> of <paramref name="mapping"/> as column names.
    /// </para>
    /// <para>
    /// The collection determines the order in which the columns appear in the CSV file.
    /// </para>
    /// <para>
    /// The collection will be copied. If the collection contains <c>null</c> values, empty 
    /// strings, or white space, these are replaced by automatically generated column names.
    /// Column names cannot appear twice. By default the comparison is case-sensitive but 
    /// it will be reset to a case-insensitive comparison if the column names are also 
    /// unique when treated case-insensitive.
    /// </para>
    /// </param>
    /// 
    /// <returns>A CSV <see cref="string"/> with header row that contains the contents of 
    /// <paramref name="data"/>.</returns>
    ///
    /// <exception cref="ArgumentNullException"><paramref name="data"/>, or 
    /// <paramref name="mapping"/>, or <paramref name="conversion"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// A column name in <paramref name="columnNames" /> occurs twice.
    /// </exception>
    /// <exception cref="IOException">I/O error.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToCsv<TData>(IEnumerable<TData?> data,
                                      CsvMapping mapping,
                                      Action<TData, dynamic> conversion,
                                      char delimiter = ',',
                                      IReadOnlyCollection<string?>? columnNames = null)
        => ToCsvIntl(data, new ToCsvIntl<TData>(mapping, conversion), delimiter, columnNames);

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
    /// <param name="converter">
    /// An object that converts a <typeparamref name="TData"/> instance to a 
    /// CSV row.
    /// </param>
    /// <param name="delimiter">The field separator character.</param>
    /// <param name="columnNames">
    /// <para>
    /// A collection of column names for the header to be written, or <c>null</c> to use the
    /// <see cref="CsvMapping.PropertyNames"/> of the <see cref="CsvMapping"/> as column names.
    /// </para>
    /// <para>
    /// The collection determines the order in which the columns appear in the CSV file.
    /// </para>
    /// <para>
    /// The collection will be copied. If the collection contains <c>null</c> values, empty 
    /// strings, or white space, these are replaced by automatically generated column names.
    /// Column names cannot appear twice. By default the comparison is case-sensitive but 
    /// it will be reset to a case-insensitive comparison if the column names are also 
    /// unique when treated case-insensitive.
    /// </para>
    /// </param>
    /// 
    /// <returns>A CSV <see cref="string"/> with header row that contains the contents of 
    /// <paramref name="data"/>.</returns>
    ///
    /// <exception cref="ArgumentNullException"><paramref name="data"/> or 
    /// <paramref name="converter"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// A column name in <paramref name="columnNames" /> occurs twice.
    /// </exception>
    /// <exception cref="IOException">I/O error.</exception>
    public static string ToCsv<TData>(IEnumerable<TData?> data,
                                      ToCsv<TData> converter,
                                      char delimiter = ',',
                                      IReadOnlyCollection<string?>? columnNames = null)
    {
        _ArgumentNullException.ThrowIfNull(converter, nameof(converter));
        return ToCsvIntl(data, converter, delimiter, columnNames);
    }

    /// <summary>
    /// Converts a collection of <typeparamref name="TData"/> instances to a CSV 
    /// <see cref="string"/> without header row.
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
    /// With each call of <paramref name="conversion"/> all <see cref="DynamicProperty"/> instances
    /// in <paramref name="mapping"/> are reset to their <see cref="DynamicProperty.DefaultValue"/>.
    /// </para>
    /// </param>
    /// <param name="delimiter">The field separator character.</param>
    /// 
    /// <returns>A CSV <see cref="string"/> without header row that contains the contents of 
    /// <paramref name="data"/>.</returns>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="data"/>, or 
    /// <paramref name="mapping"/>, or <paramref name="conversion"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="columnsCount"/> is negative.
    /// </exception>
    /// <exception cref="IOException">I/O error.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToCsv<TData>(IEnumerable<TData?> data,
                                      int columnsCount,
                                      CsvMapping mapping,
                                      Action<TData, dynamic> conversion,
                                      char delimiter = ',')
        => ToCsv(data, columnsCount, new ToCsvIntl<TData>(mapping, conversion), delimiter);

    /// <summary>
    /// Converts a collection of <typeparamref name="TData"/> instances to a CSV 
    /// <see cref="string"/> without header row.
    /// </summary>
    /// <typeparam name="TData">
    /// Generic type parameter for the data type to write as CSV row.
    /// </typeparam>
    /// <param name="data">The data to convert to CSV. Each item will be represented with 
    /// a CSV row.
    /// </param>
    /// <param name="columnsCount">Number of columns in the CSV file.</param>
    /// <param name="converter">
    /// An object that converts a <typeparamref name="TData"/> instance to a 
    /// CSV row.
    /// </param>
    /// <param name="delimiter">The field separator character.</param>
    /// 
    /// <returns>A CSV <see cref="string"/> without header row that contains the contents of 
    /// <paramref name="data"/>.</returns>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="data"/> or
    /// <paramref name="converter"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="columnsCount"/> is negative.
    /// </exception>
    /// <exception cref="IOException">I/O error.</exception>
    public static string ToCsv<TData>(IEnumerable<TData?> data,
                                      int columnsCount,
                                      ToCsv<TData> converter,
                                      char delimiter = ',')
    {
        using var stringWriter = new StringWriter();
        using CsvWriter csvWriter = Csv.OpenWrite(stringWriter, columnsCount, delimiter);

        Write(data, csvWriter, converter);

        return stringWriter.ToString();
    }

    /// <summary>Initializes a <see cref="CsvReader{TResult}"/> instance to parse CSV data 
    /// as a collection of <typeparamref name="TResult"/> instances.
    /// </summary>
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
    /// <param name="delimiter">The field separator character.</param>
    /// <param name="isHeaderPresent"> <c>true</c>, to interpret the first line as a header, 
    /// otherwise <c>false</c>.</param>
    /// <param name="options">Options for reading CSV.</param>
    /// 
    /// <returns>A <see cref="CsvReader{TResult}"/> that allows you to iterate through the
    /// data parsed from the CSV.</returns>
    /// 
    /// <remarks>
    /// When importing CSV data from Excel, the appropriate arguments can be determined 
    /// with <see cref="Csv.GetExcelArguments"/>.
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="reader" />, or 
    /// <paramref name="mapping"/>, or <paramref name="conversion"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="delimiter"/> is either 
    /// the double quotes <c>"</c> or a line break character ('\r' or  '\n').</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CsvReader<TResult> OpenRead<TResult>(TextReader reader,
                                                       CsvMapping mapping,
                                                       Func<dynamic, TResult> conversion,
                                                       char delimiter = ',',
                                                       bool isHeaderPresent = true,
                                                       CsvOpts options = CsvOpts.Default)
        => OpenRead(reader, new CsvToIntl<TResult>(mapping, conversion), delimiter, isHeaderPresent, options);

    /// <summary>Initializes a <see cref="CsvReader{TResult}"/> instance to parse CSV data 
    /// as a collection of <typeparamref name="TResult"/> instances.
    /// </summary>
    /// 
    /// <typeparam name="TResult"> Generic type parameter that specifies the <see cref="Type"/>
    /// of the items that the <see cref="CsvReader{TResult}"/> returns.</typeparam>
    /// 
    /// <param name="reader">The <see cref="TextReader" /> with which the CSV data is
    /// read.</param>
    /// <param name="converter">
    /// An object that converts a CSV row to a <typeparamref name="TResult"/> 
    /// instance.
    /// </param>
    /// <param name="delimiter">The field separator character.</param>
    /// <param name="isHeaderPresent"> <c>true</c>, to interpret the first line as a header, 
    /// otherwise <c>false</c>.</param>
    /// <param name="options">Options for reading CSV.</param>
    /// 
    /// <returns>A <see cref="CsvReader{TResult}"/> that allows you to iterate through the
    /// data parsed from the CSV.</returns>
    /// 
    /// <remarks>
    /// When importing CSV data from Excel, the appropriate arguments can be determined 
    /// with <see cref="Csv.GetExcelArguments"/>.
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="reader" />, or 
    /// <paramref name="converter"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="delimiter"/> is either 
    /// the double quotes <c>"</c> or a line break character ('\r' or  '\n').</exception>
    public static CsvReader<TResult> OpenRead<TResult>(TextReader reader,
                                                       CsvTo<TResult> converter,
                                                       char delimiter = ',',
                                                       bool isHeaderPresent = true,
                                                       CsvOpts options = CsvOpts.Default)
    {
        bool cloneMapping = MustMappingBeCloned<TResult>(ref options);

        return new CsvReader<TResult>(new CsvReader(reader,
                                                    delimiter,
                                                    isHeaderPresent,
                                                    options),
                                        converter,
                                        cloneMapping);
    }

    /// <summary>Opens a CSV file for parsing its data.
    /// </summary>
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
    /// <param name="delimiter">The field separator character.</param>
    /// <param name="textEncoding">The text encoding to be used to read the CSV file
    /// or <c>null</c> for <see cref="Encoding.UTF8" />.</param>
    /// <param name="isHeaderPresent"> <c>true</c>, to interpret the first line as a header, 
    /// otherwise <c>false</c>.</param>
    /// <param name="options">Options for reading the CSV file.</param>
    /// 
    /// <returns>A <see cref="CsvReader{TResult}"/> that allows you to iterate through the
    /// data parsed from the CSV.</returns>
    /// 
    /// <remarks>
    /// <para>
    /// The optimal parameters can be determined automatically with 
    /// <see cref="Csv.AnalyzeFile(string, Encoding?, Header, int)"/> - or use
    /// <see cref="OpenReadAnalyzed{TResult}(string, CsvMapping, Func{dynamic, TResult}, Encoding?, 
    /// Header, int)"/>.
    /// </para>
    /// <para>
    /// When importing CSV data from Excel, the appropriate arguments can be determined 
    /// with <see cref="Csv.GetExcelArguments"/>.
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
    /// <exception cref="ArgumentNullException"> <paramref name="filePath" />, or 
    /// <paramref name="mapping"/>, or <paramref name="conversion"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"> <paramref name="filePath" /> is not a valid
    /// file path.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="delimiter"/> is either 
    /// the double quotes <c>"</c> or a line break character ('\r' or  '\n').</exception>
    /// <exception cref="IOException">I/O error.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CsvReader<TResult> OpenRead<TResult>(string filePath,
                                                       CsvMapping mapping,
                                                       Func<dynamic, TResult> conversion,
                                                       char delimiter = ',',
                                                       Encoding? textEncoding = null,
                                                       bool isHeaderPresent = true,
                                                       CsvOpts options = CsvOpts.Default)
        => OpenRead(filePath, new CsvToIntl<TResult>(mapping, conversion), delimiter, textEncoding, isHeaderPresent, options);

    /// <summary>Opens a CSV file for parsing its data.
    /// </summary>
    /// 
    /// <typeparam name="TResult"> Generic type parameter that specifies the <see cref="Type"/>
    /// of the items that the <see cref="CsvReader{TResult}"/> returns.</typeparam>
    /// 
    /// <param name="filePath">File path of the CSV file to read.</param>
    /// <param name="converter">
    /// An object that converts a CSV row to a <typeparamref name="TResult"/> 
    /// instance.
    /// </param>
    /// <param name="delimiter">The field separator character.</param>
    /// <param name="textEncoding">The text encoding to be used to read the CSV file
    /// or <c>null</c> for <see cref="Encoding.UTF8" />.</param>
    /// <param name="isHeaderPresent"> <c>true</c>, to interpret the first line as a header, 
    /// otherwise <c>false</c>.</param>
    /// <param name="options">Options for reading the CSV file.</param>
    /// 
    /// <returns>A <see cref="CsvReader{TResult}"/> that allows you to iterate through the
    /// data parsed from the CSV.</returns>
    /// 
    /// <remarks>
    /// <para>
    /// The optimal parameters can be determined automatically with 
    /// <see cref="Csv.AnalyzeFile(string, Encoding?, Header, int)"/> - or use
    /// <see cref="OpenReadAnalyzed{TResult}(string, CsvMapping, Func{dynamic, TResult}, Encoding?, 
    /// Header, int)"/>.
    /// </para>
    /// <para>
    /// When importing CSV data from Excel, the appropriate arguments can be determined 
    /// with <see cref="Csv.GetExcelArguments"/>.
    /// </para>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="filePath" /> or 
    /// <paramref name="converter"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"> <paramref name="filePath" /> is not a valid
    /// file path.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="delimiter"/> is either 
    /// the double quotes <c>"</c> or a line break character ('\r' or  '\n').</exception>
    /// <exception cref="IOException">I/O error.</exception>
    public static CsvReader<TResult> OpenRead<TResult>(string filePath,
                                                       CsvTo<TResult> converter,
                                                       char delimiter = ',',
                                                       Encoding? textEncoding = null,
                                                       bool isHeaderPresent = true,
                                                       CsvOpts options = CsvOpts.Default)
    {
        bool cloneMapping = MustMappingBeCloned<TResult>(ref options);
        return new CsvReader<TResult>(new CsvReader(filePath,
                                                    delimiter,
                                                    textEncoding,
                                                    isHeaderPresent,
                                                    options),
                                      converter,
                                      cloneMapping);
    }

    /// <summary>Opens a CSV file for parsing its data after it had been analyzed.
    /// </summary>
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
    /// <param name="defaultEncoding">
    /// The text <see cref="Encoding"/> to be used if the CSV file has no byte order mark (BOM), or 
    /// <c>null</c> to use <see cref="Encoding.UTF8"/> in this case. Use <see cref="Csv.GetExcelArguments"/>
    /// to get the appropriate argument for this parameter when importing CSV data from Excel.
    /// </param>
    /// <param name="header">A supposition that is made about the presence of a header row.</param>
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
    /// The method performs a statistical analysis on the CSV file. The result of the analysis is therefore 
    /// always only an estimate, the accuracy of which increases with the number of lines analyzed.
    /// </para>
    /// <para>
    /// The field delimiters COMMA (<c>','</c>, %x2C), SEMICOLON  (<c>';'</c>, %x3B), 
    /// HASH (<c>'#'</c>, %x23), TAB (<c>'\t'</c>, %x09), and SPACE (<c>' '</c>, %x20) are recognized 
    /// automatically.
    /// </para>
    /// <para>
    /// This method also tries to determine the <see cref="Encoding"/> of the CSV file from the
    /// byte order mark (BOM). If no byte order mark can be found, <paramref name="defaultEncoding"/> is
    /// used.
    /// </para>
    /// </remarks>
    /// 
    /// <example>
    /// <note type="note">In the following code examples - for easier readability - exception handling
    /// has been omitted.</note>
    /// <para>CSV data exchange with Excel:</para>
    /// <code language="cs" source="..\Examples\ExcelExample.cs"/>
    /// </example>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="filePath"/>, or 
    /// <paramref name="mapping"/>, or <paramref name="conversion"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <para><paramref name="header"/> is not a defined value of 
    /// the <see cref="Header"/> enum.</para>
    /// <para> - or -</para>
    /// <para><paramref name="header"/> is a combination of <see cref="Header"/> values.</para>
    /// </exception>
    /// <exception cref="CsvFormatException">Invalid CSV file. Try to increase the value of 
    /// <paramref name="analyzedLines"/> to get a better analyzer result!</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CsvReader<TResult> OpenReadAnalyzed<TResult>(string filePath,
                                                               CsvMapping mapping,
                                                               Func<dynamic, TResult> conversion,
                                                               Encoding? defaultEncoding = null,
                                                               Header header = Header.ProbablyPresent,
                                                               int analyzedLines = CsvAnalyzer.AnalyzedLinesMinCount)
        => OpenReadAnalyzed(filePath, new CsvToIntl<TResult>(mapping, conversion), defaultEncoding, header, analyzedLines);

    /// <summary>Opens a CSV file for parsing its data after it had been analyzed.
    /// </summary>
    /// 
    /// <typeparam name="TResult"> Generic type parameter that specifies the <see cref="Type"/>
    /// of the items that the <see cref="CsvReader{TResult}"/> returns.</typeparam>
    /// 
    /// <param name="filePath">File path of the CSV file to read.</param>
    /// <param name="converter">
    /// An object that converts a CSV row to a <typeparamref name="TResult"/> instance.
    /// </param>
    /// <param name="defaultEncoding">
    /// The text <see cref="Encoding"/> to be used if the CSV file has no byte order mark (BOM), or 
    /// <c>null</c> to use <see cref="Encoding.UTF8"/> in this case. Use <see cref="Csv.GetExcelArguments"/>
    /// to get the appropriate argument for this parameter when importing CSV data from Excel.
    /// </param>
    /// <param name="header">A supposition that is made about the presence of a header row.</param>
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
    /// The method performs a statistical analysis on the CSV file. The result of the analysis is therefore 
    /// always only an estimate, the accuracy of which increases with the number of lines analyzed.
    /// </para>
    /// <para>
    /// The field delimiters COMMA (<c>','</c>, %x2C), SEMICOLON  (<c>';'</c>, %x3B), 
    /// HASH (<c>'#'</c>, %x23), TAB (<c>'\t'</c>, %x09), and SPACE (<c>' '</c>, %x20) are recognized 
    /// automatically.
    /// </para>
    /// <para>
    /// This method also tries to determine the <see cref="Encoding"/> of the CSV file from the
    /// byte order mark (BOM). If no byte order mark can be found, <paramref name="defaultEncoding"/> is
    /// used.
    /// </para>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="filePath"/>, or 
    /// <paramref name="converter"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <para><paramref name="header"/> is not a defined value of 
    /// the <see cref="Header"/> enum.</para>
    /// <para> - or -</para>
    /// <para><paramref name="header"/> is a combination of <see cref="Header"/> values.</para>
    /// </exception>
    /// <exception cref="CsvFormatException">Invalid CSV file. Try to increase the value of 
    /// <paramref name="analyzedLines"/> to get a better analyzer result!</exception>
    public static CsvReader<TResult> OpenReadAnalyzed<TResult>(string filePath,
                                                               CsvTo<TResult> converter,
                                                               Encoding? defaultEncoding = null,
                                                               Header header = Header.ProbablyPresent,
                                                               int analyzedLines = CsvAnalyzer.AnalyzedLinesMinCount)
    {
        (CsvAnalyzerResult analyzerResult, Encoding enc) = Csv.AnalyzeFile(filePath,
                                                                           defaultEncoding,
                                                                           header,
                                                                           analyzedLines);
        CsvOpts options = analyzerResult.Options;
        bool cloneMapping = MustMappingBeCloned<TResult>(ref options);

        return new CsvReader<TResult>(new CsvReader(filePath,
                                                    analyzerResult.Delimiter,
                                                    enc,
                                                    analyzerResult.IsHeaderPresent,
                                                    options),
                                      converter,
                                      cloneMapping);
    }

    /// <summary>Parses a CSV-<see cref="string"/>.</summary>
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
    /// <param name="delimiter">The field separator character used in <paramref name="csv"/>.</param>
    /// <param name="isHeaderPresent"> <c>true</c>, to interpret the first line as a header, 
    /// otherwise <c>false</c>.</param>
    /// <param name="options">Parsing options.</param>
    /// 
    /// <returns>An array of <typeparamref name="TResult"/> instances, initialized from the parsed 
    /// <paramref name="csv"/>.</returns>
    /// 
    /// <remarks>
    /// <note type="tip">
    /// The optimal parameters can be determined automatically with 
    /// <see cref="Csv.AnalyzeString(string, Header, int)"/> - or use 
    /// <see cref="ParseAnalyzed{TResult}(string, CsvMapping, Func{dynamic, TResult}, Header, int)"/>.
    /// </note>
    /// </remarks>
    /// 
    /// <example>
    /// <note type="note">In the following code examples - for easier readability - exception handling
    /// has been omitted.</note>
    /// <para>Object serialization with CSV:</para>
    /// <code language="cs" source="..\Benchmarks\CalculationReader_Default.cs"/>
    /// </example>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="csv" />, or 
    /// <paramref name="mapping"/>, or <paramref name="conversion"/> is <c>null</c>.</exception>
    /// <exception cref="CsvFormatException">Invalid CSV. The interpretation depends
    /// on <paramref name="options"/>.</exception>
    /// <exception cref="FormatException">
    /// Parsing fails and <see cref="TypeConverter{T}.Throwing"/> is <c>true</c>.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult[] Parse<TResult>(string csv,
                                           CsvMapping mapping,
                                           Func<dynamic, TResult> conversion,
                                           char delimiter = ',',
                                           bool isHeaderPresent = true,
                                           CsvOpts options = CsvOpts.Default)
        => Parse(csv, new CsvToIntl<TResult>(mapping, conversion), delimiter, isHeaderPresent, options);

    /// <summary>Parses a CSV-<see cref="string"/>.</summary>
    /// 
    /// <typeparam name="TResult"> Generic type parameter that specifies the <see cref="Type"/>
    /// of the items in the array that the method returns.</typeparam>
    /// 
    /// <param name="csv">The CSV-<see cref="string"/> to parse.</param>
    /// <param name="converter">
    /// An object that converts a CSV row to a <typeparamref name="TResult"/> instance.
    /// </param>
    /// <param name="delimiter">The field separator character used in <paramref name="csv"/>.</param>
    /// <param name="isHeaderPresent"> <c>true</c>, to interpret the first line as a header, 
    /// otherwise <c>false</c>.</param>
    /// <param name="options">Parsing options.</param>
    /// 
    /// <returns>An array of <typeparamref name="TResult"/> instances, initialized from the parsed 
    /// <paramref name="csv"/>.</returns>
    /// 
    /// <remarks>
    /// <note type="tip">
    /// The optimal parameters can be determined automatically with 
    /// <see cref="Csv.AnalyzeString(string, Header, int)"/> - or use 
    /// <see cref="ParseAnalyzed{TResult}(string, CsvMapping, Func{dynamic, TResult}, Header, int)"/>.
    /// </note>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="csv" /> or <paramref name="converter"/>
    /// is <c>null</c>.</exception>
    /// <exception cref="CsvFormatException">Invalid CSV. The interpretation depends
    /// on <paramref name="options"/>.</exception>
    /// <exception cref="FormatException">
    /// Parsing fails and <see cref="TypeConverter{T}.Throwing"/> is <c>true</c>.
    /// </exception>
    public static TResult[] Parse<TResult>(string csv,
                                           CsvTo<TResult> converter,
                                           char delimiter = ',',
                                           bool isHeaderPresent = true,
                                           CsvOpts options = CsvOpts.Default)
    {
        _ArgumentNullException.ThrowIfNull(csv, nameof(csv));

        bool cloneMapping = MustMappingBeCloned<TResult>(ref options);

        using var stringReader = new StringReader(csv);
        using var csvReader = new CsvReader(stringReader, delimiter, isHeaderPresent, options);
        using var typedReader = new CsvReader<TResult>(csvReader, converter, cloneMapping);
        return [.. typedReader];
    }

    /// <summary>
    /// Parses a CSV-<see cref="string"/> after it had been analyzed.
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
    /// <param name="analyzedLines">Maximum number of lines to analyze in <paramref name="csv"/>. The
    /// minimum value is <see cref="CsvAnalyzer.AnalyzedLinesMinCount" />. If <paramref name="csv"/> 
    /// has fewer lines than <paramref name="analyzedLines" />, it will be analyzed completely. (You 
    /// can specify <see cref="int.MaxValue">Int32.MaxValue</see> to analyze the entire 
    /// <see cref="string"/> in any case.)</param>
    /// 
    /// <returns>An array of <typeparamref name="TResult"/> instances, initialized from the parsed 
    /// <paramref name="csv"/>.</returns>
    /// 
    /// <remarks>
    /// <para>
    /// <see cref="CsvAnalyzer" /> performs a statistical analysis on <paramref name="csv"/>. The 
    /// result of the analysis is therefore always only an estimate, the accuracy of which increases 
    /// with the number of lines analyzed.
    /// </para>
    /// <para>
    /// The field delimiters COMMA (<c>','</c>, %x2C), SEMICOLON  (<c>';'</c>, %x3B), 
    /// HASH (<c>'#'</c>, %x23), TAB (<c>'\t'</c>, %x09), and SPACE (<c>' '</c>, %x20) are recognized 
    /// automatically.
    /// </para>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="csv" />, or 
    /// <paramref name="mapping"/>, or <paramref name="conversion"/> is <c>null</c>.</exception>
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult[] ParseAnalyzed<TResult>(string csv,
                                                   CsvMapping mapping,
                                                   Func<dynamic, TResult> conversion,
                                                   Header header = Header.ProbablyPresent,
                                                   int analyzedLines = CsvAnalyzer.AnalyzedLinesMinCount)
        => ParseAnalyzed(csv, new CsvToIntl<TResult>(mapping, conversion), header, analyzedLines);

    /// <summary>
    /// Parses a CSV-<see cref="string"/> after it had been analyzed.
    /// </summary>
    ///  <typeparam name="TResult"> Generic type parameter that specifies the <see cref="Type"/>
    /// of the items in the array that the method returns.</typeparam>
    /// <param name="csv">The CSV-<see cref="string"/> to parse.</param>
    /// <param name="converter">
    /// An object that converts a CSV row to a <typeparamref name="TResult"/> instance.
    /// </param>
    /// <param name="header">A supposition that is made about the presence of a header row.</param>
    /// 
    /// <param name="analyzedLines">Maximum number of lines to analyze in <paramref name="csv"/>. The
    /// minimum value is <see cref="CsvAnalyzer.AnalyzedLinesMinCount" />. If <paramref name="csv"/> 
    /// has fewer lines than <paramref name="analyzedLines" />, it will be analyzed completely. (You 
    /// can specify <see cref="int.MaxValue">Int32.MaxValue</see> to analyze the entire 
    /// <see cref="string"/> in any case.)</param>
    /// 
    /// <returns>An array of <typeparamref name="TResult"/> instances, initialized from the parsed 
    /// <paramref name="csv"/>.</returns>
    /// 
    /// <remarks>
    /// <para>
    /// <see cref="CsvAnalyzer" /> performs a statistical analysis on <paramref name="csv"/>. The 
    /// result of the analysis is therefore always only an estimate, the accuracy of which increases 
    /// with the number of lines analyzed.
    /// </para>
    /// <para>
    /// The field delimiters COMMA (<c>','</c>, %x2C), SEMICOLON  (<c>';'</c>, %x3B), 
    /// HASH (<c>'#'</c>, %x23), TAB (<c>'\t'</c>, %x09), and SPACE (<c>' '</c>, %x20) are recognized 
    /// automatically.
    /// </para>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="csv" />, or 
    /// <paramref name="converter"/> is <c>null</c>.</exception>
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
                                                   CsvTo<TResult> converter,
                                                   Header header = Header.ProbablyPresent,
                                                   int analyzedLines = CsvAnalyzer.AnalyzedLinesMinCount)
    {
        CsvAnalyzerResult analyzerResult = CsvAnalyzer.AnalyzeString(csv, header, analyzedLines);
        CsvOpts options = analyzerResult.Options;
        bool cloneMapping = MustMappingBeCloned<TResult>(ref options);

        using var stringReader = new StringReader(csv);
        using var csvReader = new CsvReader(stringReader,
                                            analyzerResult.Delimiter,
                                            analyzerResult.IsHeaderPresent,
                                            options);
        using var typeReader = new CsvReader<TResult>(csvReader, converter, cloneMapping);

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
    /// It's recommended to initialize <paramref name="reader"/> with the 
    /// <see cref="CsvOpts.DisableCaching"/> flag set.
    /// </para>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="dataTable"/>, or 
    /// <paramref name="reader"/>, or <paramref name="mapping"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// There is a <see cref="DynamicProperty"/> in <paramref name="mapping"/> whose 
    /// <see cref="DynamicProperty.PropertyName"/> finds no corresponding 
    /// <see cref="DataColumn.ColumnName"/> in <paramref name="dataTable"/>.
    /// </exception>
    /// <exception cref="FormatException">Parsing fails and <see cref="ITypeConverter{T}.Throwing"/>
    /// is <c>true</c>.</exception>
    /// <exception cref="NoNullAllowedException">The <paramref name="mapping"/> doesn't match the 
    /// schema of the <paramref name="dataTable"/>.</exception>
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
    /// <param name="delimiter">The field separator character.</param>
    /// <param name="textEncoding">The text encoding to be used to read the CSV file
    /// or <c>null</c> for <see cref="Encoding.UTF8" />.</param>
    /// <param name="isHeaderPresent"> <c>true</c>, to interpret the first line as a header, 
    /// otherwise <c>false</c>.</param>
    /// <param name="options">Options for reading the CSV file.</param>
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
    /// The <see cref="DynamicProperty"/> instances in <paramref name="mapping"/> don't need to 
    /// match all columns of the <see cref="DataTable"/> or all columns of the CSV file (neither 
    /// in number nor in order).
    /// </para>
    /// <para>
    /// When importing CSV data from Excel, the appropriate arguments can be determined 
    /// with <see cref="Csv.GetExcelArguments"/>.
    /// </para>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="dataTable"/>, or 
    /// <paramref name="filePath"/>, or <paramref name="mapping"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <para><paramref name="filePath" /> is not a valid file path.</para>
    /// <para>- or -</para>
    /// <para>
    /// There is a <see cref="DynamicProperty"/> in <paramref name="mapping"/> whose 
    /// <see cref="DynamicProperty.PropertyName"/> finds no corresponding 
    /// <see cref="DataColumn.ColumnName"/> in <paramref name="dataTable"/>.
    /// </para>
    /// </exception>
    /// <exception cref="FormatException">Parsing fails and <see cref="ITypeConverter{T}.Throwing"/> 
    /// is <c>true</c>.</exception>
    /// <exception cref="NoNullAllowedException">The <paramref name="mapping"/> doesn't match the 
    /// schema of the <paramref name="dataTable"/>.</exception>
    /// <exception cref="ConstraintException">The parsed CSV data does not match the schema of
    /// the <paramref name="dataTable"/>.</exception>
    /// <exception cref="IOException">I/O error.</exception>
    /// <exception cref="ObjectDisposedException">The file was already closed.</exception>
    public static void Fill(DataTable dataTable,
                            string filePath,
                            CsvMapping mapping,
                            char delimiter = ',',
                            Encoding? textEncoding = null,
                            bool isHeaderPresent = true,
                            CsvOpts options = CsvOpts.Default)
    {
        using var reader = new CsvReader(filePath,
                                         delimiter,
                                         textEncoding,
                                         isHeaderPresent,
                                         options | CsvOpts.DisableCaching);
        Fill(dataTable, reader, mapping);
    }

    /// <summary>
    /// Adds the content of a CSV file as <see cref="DataRow"/>s to a <see cref="DataTable"/>
    /// after the file had been analyzed.
    /// </summary>
    /// <param name="dataTable">The <see cref="DataTable"/> to which <see cref="DataRow"/>s
    /// are added.</param>
    /// <param name="filePath">File path of the CSV file.</param>
    /// <param name="mapping">The <see cref="CsvMapping"/> to be used.</param>
    /// <param name="defaultEncoding">
    /// The text <see cref="Encoding"/> to be used if the CSV file has no byte order mark (BOM), or
    /// <c>null</c> to use <see cref="Encoding.UTF8"/> in this case. Use <see cref="Csv.GetExcelArguments"/>
    /// to get the appropriate argument for this parameter when importing CSV data from Excel.
    /// </param>
    /// <param name="header">A supposition that is made about the presence of a header row.</param>
    /// <param name="analyzedLines">Maximum number of lines to analyze in the CSV file. The minimum 
    /// value is <see cref="CsvAnalyzer.AnalyzedLinesMinCount" />. If the file has fewer lines than 
    /// <paramref name="analyzedLines" />, it will be analyzed completely. (You can specify 
    /// <see cref="int.MaxValue">Int32.MaxValue</see> to analyze the entire file in any case.)
    /// </param>
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
    /// The <see cref="DynamicProperty"/> instances in <paramref name="mapping"/> don't need to 
    /// match all columns of the <see cref="DataTable"/> or all columns of the CSV file (neither in
    /// number nor in order).
    /// </para>
    /// <para>
    /// The method performs a statistical analysis on the CSV file to find the appropriate 
    /// parameters for reading the file. The result of the analysis is therefore always only an 
    /// estimate, the accuracy of which increases with the number of lines analyzed. The analysis 
    /// is time-consuming because the CSV file has to be accessed for reading.
    /// </para>
    /// <para>
    /// The field delimiters COMMA (<c>','</c>, %x2C), SEMICOLON  (<c>';'</c>, %x3B), 
    /// HASH (<c>'#'</c>, %x23), TAB (<c>'\t'</c>, %x09), and SPACE (<c>' '</c>, %x20) are recognized
    /// automatically.
    /// </para>
    /// <para>
    /// This method also tries to determine the <see cref="Encoding"/> of the CSV file from the
    /// byte order mark (BOM). If no byte order mark can be found, <paramref name="defaultEncoding"/> 
    /// is used.
    /// </para>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="dataTable"/>, or 
    /// <paramref name="filePath"/>, or <paramref name="mapping"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <para><paramref name="filePath" /> is not a valid file path.</para>
    /// <para>- or -</para>
    /// <para>
    /// There is a <see cref="DynamicProperty"/> in <paramref name="mapping"/> whose 
    /// <see cref="DynamicProperty.PropertyName"/> finds no corresponding 
    /// <see cref="DataColumn.ColumnName"/> in <paramref name="dataTable"/>.
    /// </para>
    /// </exception>
    /// <exception cref="FormatException">Parsing fails and 
    /// <see cref="ITypeConverter{T}.Throwing"/> is <c>true</c>.</exception>
    /// <exception cref="NoNullAllowedException">The <paramref name="mapping"/> doesn't match 
    /// the schema of the <paramref name="dataTable"/>.</exception>
    /// <exception cref="ConstraintException">The parsed CSV data does not match the schema of
    /// the <paramref name="dataTable"/>.</exception>
    /// <exception cref="IOException">I/O error.</exception>
    /// <exception cref="ObjectDisposedException">The file was already closed.</exception>
    public static void FillAnalyzed(DataTable dataTable,
                                    string filePath,
                                    CsvMapping mapping,
                                    Encoding? defaultEncoding = null,
                                    Header header = Header.ProbablyPresent,
                                    int analyzedLines = CsvAnalyzer.AnalyzedLinesMinCount)
    {
        using CsvReader reader = Csv.OpenReadAnalyzed(
            filePath, defaultEncoding, header, disableCaching: true, analyzedLines);
        Fill(dataTable, reader, mapping);
    }

    private static string ToCsvIntl<TData>(IEnumerable<TData?> data,
                                           ToCsv<TData> converter,
                                           char delimiter,
                                           IReadOnlyCollection<string?>? columnNames)
    {
        using var stringWriter = new StringWriter();
        using CsvWriter csvWriter = Csv.OpenWrite(
            stringWriter, GetColumnNames(columnNames, converter.Mapping), delimiter);

        Write(data, csvWriter, converter);

        return stringWriter.ToString();
    }

    private static IReadOnlyCollection<string?> GetColumnNames(IReadOnlyCollection<string?>? columnNames,
                                                               CsvMapping mapping)
    {
        if (columnNames is null)
        {
            _ArgumentNullException.ThrowIfNull(mapping, nameof(mapping));
            columnNames = [.. mapping.PropertyNames];
        }

        return columnNames;
    }

    private static void WriteIntl(DataTable dataTable, CsvWriter writer, CsvMapping mapping)
    {
        mapping.Record = writer.Record;

        Dictionary<string, string> captionDictionary
            = DataTableHelper.CreateCaptionDictionary(dataTable);

        DataRowCollection rows = dataTable.Rows;

        for (int i = 0; i < rows.Count; i++)
        {
            DataRow row = rows[i];

            if (row.RowState != DataRowState.Deleted)
            {
                mapping.FillWith(row, captionDictionary);
                writer.WriteRecord();
            }
        }
    }

    private static bool MustMappingBeCloned<TResult>(ref CsvOpts options)
    {
        if (options.HasFlag(CsvOpts.DisableCaching))
        {
            return false;
        }

        Type resultType = typeof(TResult);

        if (resultType == _mappingType || resultType == _dynamicType)
        {
            return true;
        }

        if (resultType != _recordType)
        {
            options |= CsvOpts.DisableCaching;
        }

        return false;
    }
}
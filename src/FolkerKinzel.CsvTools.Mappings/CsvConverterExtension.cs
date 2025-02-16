using FolkerKinzel.CsvTools.Mappings.Intls;
using System.Text;

namespace FolkerKinzel.CsvTools.Mappings;

/// <summary>
/// Extension methods for converting collections of any data type to CSV.
/// </summary>
public static class CsvConverterExtension
{
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
    public static string ToCsv<TData>(this IEnumerable<TData?> data,
                                      CsvMapping mapping,
                                      Action<TData, dynamic> conversion,
                                      char delimiter = ',',
                                      IReadOnlyCollection<string?>? columnNames = null)
        => CsvConverter.ToCsv(data, mapping, conversion, delimiter, columnNames);

    /// <summary>
    /// Converts a collection of <typeparamref name="TData"/> instances to a CSV 
    /// <see cref="string"/> without a header row.
    /// </summary>
    /// <typeparam name="TData">
    /// Generic type parameter for the data type to write as CSV row.
    /// </typeparam>
    /// <param name="data">The data to convert to CSV. Each item will be represented 
    /// with a CSV row.</param>
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
    /// 
    /// <returns>A CSV <see cref="string"/> without header row that contains the contents of 
    /// <paramref name="data"/>.</returns>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="data"/>, or 
    /// <paramref name="mapping"/>, or <paramref name="conversion"/> is <c>null</c>.</exception>
    /// <exception cref="IOException">I/O error.</exception>
    /// <exception cref="ObjectDisposedException">The file was already closed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToCsv<TData>(this IEnumerable<TData?> data,
                                      int columnsCount,
                                      CsvMapping mapping,
                                      Action<TData, dynamic> conversion,
                                      char delimiter = ',')
        => CsvConverter.ToCsv(data, columnsCount, mapping, conversion, delimiter);

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
    /// <example>
    /// <para>Object serialization with CSV:</para>
    /// <code language="cs" source="..\Examples\ObjectSerializationExample.cs"/>
    /// </example>
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
    public static void SaveCsv<TData>(this IEnumerable<TData?> data,
                                      string filePath,
                                      CsvMapping mapping,
                                      Action<TData, dynamic> conversion,
                                      char delimiter = ',',
                                      IReadOnlyCollection<string?>? columnNames = null,
                                      Encoding? textEncoding = null)
        => CsvConverter.Save(
            data, filePath, mapping, conversion, delimiter, columnNames, textEncoding);

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
    /// is passed to the method as <c>dynamic</c> argument: Inside the 
    /// <paramref name="conversion"/> method the registered 
    /// <see cref="DynamicProperty"/> instances can be used like regular .NET properties, 
    /// but without IntelliSense ("late binding").
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
    /// <exception cref="ObjectDisposedException">The file was already closed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SaveCsv<TData>(this IEnumerable<TData?> data,
                                      string filePath,
                                      int columnsCount,
                                      CsvMapping mapping,
                                      Action<TData, dynamic> conversion,
                                      char delimiter = ',',
                                      Encoding? textEncoding = null)
        => CsvConverter.Save(
            data, filePath, columnsCount, mapping, conversion, delimiter, textEncoding);
}

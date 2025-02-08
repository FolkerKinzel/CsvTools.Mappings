using FolkerKinzel.CsvTools.Mappings.TypeConverters;
using FolkerKinzel.CsvTools.Mappings.TypeConverters.Interfaces;
using FolkerKinzel.CsvTools.Mappings.Intls;
using System.Data;
using System.Text;

namespace FolkerKinzel.CsvTools.Mappings;

/// <summary>
/// Extension methods for the <see cref="DataTable"/> class.
/// </summary>
public static class DataTableExtension
{
    /// <summary>
    /// Adds CSV content as <see cref="DataRow"/>s to a <see cref="DataTable"/>.
    /// </summary>
    /// <param name="dataTable">The <see cref="DataTable"/> to which <see cref="DataRow"/>s
    /// are added.</param>
    /// <param name="reader">The <see cref="CsvReader"/> to use.</param>
    /// <param name="mapping">The <see cref="CsvRecordMapping"/> to be used.</param>
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
    public static void ReadCsv(this DataTable dataTable, CsvReader reader, CsvRecordMapping mapping)
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
    /// <param name="mapping">The <see cref="CsvRecordMapping"/> to be used.</param>
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
    public static void ReadCsv(this DataTable dataTable,
                               string filePath,
                               CsvRecordMapping mapping,
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
        dataTable.ReadCsv(reader, mapping);
    }

    /// <summary>
    /// Adds the content of a CSV file as <see cref="DataRow"/>s to a <see cref="DataTable"/>
    /// after the CSV file had been analyzed.
    /// </summary>
    /// <param name="dataTable">The <see cref="DataTable"/> to which <see cref="DataRow"/>s
    /// are added.</param>
    /// <param name="filePath">File path of the CSV file.</param>
    /// <param name="mapping">The <see cref="CsvRecordMapping"/> to be used.</param>
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
    public static void ReadCsvAnalyzed(this DataTable dataTable,
                                       string filePath,
                                       CsvRecordMapping mapping,
                                       Header header = Header.ProbablyPresent,
                                       Encoding? textEncoding = null,
                                       int analyzedLines = CsvAnalyzer.AnalyzedLinesMinCount)
    {
        using CsvReader reader = Csv.OpenReadAnalyzed(filePath, header, textEncoding, analyzedLines, true);
        dataTable.ReadCsv(reader, mapping);
    }

    /// <summary>
    /// Writes the content of a <see cref="DataTable"/> as CSV file with header.
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
    /// <param name="mapping">The <see cref="CsvRecordMapping"/> to be used.</param>
    /// 
    /// <remarks>
    /// <para>Creates a new CSV file. If the target file already exists, it is 
    /// truncated and overwritten.
    /// </para>
    /// <para>
    /// This method initializes a <see cref="CsvWriter"/> instance that uses the comma ',' (%x2C) as field delimiter.
    /// This complies with the RFC 4180 standard. If another delimiter is required, use <see cref="WriteCsv(DataTable, CsvWriter, CsvRecordMapping)"/>
    /// instead.
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
    public static void WriteCsv(this DataTable dataTable,
                                string filePath,
                                IReadOnlyCollection<string?> columnNames,
                                CsvRecordMapping mapping)
    {
        using CsvWriter writer = Csv.OpenWrite(filePath, columnNames);
        dataTable.WriteCsv(writer, mapping);
    }

    /// <summary>
    /// Writes the content of a <see cref="DataTable"/> as CSV.
    /// </summary>
    /// <param name="dataTable">The <see cref="DataTable"/> whose content is written.</param>
    /// <param name="writer">The <see cref="CsvWriter"/> to be used.</param>
    /// <param name="mapping">The <see cref="CsvRecordMapping"/> to be used.</param>
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
    public static void WriteCsv(this DataTable dataTable, CsvWriter writer, CsvRecordMapping mapping)
    {
        _ArgumentNullException.ThrowIfNull(dataTable, nameof(dataTable));
        _ArgumentNullException.ThrowIfNull(writer, nameof(writer));
        _ArgumentNullException.ThrowIfNull(mapping, nameof(mapping));

        mapping.Record = writer.Record;

        Dictionary<string, string> captionDictionary = DataTableHelper.CreateCaptionDictionary(dataTable);

        DataRowCollection rows = dataTable.Rows;

        for (int i = 0; i < rows.Count; i++)
        {
            mapping.FillWithIntl(rows[i], captionDictionary);
            writer.WriteRecord();
        }
    }
}
﻿using FolkerKinzel.CsvTools.Mappings.TypeConverters;
using FolkerKinzel.CsvTools.Mappings.TypeConverters.Interfaces;
using System.Data;
using System.Text;

namespace FolkerKinzel.CsvTools.Mappings;

/// <summary>
/// Extension methods for the <see cref="DataTable"/> class.
/// </summary>
public static class DataTableExtension
{
    /// <summary>
    /// Adds CSV content as <see cref="DataRow"/>s to the <see cref="DataTable"/>.
    /// </summary>
    /// <param name="dataTable">The <see cref="DataTable"/> to which 
    /// <see cref="DataRow"/>s are added.</param>
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
    /// The <see cref="DynamicProperty"/> instances in <paramref name="mapping"/> don't need
    /// to match all columns of the <see cref="DataTable"/> or all columns of the CSV file 
    /// (neither in number nor in order).
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
    /// <exception cref="FormatException">Parsing fails and 
    /// <see cref="ITypeConverter{T}.Throwing"/> is <c>true</c>.</exception>
    /// <exception cref="NoNullAllowedException">The <paramref name="mapping"/> doesn't 
    /// match the schema of the <paramref name="dataTable"/>.</exception>
    /// <exception cref="ConstraintException">The parsed CSV data does not match the 
    /// schema of the <paramref name="dataTable"/>.</exception>
    /// <exception cref="IOException">I/O error.</exception>
    /// <exception cref="ObjectDisposedException">The file was already closed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReadCsv(this DataTable dataTable, CsvReader reader, CsvMapping mapping)
        => CsvConverter.Fill(dataTable, reader, mapping);

    /// <summary>
    /// Adds the content of a CSV file as <see cref="DataRow"/>s to the <see cref="DataTable"/>.
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
    /// <example>
    /// <note type="note">In the following code examples - for easier readability - exception 
    /// handling has been omitted.</note>
    /// <para>DataTable serialization with CSV:</para>
    /// <code language="cs" source="..\Examples\DataTableExample.cs"/>
    /// </example>
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
    /// <exception cref="NoNullAllowedException">The <paramref name="mapping"/> doesn't 
    /// match the schema of the <paramref name="dataTable"/>.</exception>
    /// <exception cref="ConstraintException">The parsed CSV data does not match the schema of
    /// the <paramref name="dataTable"/>.</exception>
    /// <exception cref="IOException">I/O error.</exception>
    /// <exception cref="ObjectDisposedException">The file was already closed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReadCsv(this DataTable dataTable,
                               string filePath,
                               CsvMapping mapping,
                               char delimiter = ',',
                               Encoding? textEncoding = null,
                               bool isHeaderPresent = true,
                               CsvOpts options = CsvOpts.Default)
        => CsvConverter.Fill(
            dataTable, filePath, mapping, delimiter, textEncoding, isHeaderPresent, options);

    /// <summary>
    /// Adds the content of a CSV file as <see cref="DataRow"/>s to the <see cref="DataTable"/>
    /// after the file had been analyzed.
    /// </summary>
    /// <param name="dataTable">The <see cref="DataTable"/> to which <see cref="DataRow"/>s
    /// are added.</param>
    /// <param name="filePath">File path of the CSV file.</param>
    /// <param name="mapping">The <see cref="CsvMapping"/> to be used.</param>
    /// <param name="defaultEncoding">
    /// The text <see cref="Encoding"/> to be used if the CSV file has no byte order mark (BOM), or 
    /// <c>null</c> to use <see cref="Encoding.UTF8"/> in this case. Use 
    /// <see cref="Csv.GetExcelArguments"/> to get the appropriate argument for this parameter when 
    /// importing CSV data from Excel.
    /// </param>
    /// <param name="header">A supposition that is made about the presence of a header row.</param>
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
    /// parameters for reading the file. The result of the analysis is therefore always only an 
    /// estimate, the accuracy of which increases with the number of lines analyzed. The analysis 
    /// is time-consuming because the CSV file has to be accessed for reading.
    /// </para>
    /// <para>
    /// The field delimiters COMMA (<c>','</c>, %x2C), SEMICOLON  (<c>';'</c>, %x3B), 
    /// HASH (<c>'#'</c>, %x23), TAB (<c>'\t'</c>, %x09), and SPACE (<c>' '</c>, %x20) are 
    /// recognized automatically.
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
    /// <exception cref="FormatException">Parsing fails and <see cref="ITypeConverter{T}.Throwing"/>
    /// is <c>true</c>.</exception>
    /// <exception cref="NoNullAllowedException">The <paramref name="mapping"/> doesn't match the
    /// schema of the <paramref name="dataTable"/>.</exception>
    /// <exception cref="ConstraintException">The parsed CSV data does not match the schema of
    /// the <paramref name="dataTable"/>.</exception>
    /// <exception cref="IOException">I/O error.</exception>
    /// <exception cref="ObjectDisposedException">The file was already closed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReadCsvAnalyzed(this DataTable dataTable,
                                       string filePath,
                                       CsvMapping mapping,
                                       Encoding? defaultEncoding = null,
                                       Header header = Header.ProbablyPresent,
                                       int analyzedLines = CsvAnalyzer.AnalyzedLinesMinCount)
        => CsvConverter.FillAnalyzed(dataTable, filePath, mapping, defaultEncoding, header, analyzedLines);

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
    /// <example>
    /// <note type="note">In the following code examples - for easier readability - exception 
    /// handling has been omitted.</note>
    /// <para>DataTable serialization with CSV:</para>
    /// <code language="cs" source="..\Examples\DataTableExample.cs"/>
    /// </example>
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
    public static void WriteCsv(this DataTable dataTable,
                                string filePath,
                                CsvMapping mapping,
                                char delimiter = ',',
                                Encoding? textEncoding = null,
                                IEnumerable<string?>? csvColumnNames = null)
        => CsvConverter.Save(
            dataTable, filePath, mapping, delimiter, textEncoding, csvColumnNames);

    /// <summary>
    /// Writes the content of the <see cref="DataTable"/> as CSV.
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
    /// The <see cref="DynamicProperty"/> instances in <paramref name="mapping"/> don't need 
    /// to match all columns of the <see cref="DataTable"/> or all columns of the CSV file 
    /// (neither in number nor in order).
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteCsv(this DataTable dataTable, CsvWriter writer, CsvMapping mapping)
        => CsvConverter.Write(dataTable, writer, mapping);
}
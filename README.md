# FolkerKinzel.CsvTools.Mappings
[![NuGet](https://img.shields.io/nuget/v/FolkerKinzel.CsvTools.Mappings)](https://www.nuget.org/packages/FolkerKinzel.CsvTools.Mappings/)
[![GitHub](https://img.shields.io/github/license/FolkerKinzel/CsvTools.Mappings)](https://github.com/FolkerKinzel/CsvTools.Mappings/blob/master/LICENSE)
[![Stand With Ukraine](https://raw.githubusercontent.com/vshymanskyy/StandWithUkraine/main/badges/StandWithUkraine.svg)](https://stand-with-ukraine.pp.ua)

## Mappings and Type Conversions for FolkerKinzel.CsvTools (RFC 4180)
[Project Reference](https://folkerkinzel.github.io/CsvTools.Mappings/reference/)

This package allows:
- analyzing CSV files and CSV strings (delimiter, header row, column names, text encoding, and required options for reading non-standard CSV)
- retrieving the appropriate method arguments for exchanging CSV data with Excel
- serializing and deserializing collections of any data type 
- serializing and deserializing DataTables
- to do things easily and with just a few lines of code. In addition, the package also provides the means to write high-performance code:
	- It allows you to parse ultra large CSV files because only one row of the file has to be in memory at a time.
	- It makes the fields of the CSV file available as `ReadOnlyMemory<Char>` instances. This avoids the allocation of numerous temporary strings.
	- It allows to parse value types without any boxing and unboxing and (when using a modern .NET version) without any allocation of temporary strings.
	- The `CsvOpts.DisableCaching` option allows reusing the same `CsvRecord` instance for each parsed row of the CSV file. This can avoid further allocations.

(See [benchmark results](https://github.com/FolkerKinzel/CsvTools.Mappings/tree/master/src/Benchmarks/results) and the associated [benchmark code](https://github.com/FolkerKinzel/CsvTools.Mappings/tree/master/src/Benchmarks).)
### Code Examples
- [Object serialization with CSV](https://github.com/FolkerKinzel/CsvTools.Mappings/blob/master/src/Examples/ObjectSerializationExample.cs)
- [Serializing and deserializing DataTables with CSV](https://github.com/FolkerKinzel/CsvTools.Mappings/blob/master/src/Examples/DataTableExample.cs)
- [CSV data exchange with Excel](https://github.com/FolkerKinzel/CsvTools.Mappings/blob/master/src/Examples/ExcelExample.cs)
- [High performance CSV parsing](https://github.com/FolkerKinzel/CsvTools.Mappings/blob/master/src/Benchmarks/CalculationReader_Performance.cs)
- [Fastest way to write CSV](https://github.com/FolkerKinzel/CsvTools.Mappings/blob/master/src/Benchmarks/CalculationWriter_Performance.cs)
- [Using the CsvOpts.DisableCaching option](https://github.com/FolkerKinzel/CsvTools/blob/master/src/Examples/DisableCachingExample.cs)
- [Writing an own TypeConverter&lt;T&gt;](https://github.com/FolkerKinzel/CsvTools.Mappings/blob/master/src/Examples/Int128Converter.cs)
- [CSV serialization with MultiColumnTypeConverter&lt;T&gt;](https://github.com/FolkerKinzel/CsvTools.Mappings/blob/master/src/Examples/MultiColumnConverterExample.cs)

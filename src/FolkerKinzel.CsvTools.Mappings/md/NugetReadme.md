[![GitHub](https://img.shields.io/github/license/FolkerKinzel/CsvTools.Mappings)](https://github.com/FolkerKinzel/CsvTools.Mappings/blob/master/LICENSE)

## Mappings and Type Conversions for FolkerKinzel.CsvTools (RFC 4180)
([FolkerKinzel.CsvTools](https://www.nuget.org/packages/FolkerKinzel.CsvTools) comes with the dependencies and doesn't need to be installed separately.)

This package allows:
- analyzing CSV files and CSV strings (delimiter, header row, column names, text encoding, and required options for reading non-standard CSV)
- serializing and deserializing collections of any data type 
- serializing and deserializing DataTables
- to do things easily and with just a few lines of code. In addition, the package also provides the means to write high-performance code:
	- It allows you to parse ultra large CSV files because only one row of the file has to be in memory at a time.
	- It makes the fields of the CSV file available as `ReadOnlyMemory<Char>` instances. This avoids the allocation of numerous temporary strings.
	- It allows to parse value types without any boxing and unboxing and - with modern .NET versions - without any allocation of temporary strings.
	- The `CsvOpts.DisableCaching` option allows reusing the same `CsvRecord` instance for each parsed row of the CSV file. This can avoid further allocations.

[Project Reference and Release Notes](https://github.com/FolkerKinzel/CsvTools.Mappings/releases/tag/v1.0.0-beta.1)

[See code examples on GitHub](https://github.com/FolkerKinzel/CsvTools.Mappings)

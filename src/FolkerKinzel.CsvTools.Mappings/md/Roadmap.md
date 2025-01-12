# FolkerKinzel.CsvTools
## Roadmap

### 1.0.0.beta.1
- [x] Add .NET 9.0 version of the package
- [ ] Write tests
- [ ] Write benchmarks
- [ ] Write examples

### 1.0.0-alpha.2
- [x] Dependency update
- [x] Add an abstract `IEnumerable<int> DynamicProperty.AccessedCsvColumnIndexes { get; }` property.
- [x] Add an abstract `IEnumerable<string> DynamicProperty.AccessedCsvColumnNames { get; }` property.

### 1.0.0-alpha.1
- [x] Add a DateOnlyConverter.
- [x] Add a TimeOnlyConverter.
- [x] Add a UriConverter.
- [x] Add a VersionConverter.
- [x] Add a `AllowsNull` property to `CsvTypeConverter<T>`.
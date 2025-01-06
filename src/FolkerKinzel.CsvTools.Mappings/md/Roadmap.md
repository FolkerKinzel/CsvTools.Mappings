# FolkerKinzel.CsvTools
## Roadmap

### 1.0.0.beta.1
- [ ] Write tests
- [ ] Write benchmarks
- [ ] Write examples

### 1.0.0-alpha.2
- [ ] Add an abstract `IReadOnlyList<int> DynamicProperty.AccessedCsvColumnIndexes()` method.
- [ ] Add an abstract `IReadOnlyList<string> DynamicProperty.AccessedCsvColumnNames()` method.

### 1.0.0-alpha.1
- [x] Add a DateOnlyConverter.
- [x] Add a TimeOnlyConverter.
- [x] Add a UriConverter.
- [x] Add a VersionConverter.
- [x] Add a `AllowsNull` property to `CsvTypeConverter<T>`.
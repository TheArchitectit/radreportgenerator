# Design: Contracts

```csharp
public interface IDataParser {
  bool CanParse(string filePath);
  IReportData Parse(string filePath); // or Task async later
}
public interface IReportGenerator {
  string Format { get; }
  void Generate(IReportData data, string outputPath);
}
public interface IChartProvider { ... }
public interface IAnalysisService { Task<string> AnalyzeAsync(...); }
```
ServiceConfiguration.AddOpenReportViewer(services).

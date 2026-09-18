# Design: Null contract
```
if (project is null) throw new ArgumentNullException(nameof(project));
if (string.IsNullOrWhiteSpace(outputPath)) throw new ArgumentException(...);
```
Update ReportGeneratorServiceTests.GeneratePresentation_WithNullProject_* accordingly.

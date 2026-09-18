# Design: Module boundaries

```
OpenReportViewer.Core        # models, interfaces, no IO deps if possible
OpenReportViewer.Parsers     # LiveOpticsXlsxParser, future RVToolsParser
OpenReportViewer.Reporting   # ReportGeneratorService, future QuestPdf
OpenReportViewer.AI          # ResearchAgentService / Copilot later
OpenReportViewer.UI.Wpf      # presentation
OpenReportViewer.Tests       # unit tests
```
DI: ServiceConfiguration extension in Core or Composition project.

# Proposal: Drive charts from parsed performance data (QA-08, QA-09)

## Intent
Dashboard charts always show hardcoded dummy series. Users cannot trust the dashboard. Parser performance path is also a stub.

## Scope
In: LiveOpticsXlsxParser.ParsePerformanceData, MainViewModel.UpdateCharts, model binding to PerformanceProfile.
Out: new chart types beyond current line charts.

## Approach
1. Implement performance parsing for known Live Optics sheet patterns (best-effort + metadata).
2. Aggregate IoHistory/CpuHistory onto servers/project.
3. Bind LiveCharts series from real MetricPoint lists; show empty state when no data instead of fake series.

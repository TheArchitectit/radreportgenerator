# Sprint 5 — Observability, chart providers, HTML reports

**Goal:** Structured logging/debug traces, shared chart provider system used by UI + PDF, and HTML report output via the factory.  
**Depends on:** Sprint 3 (Reporting) and Sprint 4 (real data)  
**Duration:** 2 weeks

## OpenSpec changes

| Change | Purpose |
|--------|---------|
| [`debug-infrastructure`](../changes/debug-infrastructure/) | Serilog + IDebugService |
| [`chart-provider-system`](../changes/chart-provider-system/) | IChartProvider + shared ChartData |
| [`multi-format-reports`](../changes/multi-format-reports/) | HTML generator complete |
| [`core-interfaces-and-di`](../changes/core-interfaces-and-di/) | IChartProvider registration |

## Files in scope

| File | Action |
|------|--------|
| **New** `src/OpenReportViewer.Core/Interfaces/IDebugService.cs`, `IChartProvider.cs` | Contracts |
| **New** `src/OpenReportViewer.Core/Models/ChartData.cs` | Shared chart DTO |
| **New** `src/OpenReportViewer.Visualizations/*` (or Reporting/) | Bar/line providers |
| `src/OpenReportViewer.Parsers/*` | Replace Console with ILogger |
| `src/OpenReportViewer.Reporting/HtmlReportGenerator.cs` | New |
| `src/OpenReportViewer.Reporting/IReportFactory.cs` | Include html |
| `src/OpenReportViewer.Reporting/QuestPdfReportGenerator.cs` | Embed chart images |
| `src/OpenReportViewer.UI.Wpf/ViewModels/MainViewModel.cs` | ChartData binding |
| `src/OpenReportViewer.UI.Wpf/MainWindow.xaml` | Optional debug/status detail |
| `src/OpenReportViewer.UI.Wpf/App.xaml.cs` | Register debug + chart services |
| **New** logs/ config path | Rolling files (gitignored) |
| `.gitignore` | logs/ |
| Tests under `src/OpenReportViewer.Tests/**` | Chart transform, HTML, debug info |
| `docs/OpenReportViewer-Development-Plan.md` | Update status when shipping |
| `docs/OPENREPORTVIEWER-EXECUTIVE-SUMMARY.md` | Update phase progress |

## Tasks

### 1. Debug/logging
- [ ] 1.1 Serilog packages + config
- [ ] 1.2 IDebugService timings for parse/generate
- [ ] 1.3 Remove Console.WriteLine from parser
- [ ] 1.4 gitignore logs/

### 2. Chart providers
- [ ] 2.1 ChartData model
- [ ] 2.2 Bar + line providers
- [ ] 2.3 WPF adapter
- [ ] 2.4 PDF chart embedding
- [ ] 2.5 Unit tests for transforms

### 3. HTML + factory
- [ ] 3.1 HtmlReportGenerator self-contained file
- [ ] 3.2 Factory formats: pdf, pptx, html
- [ ] 3.3 UI format picker includes HTML
- [ ] 3.4 Tests for each format non-empty output

### 4. Verify
- [ ] 4.1 Full `dotnet test`
- [ ] 4.2 Log file created on parse
- [ ] 4.3 HTML opens in browser with tables/charts
- [ ] 4.4 Update sprint README exit notes

## Exit criteria

- Parse/generate leave structured logs
- One ChartData source feeds UI + PDF
- User can export HTML/PDF/PPTX from same loaded project
- FILE-INVENTORY and docs status accurate

## Out of scope for Sprint 5 (later OpenSpec candidates)

- ASP.NET Core API + OpenAPI
- React SPA
- JWT auth / multi-tenancy / K8s
- Real LLM provider implementation
- ML anomaly detection

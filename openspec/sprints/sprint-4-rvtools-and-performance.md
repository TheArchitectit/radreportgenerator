# Sprint 4 — RVTools + Live Optics performance data

**Goal:** Parse RVTools workbooks (`vInfo`/`vHost`/`vPartition`) and populate Live Optics performance series so charts and reports use real metrics.  
**Depends on:** Sprint 2 (Parsers module + fixtures)  
**Duration:** 2 weeks

## OpenSpec changes

| Change | Purpose |
|--------|---------|
| [`rvtools-parser`](../changes/rvtools-parser/) | RVTools multi-sheet parse |
| [`liveoptics-performance-data`](../changes/liveoptics-performance-data/) | Performance series + peaks |
| [`qa-fix-dummy-charts`](../changes/qa-fix-dummy-charts/) | Bind UI to parsed series |
| [`qa-fix-test-fixtures`](../changes/qa-fix-test-fixtures/) | Use `docs/*.xlsx` when present |

## Files in scope

| File | Action |
|------|--------|
| `analyze_rvtools.py`, `analyze_excel.py` | Reference for column maps; keep in repo |
| `docs/SizingWorkshop-RVTools.xlsx` | Primary RVTools fixture (optional in CI) |
| `docs/Optical-Prime-*.xlsx`, `docs/PowerProtect-DM-*.xlsx` | Schema references |
| **New** `src/OpenReportViewer.Parsers/RVTools/RVToolsParser.cs` | Implementation |
| **New** `src/OpenReportViewer.Parsers/RVTools/Models/*.cs` | Row DTOs if needed |
| `src/OpenReportViewer.Parsers/LiveOptics/LiveOpticsXlsxParser.cs` | Performance parse |
| `src/OpenReportViewer.Core/Models/Entities.cs` | Extend for VMs/hosts or IReportData impl |
| `src/OpenReportViewer.Core/Interfaces/IDataParser.cs` | CanParse + Parse |
| `src/OpenReportViewer.UI.Wpf/ViewModels/MainViewModel.cs` | Parser factory by file type; real charts |
| `src/OpenReportViewer.UI.Wpf/MainWindow.xaml` | File filter xlsx; chart titles |
| `src/OpenReportViewer.Tests/Services/LiveOpticsXlsxParserTests.cs` | Performance cases |
| **New** `src/OpenReportViewer.Tests/Parsers/RVToolsParserTests.cs` | RVTools cases |
| `src/OpenReportViewer.Tests/TestFixtures/*` | Sample path helpers |

## Tasks

### 1. RVTools parser
- [ ] 1.1 Column map from analyze scripts + SizingWorkshop sample
- [ ] 1.2 CanParse sheet detection
- [ ] 1.3 vInfo → VM inventory
- [ ] 1.4 vPartition → disk usage
- [ ] 1.5 vHost → ESXi hosts
- [ ] 1.6 Aggregates (counts, CPU, memory, storage)
- [ ] 1.7 Tests skip-if-missing sample

### 2. Live Optics performance
- [ ] 2.1 Sheet layout documentation
- [ ] 2.2 Timestamp series parse → IoHistory
- [ ] 2.3 Peak IOPS/throughput/latency
- [ ] 2.4 Synthetic xlsx unit tests

### 3. UI + reports
- [ ] 3.1 Auto-detect parser
- [ ] 3.2 Charts from real series / empty state
- [ ] 3.3 PDF/PPTX sections include storage/host metrics when present
- [ ] 3.4 Manual verify with `docs/SizingWorkshop-RVTools.xlsx`

## Exit criteria

- RVTools sample parses (when file present) to non-zero VM/host counts
- Performance parse fills PeakIOPS when series present
- No dummy series in production UI
- `dotnet test` green

## Non-goals

- vCenter live API
- ML anomaly detection (later roadmap)
- Web dashboard

# Sprint 3 — Reporting MVP (PDF + factory seam)

**Goal:** Real PDF generation via QuestPDF in `OpenReportViewer.Reporting`, PPTX retained behind the same `IReportGenerator` contract, UI can export PDF.  
**Depends on:** Sprint 2 (Reporting project + interfaces)  
**Duration:** 1–2 weeks  
**Note:** Planning docs claimed this sprint complete — it was not. This is the first implementation.

## OpenSpec changes

| Change | Purpose |
|--------|---------|
| [`pdf-generation-questpdf`](../changes/pdf-generation-questpdf/) | QuestPDF MVP |
| [`multi-format-reports`](../changes/multi-format-reports/) | Factory seam (HTML can be Sprint 5) |
| [`qa-fix-sprint3-guide`](../changes/qa-fix-sprint3-guide/) | Use corrected template as draft |

## Files in scope

| File | Action |
|------|--------|
| **New** `src/OpenReportViewer.Reporting/OpenReportViewer.Reporting.csproj` | QuestPDF + Logging packages |
| **New** `src/OpenReportViewer.Reporting/IPdfReportGenerator.cs` | Contract + options |
| **New** `src/OpenReportViewer.Reporting/QuestPdfReportGenerator.cs` | Implementation |
| **New** `src/OpenReportViewer.Reporting/IReportFactory.cs` | Format selection |
| `src/OpenReportViewer.Reporting/ReportGeneratorService.cs` | Implement `IReportGenerator` fully |
| `src/OpenReportViewer.Tests/Services/ReportGeneratorServiceTests.cs` | Keep PPTX tests |
| **New** `src/OpenReportViewer.Tests/Reporting/PdfReportGeneratorTests.cs` | PDF file non-empty |
| `src/OpenReportViewer.UI.Wpf/ViewModels/MainViewModel.cs` | Export format (pptx/pdf) |
| `src/OpenReportViewer.UI.Wpf/MainWindow.xaml` | Save dialog filters |
| `SPRINT-3-EXECUTION-GUIDE.txt` | Reference only; status updated in Sprint 1 |
| `docs/OPENREPORTVIEWER-EXECUTIVE-SUMMARY.md` | After ship, mark Sprint 3 actually complete |
| `installer.iss`, `README.md` | Mention PDF export |

## Tasks

### 1. Reporting project
- [ ] 1.1 Create Reporting csproj; add QuestPDF
- [ ] 1.2 Move/keep PPTX generator here
- [ ] 1.3 IPdfReportGenerator + options + validation result

### 2. QuestPDF MVP
- [ ] 2.1 License setting (Community/commercial)
- [ ] 2.2 Cover page + executive summary
- [ ] 2.3 Server inventory table + totals
- [ ] 2.4 Header/footer page numbers
- [ ] 2.5 GenerateAsync → Stream; write from UI

### 3. Factory + UI
- [ ] 3.1 IReportFactory.Create("pdf"|"pptx")
- [ ] 3.2 ViewModel export command takes format
- [ ] 3.3 SaveFileDialog filters

### 4. Tests & docs
- [ ] 4.1 PDF unit test (non-empty, header bytes `%PDF`)
- [ ] 4.2 PPTX regression tests still pass
- [ ] 4.3 Update executive summary status when done
- [ ] 4.4 Performance smoke (sample project)

## Exit criteria

```
dotnet test
# PDF test produces %PDF file
# PPTX tests pass
# UI can save both formats
```

## Non-goals

- Full storage/host chart sections (follow-on change)
- HTML/D3 completeness (Sprint 5)
- API surface

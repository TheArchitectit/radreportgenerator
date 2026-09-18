# Sprint 2 — Contracts, DI, and quality of current features

**Goal:** Modular seams, dependency injection, honest AI labeling, charts that do not lie, and test fixtures that can use real samples.  
**Depends on:** Sprint 1  
**Duration:** 1–2 weeks

## OpenSpec changes

| Change | Purpose |
|--------|---------|
| [`modular-architecture`](../changes/modular-architecture/) | Parsers/Reporting/AI projects |
| [`core-interfaces-and-di`](../changes/core-interfaces-and-di/) | IDataParser, IReportGenerator, IAnalysisService |
| [`qa-fix-ui-di`](../changes/qa-fix-ui-di/) | WPF composition root |
| [`qa-fix-dummy-charts`](../changes/qa-fix-dummy-charts/) | No fake series in production UI |
| [`qa-fix-real-ai-analysis`](../changes/qa-fix-real-ai-analysis/) | Demo labeling + provider seam |
| [`qa-fix-test-fixtures`](../changes/qa-fix-test-fixtures/) | Optional sample xlsx fixtures |

## Files in scope

| File | Action |
|------|--------|
| `src/OpenReportViewer.Core/LiveOptics.Core.csproj` → slim Core | Models + interfaces only |
| `src/OpenReportViewer.Core/Models/Entities.cs` | Map toward IReportData; keep ProjectInfo for now |
| **New** `src/OpenReportViewer.Core/Interfaces/*.cs` | Contracts |
| **New** `src/OpenReportViewer.Parsers/*` | Move parser |
| **New** `src/OpenReportViewer.Reporting/*` | Move PPTX generator |
| **New** `src/OpenReportViewer.AI/*` | Move research agent |
| `src/OpenReportViewer.Core/Services/*` | Moved out; Core may keep ServiceConfiguration |
| `src/OpenReportViewer.Tests/*` | Update refs; DI tests; fixture helper |
| `src/OpenReportViewer.UI.Wpf/App.xaml.cs` | DI composition root |
| `src/OpenReportViewer.UI.Wpf/ViewModels/MainViewModel.cs` | Ctor injection; real chart data empty-state |
| `src/OpenReportViewer.UI.Wpf/MainWindow.xaml` | Demo badge; chart empty state |
| `README.md` | Honest AI feature text |
| `analyze_*.py`, `docs/*.xlsx` | Documented as parser inputs |
| Solution + all csproj | New projects referenced |

## Tasks

### 1. Modularization
- [ ] 1.1 Create Parsers, Reporting, AI classlibs
- [ ] 1.2 Move three services; fix namespaces
- [ ] 1.3 Core interfaces + ServiceConfiguration
- [ ] 1.4 Solution + references
- [ ] 1.5 Build/test green

### 2. UI quality
- [ ] 2.1 DI in App.xaml.cs + MainViewModel
- [ ] 2.2 Charts from data or empty state — remove production dummy arrays
- [ ] 2.3 AI insights labeled Demo
- [ ] 2.4 Converter still present (Sprint 0)

### 3. Tests
- [ ] 3.1 Update all project references/namespaces
- [ ] 3.2 Parser null-name contract tests
- [ ] 3.3 Report null contract tests
- [ ] 3.4 Sample fixture skip-if-missing helper
- [ ] 3.5 DI registration tests

## Exit criteria

- `dotnet test` green
- UI cannot show fabricated series
- Services resolved via DI
- FILE-INVENTORY updated for new project paths

## Non-goals

- QuestPDF implementation (Sprint 3)
- Full performance/RVTools parse (Sprint 4)
- External LLM HTTP provider (future OpenSpec)

# Full QA Review — reportgenerator

**Date:** 2026-09-18  
**Scope:** Entire repo (source, tests, docs, packaging, git hygiene)  
**Method:** Static review + `dotnet build LiveOptics.sln` + doc/code cross-check  
**OpenSpec install:** `@fission-ai/openspec` 1.13.1 @ `~/.local/openspec-cli`

## Executive summary

The repo is mid-rename from **LiveOptics** to **OpenReportViewer**. Folder names changed; solution, csproj filenames, project references, C# namespaces, and packaging scripts did not. Planning docs claim Sprint 0–3 complete (interfaces, DI, QuestPDF PDF MVP, 15+ tests, 60% coverage). **None of that code is present.** The live tree is still the original LiveOptics WPF demo: Excel parse stub, PPTX generator, mock AI, dummy charts.

**Build status:** FAILED  
`LiveOptics.sln` resolves `src\LiveOptics.Core\...` etc.; those paths no longer exist (now `src\OpenReportViewer.*` with old csproj names inside).

---

## Critical (P0) — build / correctness

| ID | Finding | Evidence | OpenSpec change |
|----|---------|----------|-----------------|
| QA-01 | Solution project paths invalid | `dotnet build LiveOptics.sln` → MSB3202 ×3 | `qa-fix-solution-paths` |
| QA-02 | Project references broken | Tests/UI csproj → `..\LiveOptics.Core\LiveOptics.Core.csproj` | `qa-fix-project-references` |
| QA-03 | Solution GUID placeholders | `{GUID_CORE}`, `{GUID_WPF}`, `{GUID_TESTS}` | `qa-fix-solution-paths` |
| QA-04 | Namespace/folder mismatch | `namespace LiveOptics.Core` under `OpenReportViewer.Core` | `qa-fix-namespaces` |
| QA-05 | csproj filenames still LiveOptics | `LiveOptics.Core.csproj` inside `OpenReportViewer.Core/` | `qa-fix-namespaces` |
| QA-06 | Packaging paths stale | `package.json`, `publish_portable.cmd` → `src/LiveOptics.UI.Wpf/...` | `qa-fix-publish-paths` |

## High (P1) — functional defects

| ID | Finding | Evidence | OpenSpec change |
|----|---------|----------|-----------------|
| QA-07 | Missing `BooleanToVisibilityConverter` resource | `MainWindow.xaml` uses `{StaticResource BooleanToVisibilityConverter}`; not defined in `App.xaml` or window resources | `qa-fix-wpf-converter` |
| QA-08 | Charts always dummy data | `MainViewModel.UpdateCharts` hardcodes IOPS/throughput arrays; never binds parsed performance | `qa-fix-dummy-charts` |
| QA-09 | Performance parse is empty stub | `LiveOpticsXlsxParser.ParsePerformanceData` body is comment-only | `qa-fix-performance-placeholder` |
| QA-10 | AI is mock | `ResearchAgentService` returns canned strings + `Task.Delay` | `qa-fix-real-ai-analysis` |
| QA-11 | Parser name defaults inconsistent | Code uses `"Unknown"` for missing server names; test `ParseFile_WithNullServerData` expects `string.Empty` | `qa-fix-parser-null-handling` |
| QA-12 | Report null path throws NRE | `GeneratePresentation(null, …)` NRE; test *asserts* NRE instead of contract | `qa-fix-report-null-contract` |

## Medium (P2) — docs / truth / process

| ID | Finding | Evidence | OpenSpec change |
|----|---------|----------|-----------------|
| QA-13 | Docs claim false progress | Executive summary: Sprint 3 COMPLETED, QuestPDF, 15+ tests; no Reporting project, no QuestPDF package | `qa-fix-docs-truth` |
| QA-14 | Sprint 3 guide invalid C# | `ComposeCoverPage`/`ComposeExecutiveSummary` reference `data` out of scope | `qa-fix-sprint3-guide` |
| QA-15 | No develop branch | Sprint 3 pre-check requires `On branch develop`; only `main` exists | `qa-fix-git-branch-workflow` |
| QA-16 | README structure outdated | README still documents `src/LiveOptics.*` | `qa-fix-docs-truth` |

## Low (P3) — hygiene / polish

| ID | Finding | Evidence | OpenSpec change |
|----|---------|----------|-----------------|
| QA-17 | TestResults/ untracked noise | Multiple coverage runs not in `.gitignore` | `qa-fix-gitignore` |
| QA-18 | Portable binary in tree | `PortableBuild/LiveOptics.UI.Wpf.exe` + zip at repo root | `qa-fix-gitignore` |
| QA-19 | MainViewModel no DI | `new LiveOpticsXlsxParser()` etc. in ctor | `qa-fix-ui-di` |
| QA-20 | Sample xlsx not used by tests | `docs/*.xlsx` present; tests generate synthetic Excel only | `qa-fix-test-fixtures` |
| QA-21 | Duplicate PortableBuild README | Root README vs PortableBuild/README.md drift risk | `qa-fix-docs-truth` |

---

## Open items from planning docs (not in code)

These are **product/architecture open items** derived from `docs/OpenReportViewer-*`, claimed done or planned. Each is a full OpenSpec under `openspec/changes/`.

| OpenSpec ID | Source claim | Reality |
|-------------|--------------|---------|
| `rename-to-openreportviewer` | Phase 0 complete rename | Partial folder rename only |
| `modular-architecture` | 7 modules + Reporting/API | 3 projects, old names |
| `core-interfaces-and-di` | IDataParser, DI container | Not present |
| `rvtools-parser` | vInfo/vPartition/vHost | Not present |
| `pdf-generation-questpdf` | Sprint 3 COMPLETED | Not present |
| `chart-provider-system` | IChartProvider + gallery | Not present |
| `debug-infrastructure` | IDebugService + Serilog | Not present |
| `multi-format-reports` | HTML + PPTX factory | PPTX only, hardcoded slides |
| `real-ai-analysis` | Copilot/OpenAI integration | Mock strings |
| `liveoptics-performance-data` | Full performance parse | Stub method |

---

## Build verification log

```
dotnet --version
9.0.308

dotnet build LiveOptics.sln
error MSB3202: src\LiveOptics.Core\LiveOptics.Core.csproj was not found
error MSB3202: src\LiveOptics.UI.Wpf\LiveOptics.UI.Wpf.csproj was not found
error MSB3202: src\LiveOptics.Tests\LiveOptics.Tests.csproj was not found
Build FAILED. 3 Error(s)
```

---

## Recommended sequence

1. **Sprint 0** — QA-01…QA-06, QA-17…QA-18 (make `dotnet test` green on current feature set)
2. **Sprint 1** — Finish rename + namespaces + docs truth (QA-04, QA-05, QA-13, QA-16)
3. **Sprint 2** — Interfaces, DI, unified models, real parser fixes (QA-08…QA-12, QA-19)
4. **Sprint 3+** — Planned product work (PDF, RVTools, charts) from OpenSpec changes

See `openspec/sprints/` for full sprint packages and file coverage.

---

## Build environment note (2026-09-18)

`dotnet restore` fails on this agent host when `ProgramFiles` / `ProgramFiles(x86)` / `ProgramW6432` env vars are missing. `NuGet.Configuration.ConfigurationDefaults` static ctor throws `Value cannot be null. (Parameter 'path1')`.

**Workaround:** set those vars, restore each csproj individually, then `dotnet build LiveOptics.sln --no-restore`. See `build/verify-sprint0.cmd` and `build/dotnet-env.cmd`.

**Sprint 0 verification (this host):**

```
dotnet build LiveOptics.sln --no-restore   # 0 errors, 0 warnings
dotnet test src/OpenReportViewer.Tests     # 55 passed
```

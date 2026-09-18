# Sprint 0 — Build stabilization

**Goal:** The repository builds, tests run, packaging paths resolve, and the WPF window can load.  
**Duration:** 1–2 days  
**Priority:** P0 — blocks all other sprints

## Why

QA found `dotnet build LiveOptics.sln` fails (MSB3202 ×3). Folder rename to `OpenReportViewer.*` left solution paths, project references, and publish scripts pointing at `LiveOptics.*`. This sprint restores a working baseline on current features (not new features).

## OpenSpec changes (implement these)

| Change | Fixes |
|--------|-------|
| [`fix-solution-paths`](../changes/fix-solution-paths/) | QA-01, QA-02, QA-03, QA-06 |
| [`fix-publish-paths`](../changes/fix-publish-paths/) | QA-06 packaging |
| [`qa-fix-gitignore`](../changes/qa-fix-gitignore/) | QA-17, QA-18 |
| [`qa-fix-wpf-converter`](../changes/qa-fix-wpf-converter/) | QA-07 |
| [`qa-fix-parser-null-handling`](../changes/qa-fix-parser-null-handling/) | QA-11 |
| [`qa-fix-report-null-contract`](../changes/qa-fix-report-null-contract/) | QA-12 |
| [`qa-fix-performance-placeholder`](../changes/qa-fix-performance-placeholder/) | QA-09 (explicit contract) |

## Files in scope

| File | Action |
|------|--------|
| `LiveOptics.sln` | Fix project paths + GUIDs |
| `src/OpenReportViewer.Core/LiveOptics.Core.csproj` | Ensure path referenced correctly |
| `src/OpenReportViewer.Tests/LiveOptics.Tests.csproj` | Fix ProjectReference |
| `src/OpenReportViewer.UI.Wpf/LiveOptics.UI.Wpf.csproj` | Fix ProjectReference |
| `src/OpenReportViewer.Core/Services/LiveOpticsXlsxParser.cs` | Name default contract; log stub |
| `src/OpenReportViewer.Core/Services/ReportGeneratorService.cs` | ArgumentNullException |
| `src/OpenReportViewer.Tests/Services/LiveOpticsXlsxParserTests.cs` | Align expectations |
| `src/OpenReportViewer.Tests/Services/ReportGeneratorServiceTests.cs` | Assert ArgumentNullException |
| `src/OpenReportViewer.UI.Wpf/App.xaml` | BooleanToVisibilityConverter |
| `src/OpenReportViewer.UI.Wpf/MainWindow.xaml` | Verify resource reference |
| `package.json` | Publish project path |
| `publish_portable.cmd` | Publish project path |
| `installer.iss` | Confirm exe name |
| `install_portable.bat`, `uninstall.bat`, `PortableBuild/*` | Sync paths/names as needed |
| `.gitignore` | TestResults, zips |
| `LiveOpticsReportGenerator-Portable.zip`, `PortableBuild/LiveOptics.UI.Wpf.exe` | Untrack/ignore |

## Tasks

### 1. Restore build graph
- [ ] 1.1 Update `LiveOptics.sln` → on-disk `src/OpenReportViewer.*/*.csproj` (keep LiveOptics csproj filenames until Sprint 1 if splitting)
- [ ] 1.2 Replace `{GUID_*}` placeholders
- [ ] 1.3 Fix Tests + UI `ProjectReference`
- [ ] 1.4 `dotnet build LiveOptics.sln`

### 2. Runtime safety on current UI
- [ ] 2.1 Define `BooleanToVisibilityConverter` in `App.xaml`
- [ ] 2.2 Parser name default aligned with tests
- [ ] 2.3 PPTX generator null → `ArgumentNullException`
- [ ] 2.4 `ParsePerformanceData` logs explicit unimplemented behavior

### 3. Packaging & git hygiene
- [ ] 3.1 Fix `package.json` + `publish_portable.cmd` project paths
- [ ] 3.2 Extend `.gitignore`; untrack portable binaries/zips
- [ ] 3.3 Smoke: `npm run build:dotnet` or `publish_portable.cmd` reaches existing csproj

### 4. Verify
- [ ] 4.1 `dotnet build` succeeds
- [ ] 4.2 `dotnet test` all pass
- [ ] 4.3 `git status` free of TestResults/exe noise

## Exit criteria

```
dotnet build LiveOptics.sln     # 0 errors
dotnet test                     # all green
git status                      # no TestResults/exe/zip requiring commit
```

## Explicit non-goals

- Namespace/product rename completion (Sprint 1)
- QuestPDF / new modules (Sprint 2–3)
- Real AI or performance parsing implementation (Sprint 2/4)

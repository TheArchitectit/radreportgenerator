# FILE-INVENTORY.md — every file → sprint coverage

Excludes `.git/`, `bin/`, `obj/`, `node_modules/`, `TestResults/`, generated OpenSpec tool dirs.

| File | Sprint(s) | OpenSpec change(s) | Notes |
|------|-----------|-------------------|-------|
| `LiveOptics.sln` | 0, 1 | `fix-solution-paths`, `rename-to-openreportviewer`, `modular-architecture` | Paths/GUIDs broken; rename; add modules |
| `package.json` | 0, 1 | `fix-publish-paths`, `rename-to-openreportviewer` | `build:dotnet` points at LiveOptics path |
| `package-lock.json` | 0, 1 | `fix-publish-paths` | Regenerate after package.json path fix if deps change |
| `installer.iss` | 0, 1, 3 | `fix-publish-paths`, `rename-to-openreportviewer`, `pdf-generation-questpdf` | Exe name + product name |
| `publish_portable.cmd` | 0, 1 | `fix-publish-paths`, `rename-to-openreportviewer` | cd path stale |
| `install_portable.bat` | 0, 1 | `fix-publish-paths`, `rename-to-openreportviewer` | Keep in sync with PortableBuild |
| `uninstall.bat` | 0, 1 | `rename-to-openreportviewer` | Product/display names |
| `README.md` | 1, 2 | `qa-fix-docs-truth`, `rename-to-openreportviewer`, `qa-fix-real-ai-analysis` | Structure + honest AI status |
| `LICENSE` | — | — | BSD-3-Clause; no change |
| `.gitignore` | 0 | `qa-fix-gitignore` | TestResults, zips, binaries |
| `analyze_excel.py` | 4 | `rvtools-parser`, `liveoptics-performance-data` | Source of column knowledge |
| `analyze_rvtools.py` | 4 | `rvtools-parser` | Source of sheet/column knowledge |
| `SPRINT-3-EXECUTION-GUIDE.txt` | 1, 3 | `qa-fix-sprint3-guide`, `pdf-generation-questpdf` | Template vs real impl |
| `LiveOpticsReportGenerator-Portable.zip` | 0 | `qa-fix-gitignore` | Untrack/ignore release zip |
| `docs\OpenReportViewer-Development-Plan.md` | 1, 2, 3, 4, 5 | `qa-fix-docs-truth`, feature changes | Roadmap; mark aspirational |
| `docs\OPENREPORTVIEWER-EXECUTIVE-SUMMARY.md` | 1 | `qa-fix-docs-truth` | False Sprint 3 COMPLETED |
| `docs\OpenReportViewer-Gap-Analysis-Report.md` | 1, 5 | `qa-fix-docs-truth` | Market gap input |
| `docs\OpenReportViewer-Sprint-Breakdown.md` | 1, 2 | `qa-fix-docs-truth` | Unchecked vs claimed items |
| `docs\OpenReportViewer-Sprint-Timeline.md` | 1 | `qa-fix-docs-truth` | Phase 0/1 status wrong |
| `docs\Optical-Prime-General-Excel-Definitions_pkb_en_US_1.xlsx` | 4 | `qa-fix-test-fixtures` | Live Optics schema reference |
| `docs\Optical-Prime-VMware-Excel-Definitions_pkb_en_US_1.xlsx` | 4 | `qa-fix-test-fixtures`, `liveoptics-performance-data` | VMware export definitions |
| `docs\PowerProtect-DM-Excel-Definitions_pkb_en_US_1.xlsx` | 4 | `qa-fix-test-fixtures` | Future data source reference |
| `docs\SizingWorkshop-RVTools.xlsx` | 4 | `rvtools-parser`, `qa-fix-test-fixtures` | Primary RVTools fixture |
| `PortableBuild\install_portable.bat` | 0, 1 | `fix-publish-paths` | Packaged copy |
| `PortableBuild\LiveOptics.UI.Wpf.exe` | 0 | `qa-fix-gitignore` | Do not track binary |
| `PortableBuild\README.md` | 1 | `qa-fix-docs-truth` | Snapshot of user README |
| `PortableBuild\uninstall.bat` | 0, 1 | `rename-to-openreportviewer` | Packaged copy |
| `src\OpenReportViewer.Core\LiveOptics.Core.csproj` | 0, 1, 2 | `fix-solution-paths`, `qa-fix-namespaces`, `modular-architecture` | Rename + slim to models/interfaces |
| `src\OpenReportViewer.Core\Models\Entities.cs` | 1, 2, 4 | `qa-fix-namespaces`, `core-interfaces-and-di`, `liveoptics-performance-data` | Namespace + IReportData mapping |
| `src\OpenReportViewer.Core\Services\LiveOpticsXlsxParser.cs` | 0, 1, 2, 4 | `fix-solution-paths`, `qa-fix-namespaces`, `qa-fix-parser-null-handling`, `qa-fix-performance-placeholder`, `liveoptics-performance-data`, `modular-architecture` | Move to Parsers; real performance |
| `src\OpenReportViewer.Core\Services\ReportGeneratorService.cs` | 0, 1, 2, 3 | `qa-fix-report-null-contract`, `qa-fix-namespaces`, `pdf-generation-questpdf`, `modular-architecture` | Null guard; move to Reporting |
| `src\OpenReportViewer.Core\Services\ResearchAgentService.cs` | 1, 2 | `qa-fix-real-ai-analysis`, `qa-fix-namespaces`, `modular-architecture` | Honest mock; move to AI module |
| `src\OpenReportViewer.Tests\LiveOptics.Tests.csproj` | 0, 1, 2 | `fix-solution-paths`, `qa-fix-namespaces`, `modular-architecture` | ProjectReference path |
| `src\OpenReportViewer.Tests\UnitTest1.cs` | 1, 2 | `qa-fix-namespaces`, `core-interfaces-and-di` | Expand/replace smoke tests |
| `src\OpenReportViewer.Tests\Models\EntitiesTests.cs` | 1 | `qa-fix-namespaces` | Namespace only |
| `src\OpenReportViewer.Tests\Services\LiveOpticsXlsxParserTests.cs` | 0, 2, 4 | `qa-fix-parser-null-handling`, `qa-fix-test-fixtures`, `liveoptics-performance-data`, `rvtools-parser` | Contract + fixtures |
| `src\OpenReportViewer.Tests\Services\ReportGeneratorServiceTests.cs` | 0, 2, 3 | `qa-fix-report-null-contract`, `pdf-generation-questpdf` | Typed null + PDF tests |
| `src\OpenReportViewer.Tests\Services\ResearchAgentServiceTests.cs` | 2 | `qa-fix-real-ai-analysis` | Mock labeling/provider seam |
| `src\OpenReportViewer.UI.Wpf\App.xaml` | 0, 1, 2 | `qa-fix-wpf-converter`, `qa-fix-namespaces`, `qa-fix-ui-di` | Converter resource; DI later |
| `src\OpenReportViewer.UI.Wpf\App.xaml.cs` | 1, 2 | `qa-fix-namespaces`, `qa-fix-ui-di` | Composition root |
| `src\OpenReportViewer.UI.Wpf\LiveOptics.UI.Wpf.csproj` | 0, 1, 2 | `fix-solution-paths`, `qa-fix-namespaces`, `modular-architecture` | References |
| `src\OpenReportViewer.UI.Wpf\MainWindow.xaml` | 0, 1, 2, 3 | `qa-fix-wpf-converter`, `qa-fix-namespaces`, `qa-fix-dummy-charts`, `pdf-generation-questpdf` | Resources; charts; PDF export |
| `src\OpenReportViewer.UI.Wpf\MainWindow.xaml.cs` | 1, 2 | `qa-fix-namespaces`, `qa-fix-ui-di` | DataContext |
| `src\OpenReportViewer.UI.Wpf\ViewModels\MainViewModel.cs` | 1, 2, 3, 4 | `qa-fix-dummy-charts`, `qa-fix-ui-di`, `qa-fix-real-ai-analysis`, `qa-fix-namespaces` | DI; real series; format export |
| `src\OpenReportViewer.UI.Wpf\ViewModels\Core\ViewModelBase.cs` | 1, 2 | `qa-fix-namespaces`, `qa-fix-ui-di` | Namespace + shared base |

## Coverage check

- **45 tracked project files** (excluding bin/obj/git/TestResults/tool dirs) — all mapped above.
- Newly added OpenSpec artifacts live under `openspec/`, `.agents/`, `.claude/`, `.mimocode/` and are maintained by the OpenSpec workflow, not product sprints.

# Sprint 1 — Identity & documentation truth

**Goal:** OpenReportViewer naming is consistent across code, solution, packaging, and docs. Planning docs no longer claim unimplemented sprints as complete.  
**Depends on:** Sprint 0 (build must work)  
**Duration:** 2–3 days

## OpenSpec changes

| Change | Purpose |
|--------|---------|
| [`rename-to-openreportviewer`](../changes/rename-to-openreportviewer/) | Full product rename |
| [`qa-fix-namespaces`](../changes/qa-fix-namespaces/) | Namespaces + csproj filenames |
| [`qa-fix-docs-truth`](../changes/qa-fix-docs-truth/) | Accurate status in docs |
| [`qa-fix-sprint3-guide`](../changes/qa-fix-sprint3-guide/) | Valid template code |
| [`qa-fix-git-branch-workflow`](../changes/qa-fix-git-branch-workflow/) | Branch policy vs guides |

## Files in scope

| File | Action |
|------|--------|
| `LiveOptics.sln` | Rename to `OpenReportViewer.sln` (or update in place); new project names |
| All `src/**/*.csproj` | `OpenReportViewer.*.csproj` filenames |
| All `src/**/*.cs` | `namespace OpenReportViewer.*` |
| `App.xaml`, `App.xaml.cs`, `MainWindow.xaml`, `MainWindow.xaml.cs`, ViewModels | x:Class, xmlns, usings |
| Tests `.cs` | namespaces/usings |
| `package.json`, `publish_portable.cmd`, `installer.iss` | Product + path names |
| `README.md`, `PortableBuild/README.md` | Structure + product name |
| `docs/OPENREPORTVIEWER-EXECUTIVE-SUMMARY.md` | Status = actual |
| `docs/OpenReportViewer-Sprint-Timeline.md` | Uncheck false COMPLETED |
| `docs/OpenReportViewer-Sprint-Breakdown.md` | Align checkboxes |
| `docs/OpenReportViewer-Development-Plan.md` | Aspirational banner |
| `docs/OpenReportViewer-Gap-Analysis-Report.md` | Status note |
| `SPRINT-3-EXECUTION-GUIDE.txt` | Fix sample + template banner |
| `install_portable.bat`, `uninstall.bat`, `PortableBuild/*.bat` | Display names |
| `src/**/ViewModelBase.cs` etc. | Namespace |

## Tasks

### 1. Mechanical rename
- [ ] 1.1 Solution + csproj renames
- [ ] 1.2 Namespace/XAML replace
- [ ] 1.3 Packaging script names
- [ ] 1.4 Build + test still green

### 2. Documentation truth pass
- [ ] 2.1 Executive summary CURRENT STATUS → code reality
- [ ] 2.2 Timeline Phase 0/1 marks corrected; point to `openspec/sprints/`
- [ ] 2.3 README links `openspec/QA-REVIEW.md`; correct project structure
- [ ] 2.4 Sprint 3 guide compiles conceptually; marked template-only
- [ ] 2.5 Branch policy documented; guides match

### 3. Verify
- [ ] 3.1 No remaining `LiveOptics.` namespace strings in src (except changelog notes)
- [ ] 3.2 `rg LiveOptics` only hits historical docs or intentional mentions
- [ ] 3.3 Fresh clone instructions in README work

## Exit criteria

- Build/test green under OpenReportViewer names
- Docs distinguish **implemented** vs **roadmap**
- File inventory still complete (update FILE-INVENTORY if paths renamed)

## Non-goals

- Implementing PDF/RVTools/API (later sprints)
- Deleting roadmap docs (keep as plan, fix status only)

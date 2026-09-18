# OpenReportViewer

Windows WPF application that ingests Dell Live Optics / RVTools assessment data (`.xlsx`), visualizes key performance metrics, and generates PowerPoint reports. Includes a demo research-agent sidebar (simulated insights).

![Status](https://img.shields.io/badge/status-active-success.svg)
![License](https://img.shields.io/badge/license-BSD--3--Clause-blue.svg)

## Status (verified)

See [`openspec/QA-REVIEW.md`](openspec/QA-REVIEW.md) and [`openspec/sprints/`](openspec/sprints/) for the live execution plan.

| Area | Actual state |
|------|----------------|
| Solution | `OpenReportViewer.sln` → `src/OpenReportViewer.*` |
| Build | `dotnet build` green after Sprint 0/1 path+rename work |
| Reports | PowerPoint (`.pptx`) only — PDF/HTML are OpenSpec roadmap items |
| AI sidebar | **Demo/mock** insights, not a live LLM |
| Charts | Not yet bound to parsed performance series (OpenSpec `qa-fix-dummy-charts`) |

Planning docs under `docs/` describe an enterprise roadmap. Treat unchecked/false “COMPLETED” claims there as aspirational until OpenSpec changes are archived.

## Features

* **Excel ingest** — Live Optics `.xlsx` via ExcelDataReader
* **Dashboard** — project name, server count, chart placeholders
* **Research agent (demo)** — simulated analysis text in the sidebar
* **PPTX export** — title, executive summary, AI-insights placeholder slides

## Prerequisites

* Windows 10/11 (WPF)
* [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) (9.x also works for building)
* Node.js (optional, installer packaging)

**Agent/CI note:** NuGet restore requires `ProgramFiles`, `ProgramFiles(x86)`, and `ProgramW6432` to be set. Use `build/verify-sprint0.cmd` (or `build/dotnet-env.cmd`).

## Build

```bat
build\verify-sprint0.cmd
```

Or manually:

```bat
dotnet restore src\OpenReportViewer.Core\OpenReportViewer.Core.csproj
dotnet restore src\OpenReportViewer.Tests\OpenReportViewer.Tests.csproj
dotnet restore src\OpenReportViewer.UI.Wpf\OpenReportViewer.UI.Wpf.csproj
dotnet build OpenReportViewer.sln --no-restore
dotnet test src\OpenReportViewer.Tests\OpenReportViewer.Tests.csproj --no-build
```

### Portable exe

```bat
npm run build:dotnet
:: or
publish_portable.cmd
```

Output: `PortableBuild/OpenReportViewer.UI.Wpf.exe`

### Windows installer

```bat
npm install
npm run dist
```

Output: `Installer/OpenReportViewerSetup.exe`

## Usage

1. Run `OpenReportViewer.UI.Wpf.exe`
2. **Load .xlsx** — select a Live Optics export
3. Review dashboard metrics / demo AI sidebar
4. **Generate Report** — save a `.pptx`

## Project structure

```
src/OpenReportViewer.Core/       # Models + Live Optics parser + PPTX + mock agent
src/OpenReportViewer.UI.Wpf/     # WPF UI (MVVM, LiveCharts2)
src/OpenReportViewer.Tests/      # xUnit tests
openspec/                        # Specs, change proposals, sprints, QA review
docs/                            # Roadmap docs + sample xlsx definitions
build/                           # Env-hardened verify scripts
```

## OpenSpec

Spec-driven work lives in `openspec/`:

* Baseline specs: `openspec/specs/`
* Changes (24): `openspec/changes/`
* Sprints + full file inventory: `openspec/sprints/`
* QA findings: `openspec/QA-REVIEW.md`

Install CLI (user prefix on this machine):

```bat
npm install -g @fission-ai/openspec@latest --prefix "%USERPROFILE%\.local\openspec-cli"
set PATH=%USERPROFILE%\.local\openspec-cli;%PATH%
openspec list
```

Branch policy: trunk-based on `main` with short-lived feature branches (see `openspec/changes/qa-fix-git-branch-workflow/`).

## License

BSD-3-Clause. See `LICENSE`.

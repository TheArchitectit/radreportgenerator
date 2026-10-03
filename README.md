# OpenReportViewer

Windows WPF application that ingests Dell Live Optics / RVTools assessment data (`.xlsx`), visualizes key performance metrics, and generates PowerPoint reports. Includes a demo research-agent sidebar (simulated insights).

![Status](https://img.shields.io/badge/status-active-success.svg)
![License](https://img.shields.io/badge/license-BSD--3--Clause-blue.svg)

## Status (verified)

See [`openspec/QA-REVIEW.md`](openspec/QA-REVIEW.md) and [`openspec/sprints/`](openspec/sprints/) for the live execution plan.

| Area | Actual state |
|------|----------------|
| Solution | `OpenReportViewer.sln` â†’ `src/OpenReportViewer.*` |
| Build | `dotnet build` green after Sprint 0/1 path+rename work |
| Reports | PowerPoint (`.pptx`) + QuestPDF PDF MVP; HTML still OpenSpec roadmap |
| AI sidebar | **Demo/mock** insights, not a live LLM |
| Charts | Not yet bound to parsed performance series (OpenSpec `qa-fix-dummy-charts`) |

Planning docs under `docs/` describe an enterprise roadmap. Treat unchecked/false â€œCOMPLETEDâ€ claims there as aspirational until OpenSpec changes are archived.

## Features

* **Excel ingest** â€” Live Optics `.xlsx` via ExcelDataReader
* **Dashboard** â€” project name, server count, chart placeholders
* **Research agent (demo)** â€” simulated analysis text in the sidebar
* **PPTX export** â€” title, executive summary, AI-insights placeholder slides

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
2. **Load .xlsx** â€” select a Live Optics export
3. Review dashboard metrics / demo AI sidebar
4. **Generate Report** â€” save a `.pptx`

## Project structure

```
src/OpenReportViewer.Core/       # Models, aggregates, chart builder, interfaces
src/OpenReportViewer.Parsers/    # Live Optics + RVTools parsers, ParserFactory
src/OpenReportViewer.Reporting/  # PPTX + QuestPDF PDF
src/OpenReportViewer.AI/         # Demo research agent
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

## ☕ Support This Project

If this project helps you, consider [sponsoring on GitHub](https://github.com/sponsors/TheArchitectit). Every donation goes straight back into the work — GPU hardware and cloud compute for AI development, API credits for the agents that build and test these projects, and keeping everything free and open source. As a solo architect shipping on nights and weekends, even a small monthly sponsor makes a real difference.

Help keep this project going — use a referral link below and both of us get credits!

| Service | Your Bonus | Details | Referral Code |
| --------- | ----------- | --------- | --------------- |
| [**Neuralwatt**](https://portal.neuralwatt.com/auth/register?ref=NW-ROGER-ET3Y) | $5 in credits | Refer a friend — when they use $25 in compute, you both earn $5 in credits | `NW-ROGER-ET3Y` |
| [**Synthetic**](https://synthetic.new/?referral=UAWqkKQQLFkzMkY) | $10 in credits | Subscribe → both get $10 credit | `UAWqkKQQLFkzMkY` |
| [**Ozore**](https://ozore.com/?ref=cwe4kdx0) | 50% off first month | AI-ready cloud — code **lundrog50** | `lundrog50` |

[![Buy Me a Coffee](https://img.shields.io/badge/Buy%20Me%20a%20Coffee-TheArchitectit-FFDD00?style=for-the-badge&logo=buy-me-a-coffee&logoColor=black)](https://www.buymeacoffee.com/TheArchitectit)

## License

BSD-3-Clause. See `LICENSE`.



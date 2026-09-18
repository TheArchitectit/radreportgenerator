# OpenSpec Sprints — reportgenerator

Execution plans derived from `openspec/QA-REVIEW.md` and planning docs. Every repo file is assigned to at least one sprint in `FILE-INVENTORY.md`.

## Sprint map

| Sprint | Theme | Exit criteria |
|--------|-------|---------------|
| [Sprint 0](sprint-0-build-stabilization.md) | Make the solution build and test | `dotnet build` + `dotnet test` green; packaging paths resolve |
| [Sprint 1](sprint-1-identity-and-docs.md) | Finish rename + truth in docs | OpenReportViewer identity consistent; docs match code |
| [Sprint 2](sprint-2-contracts-and-quality.md) | Interfaces, DI, parser/UI quality | Services behind interfaces; charts/agent contracts honest |
| [Sprint 3](sprint-3-reporting-mvp.md) | Real PDF + modular reporting | QuestPDF MVP ships; PPTX remains; factory seam exists |
| [Sprint 4](sprint-4-rvtools-and-performance.md) | RVTools + Live Optics performance | Multi-source parse + real chart series |
| [Sprint 5](sprint-5-observability-and-extensibility.md) | Debug/logging + HTML + chart providers | Operability + multi-format + shared charts |

## OpenSpec changes by sprint

| Sprint | Changes |
|--------|---------|
| 0 | `fix-solution-paths`, `fix-publish-paths`, `qa-fix-gitignore`, `qa-fix-wpf-converter`, `qa-fix-parser-null-handling`, `qa-fix-report-null-contract`, `qa-fix-performance-placeholder` |
| 1 | `rename-to-openreportviewer`, `qa-fix-namespaces`, `qa-fix-docs-truth`, `qa-fix-sprint3-guide`, `qa-fix-git-branch-workflow` |
| 2 | `modular-architecture`, `core-interfaces-and-di`, `qa-fix-ui-di`, `qa-fix-dummy-charts`, `qa-fix-real-ai-analysis`, `qa-fix-test-fixtures` |
| 3 | `pdf-generation-questpdf`, `multi-format-reports` (factory seam) |
| 4 | `rvtools-parser`, `liveoptics-performance-data`, `qa-fix-test-fixtures` (real samples) |
| 5 | `debug-infrastructure`, `chart-provider-system`, `multi-format-reports` (HTML complete), `core-interfaces-and-di` (IChartProvider) |

## How to execute

1. Read this sprint’s markdown.
2. Open each listed `openspec/changes/<id>/` — proposal, specs, design, tasks.
3. Implement tasks; check boxes.
4. Run verification commands in the sprint exit criteria.
5. Archive completed changes when verified (`/opsx:archive` or `openspec archive`).

## OpenSpec tooling

- CLI: `~/.local/openspec-cli/openspec` (v1.13.1) — prefix on PATH when needed
- Project skills copied to `.mimocode/skills/openspec-*` for MiMo Desktop
- Claude Code commands under `.claude/commands/opsx/`
- Vendor-neutral skills under `.agents/skills/`

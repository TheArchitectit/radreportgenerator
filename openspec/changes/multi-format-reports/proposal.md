# Proposal: Report factory and HTML output

## Intent
Roadmap: PDF + HTML + PPTX + Excel via report factory. Only hardcoded PPTX exists.

## Scope
In: IReportGenerator factory, HTML generator (Razor or string template + simple charts), keep PPTX.
Out: React SPA, REST API (later phases).

## Approach
Factory selects generator by format; HTML self-contained file with tables + optional SVG charts.

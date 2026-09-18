# Proposal: Chart provider system for reports and UI

## Intent
Planned IChartProvider with bar/line gallery for PDF embeds and WPF. Today only LiveCharts line series in UI, hardcoded.

## Scope
In: ChartData model, IChartProvider, bar/line providers, export to PNG/SVG for PDF, UI gallery later.
Out: D3.js web charts (web change).

## Approach
Shared ChartData DTO; providers render; WPF LiveCharts consumes DTO; PDF embeds rasterized charts.

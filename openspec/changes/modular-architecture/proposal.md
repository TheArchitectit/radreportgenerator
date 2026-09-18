# Proposal: Establish modular OpenReportViewer project structure

## Intent
Docs target: Core, Parsers, Visualizations, Generators/Reporting, AI.Copilot, UI.Wpf, Tests (+ optional Debugger/Benchmarks). Today only Core/Tests/UI exist with LiveOptics names and mixed responsibilities.

## Scope
In: Create missing csproj modules, move responsibilities, wire solution.
Out: Full feature content of each module (covered by feature changes).

## Approach
After rename + path fix:
1. Create OpenReportViewer.Parsers, .Reporting, .Visualizations (optional initially), .AI
2. Move parser out of Core; move PPTX generator to Reporting
3. Core holds models + interfaces only
4. Tests split or remain one project referencing modules

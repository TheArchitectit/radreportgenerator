# Proposal: Debug/logging infrastructure

## Intent
Plans call for IDebugService, Serilog file logs, parse/chart/report tracing, performance metrics. Absent today; parser uses Console.WriteLine.

## Scope
In: ILogger abstraction or Serilog, IDebugService, replace Console.WriteLine, optional debug tab later.
Out: Full debugger UI (later).

## Approach
Add Serilog to Core/Parsers; structured parse events; timings around parse and generate.

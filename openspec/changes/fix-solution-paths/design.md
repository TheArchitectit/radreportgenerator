# Design: Fix solution paths

## Approach
Minimal path repair:
- Update solution project entries to `src\OpenReportViewer.Core\LiveOptics.Core.csproj` (current on-disk names) OR full rename in same PR.
- Prefer full rename if doing both; if split, path-only fix first.
- Generate real GUIDs via `dotnet sln` or VS.
- Fix `..\LiveOptics.Core\` → correct relative path to Core csproj.
- Fix publish scripts to OpenReportViewer.UI.Wpf path.

## Files
- LiveOptics.sln
- src/OpenReportViewer.Tests/LiveOptics.Tests.csproj
- src/OpenReportViewer.UI.Wpf/LiveOptics.UI.Wpf.csproj
- package.json
- publish_portable.cmd

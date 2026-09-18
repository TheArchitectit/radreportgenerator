# Design: Complete rename

## Technical Approach
1. Fix `LiveOptics.sln` (or rename to `OpenReportViewer.sln`) with correct paths under `src/OpenReportViewer.*` and generate real GUIDs.
2. Rename csproj files to match folders.
3. Update ProjectReference paths in Tests + UI.
4. Global namespace replace LiveOptics → OpenReportViewer in .cs/.xaml.
5. Update packaging scripts and docs.

## File Changes
- LiveOptics.sln (or OpenReportViewer.sln)
- src/OpenReportViewer.Core/LiveOptics.Core.csproj → OpenReportViewer.Core.csproj
- src/OpenReportViewer.Tests/LiveOptics.Tests.csproj → OpenReportViewer.Tests.csproj
- src/OpenReportViewer.UI.Wpf/LiveOptics.UI.Wpf.csproj → OpenReportViewer.UI.Wpf.csproj
- All .cs and .xaml under src/
- package.json, publish_portable.cmd, installer.iss, README.md

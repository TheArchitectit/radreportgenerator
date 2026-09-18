# Delta for Build & Packaging

## MODIFIED Requirements

### Requirement: Portable publish
`npm run build:dotnet` and `publish_portable.cmd` MUST publish the WPF project from a path that exists, producing a single-file win-x64 exe under PortableBuild/.

#### Scenario: Stale publish path
- GIVEN scripts reference src/LiveOptics.UI.Wpf/LiveOptics.UI.Wpf.csproj
- WHEN folders are OpenReportViewer.UI.Wpf
- THEN publish fails
- AND after scripts reference src/OpenReportViewer.UI.Wpf/, publish succeeds

#### Scenario: npm run build:dotnet
- GIVEN updated package.json scripts
- WHEN portable publish runs
- THEN PortableBuild receives the WPF single-file exe

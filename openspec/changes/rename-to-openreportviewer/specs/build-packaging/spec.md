# Delta for Build & Packaging

## ADDED Requirements

### Requirement: Consistent product identity
After rename, solution name, project names, assembly names, and packaging scripts MUST all use OpenReportViewer naming (or a single agreed product name) with no LiveOptics path leftovers that break restore.

#### Scenario: Clean build
- GIVEN the renamed solution
- WHEN `dotnet build` runs at repo root
- THEN all projects restore and compile

#### Scenario: Portable publish path
- GIVEN packaging scripts
- WHEN `npm run build:dotnet` or `publish_portable.cmd` runs
- THEN the publish project path exists under src/OpenReportViewer.UI.Wpf/

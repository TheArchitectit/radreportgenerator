# Build & Packaging Specification

## Purpose
Solution build, portable publish, and Windows installer packaging.

## Requirements

### Requirement: Solution builds
`dotnet build` on the root solution MUST restore and compile all projects using paths that exist on disk.

#### Scenario: Build after folder rename
- GIVEN src project folders are named OpenReportViewer.*
- WHEN LiveOptics.sln still lists src\LiveOptics.*\*.csproj
- THEN build fails with MSB3202 (current state)

### Requirement: Project references resolve
Each csproj ProjectReference MUST point at an existing csproj path relative to the referencing project.

#### Scenario: Tests reference Core
- GIVEN OpenReportViewer.Tests\LiveOptics.Tests.csproj
- WHEN ProjectReference is `..\LiveOptics.Core\LiveOptics.Core.csproj`
- AND that path does not exist
- THEN restore/build fails

### Requirement: Portable publish
`npm run build:dotnet` and `publish_portable.cmd` MUST publish the WPF project from a path that exists, producing a single-file win-x64 exe under PortableBuild/.

#### Scenario: Stale publish path
- GIVEN scripts reference src/LiveOptics.UI.Wpf/LiveOptics.UI.Wpf.csproj
- WHEN folders are OpenReportViewer.UI.Wpf
- THEN publish fails

### Requirement: Installer consumes portable output
`installer.iss` MUST package the published exe name that publish actually produces.

### Requirement: Git hygiene
Build outputs, TestResults, and large binaries SHOULD be ignored; committed portable exes are undesirable for source control.

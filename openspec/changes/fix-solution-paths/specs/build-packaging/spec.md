# Delta for Build & Packaging

## MODIFIED Requirements

### Requirement: Solution builds
`dotnet build` on the root solution MUST restore and compile all projects using paths that exist on disk.

#### Scenario: Build after folder rename
- GIVEN src project folders are named OpenReportViewer.*
- WHEN LiveOptics.sln still lists src\LiveOptics.*\*.csproj
- THEN build fails with MSB3202
- AND after solution paths are corrected to on-disk OpenReportViewer projects, `dotnet build` restores and compiles all projects

#### Scenario: Build succeeds
- GIVEN src folders are OpenReportViewer.*
- WHEN solution project paths match those folders
- THEN `dotnet build` restores and compiles all projects

### Requirement: Project references resolve
Each csproj ProjectReference MUST point at an existing csproj path relative to the referencing project.

#### Scenario: Tests reference Core
- GIVEN OpenReportViewer.Tests project referencing Core
- WHEN ProjectReference still targets deleted ..\LiveOptics.Core\LiveOptics.Core.csproj
- THEN restore/build fails
- AND after the path is updated to the on-disk Core csproj, restore and build succeed

#### Scenario: ProjectReference resolves
- GIVEN Tests and UI projects
- WHEN ProjectReference points at the existing Core csproj
- THEN project graph loads without MSB3202/NU1101 path errors

### Requirement: Portable publish
`npm run build:dotnet` and `publish_portable.cmd` MUST publish the WPF project from a path that exists, producing a single-file win-x64 exe under PortableBuild/.

#### Scenario: Stale publish path
- GIVEN scripts reference src/LiveOptics.UI.Wpf/LiveOptics.UI.Wpf.csproj
- WHEN folders are OpenReportViewer.UI.Wpf
- THEN publish fails
- AND after scripts are updated to the on-disk OpenReportViewer.UI.Wpf path, publish succeeds

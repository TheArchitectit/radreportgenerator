# Delta for Build & Packaging

## ADDED Requirements

### Requirement: Namespace matches project
Public C# namespaces MUST match the OpenReportViewer project naming used by folders and csproj files.

#### Scenario: Consistent identity
- GIVEN compiled assemblies
- WHEN inspecting namespaces
- THEN they are under OpenReportViewer.* with no LiveOptics.* leftovers

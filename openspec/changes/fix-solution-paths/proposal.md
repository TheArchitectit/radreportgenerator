# Proposal: Fix solution paths and project references (QA-01, QA-02, QA-03, QA-06)

## Intent
`dotnet build LiveOptics.sln` fails because the solution and project references still point at deleted `src/LiveOptics.*` paths. Restore a buildable graph immediately.

## Scope
In: solution paths, GUIDs, ProjectReference paths, publish script paths.
Out: full namespace rename product decisions (covered by rename-to-openreportviewer if desired; this change can be the minimal path fix).

## Approach
Point every project path at the on-disk OpenReportViewer folders and fix ProjectReferences so tests/UI can load Core.

# Proposal: Complete OpenReportViewer rename

## Intent
Phase 0 docs claim LiveOptics → OpenReportViewer is done. Only folder names changed; solution, csproj filenames, namespaces, packaging, and README still say LiveOptics. Finish the rename so identity is consistent and the solution builds.

## Scope
In scope:
- Solution file name + project paths + real GUIDs
- Rename `LiveOptics.*.csproj` → `OpenReportViewer.*.csproj`
- C# namespaces `LiveOptics.*` → `OpenReportViewer.*`
- ProjectReference paths
- package.json / publish_portable.cmd / installer.iss / README

Out of scope:
- New architecture modules (separate changes)
- PDF/API/cloud work

## Approach
Mechanical rename in one change so build/tests can go green; then product changes build on a coherent identity.

# Proposal: Align namespaces and csproj names with OpenReportViewer (QA-04, QA-05)

## Intent
Folders are OpenReportViewer.* but C# namespaces and csproj filenames remain LiveOptics.*. This confuses tooling, docs, and future modules.

## Scope
In: csproj filenames, RootNamespace/AssemblyName if set, all C# namespaces, XAML xmlns/clr-namespace/x:Class.
Out: behavior changes.

## Approach
Mechanical rename after paths build; update tests usings; verify build+test.

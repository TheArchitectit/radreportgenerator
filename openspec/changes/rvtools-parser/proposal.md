# Proposal: RVTools xlsx parser

## Intent
Support SizingWorkshop-RVTools.xlsx style exports: vInfo (VMs), vPartition, vHost. Planned in development plan; not implemented.

## Scope
In: OpenReportViewer.Parsers RVTools parser + models + tests using docs sample when present.
Out: vCenter API live connect.

## Approach
Sheet-name based detection; map columns; cross-link VMs to hosts; produce IReportData.

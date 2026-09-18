# Delta for LiveOptics Parser

## ADDED Requirements

### Requirement: Multi-source parsing
The system MUST detect and parse RVTools workbooks in addition to Live Optics exports.

#### Scenario: RVTools file
- GIVEN an xlsx with vInfo/vHost sheets
- WHEN the user loads the file
- THEN VMs and hosts are available for reports/charts

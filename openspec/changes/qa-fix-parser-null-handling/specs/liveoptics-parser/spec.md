# Delta for LiveOptics Parser

## MODIFIED Requirements

### Requirement: Server Inventory sheet
The parser SHOULD read a worksheet named `Server Inventory` and populate servers from columns `Server Name`, `OS`, `CPU Count`, `Total Memory (GB)` when present. Missing `Server Name` cells MUST map to a single documented default (prefer `string.Empty`) consistent across parser and tests.

#### Scenario: Valid server rows
- GIVEN Server Inventory rows for Server1 (16 CPU, 64 GB) and Server2 (32 CPU, 128 GB)
- WHEN ParseFile completes
- THEN Servers contains two nodes with those names and resource values

#### Scenario: Malformed numeric cells
- GIVEN a server row with non-numeric CPU or memory
- WHEN ParseFile runs
- THEN that field remains at default (0) and other rows still parse

#### Scenario: Blank name cell
- GIVEN Server Inventory row with empty Server Name
- WHEN ParseFile completes
- THEN ServerName equals the documented default (not mixed Empty/Unknown)

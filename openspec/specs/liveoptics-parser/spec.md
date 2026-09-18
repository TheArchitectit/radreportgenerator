# LiveOptics Parser Specification

## Purpose
Parse Dell Live Optics `.xlsx` exports into the in-memory project model used by the WPF app and report generator.

## Requirements

### Requirement: File path validation
The parser MUST reject null, empty, or whitespace file paths with `ArgumentException`, and missing files with `FileNotFoundException`.

#### Scenario: Null path
- GIVEN a null path
- WHEN ParseFile is called
- THEN ArgumentException is thrown

#### Scenario: Missing file
- GIVEN a path that does not exist on disk
- WHEN ParseFile is called
- THEN FileNotFoundException is thrown

### Requirement: Project Info sheet
The parser SHOULD read a worksheet named `Project Info` and set `ProjectInfo.ProjectName` from the `Project Name` column when present.

#### Scenario: Project name present
- GIVEN an xlsx with sheet `Project Info`, column `Project Name`, value `Test Project`
- WHEN ParseFile completes
- THEN ProjectName equals `Test Project`

#### Scenario: Column absent
- GIVEN an xlsx with sheet `Project Info` but no `Project Name` column
- WHEN ParseFile completes
- THEN ProjectName remains `string.Empty`

### Requirement: Server Inventory sheet
The parser SHOULD read a worksheet named `Server Inventory` and populate servers from columns `Server Name`, `OS`, `CPU Count`, `Total Memory (GB)` when present.

#### Scenario: Valid server rows
- GIVEN Server Inventory rows for Server1 (16 CPU, 64 GB) and Server2 (32 CPU, 128 GB)
- WHEN ParseFile completes
- THEN Servers contains two nodes with those names and resource values

#### Scenario: Malformed numeric cells
- GIVEN a server row with non-numeric CPU or memory
- WHEN ParseFile runs
- THEN that field remains at default (0) and other rows still parse

### Requirement: Performance data (current stub)
Performance history fields exist on the model but the parser does NOT populate them today. Charts in the UI are therefore not driven by parsed data.

#### Scenario: Performance columns present
- GIVEN any valid Live Optics xlsx
- WHEN ParseFile completes
- THEN Performance.IoHistory and CpuHistory remain empty

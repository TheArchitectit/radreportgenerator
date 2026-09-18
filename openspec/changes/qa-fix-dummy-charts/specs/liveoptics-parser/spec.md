# Delta for LiveOptics Parser

## MODIFIED Requirements

### Requirement: Performance data
When the export contains performance time series, the parser MUST populate PerformanceProfile history used by the UI. When no series exist, history lists remain empty.

#### Scenario: Performance columns present
- GIVEN any valid Live Optics xlsx without performance series
- WHEN ParseFile completes
- THEN Performance.IoHistory and CpuHistory remain empty

#### Scenario: IO history present
- GIVEN an xlsx with timestamped IOPS columns
- WHEN ParseFile completes
- THEN at least one server or project aggregate has non-empty IoHistory

#### Scenario: No performance sheets
- GIVEN an xlsx without performance data
- WHEN ParseFile completes
- THEN history lists remain empty and UI shows empty chart state

## ADDED Requirements

### Requirement: No fabricated production metrics
The production UI MUST NOT display hardcoded sample series as if they came from the file.

#### Scenario: Load without performance data
- GIVEN a project with empty performance history
- WHEN the dashboard charts render
- THEN the UI shows an empty/placeholder chart state rather than invented IOPS numbers

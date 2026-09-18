# Delta for LiveOptics Parser

## ADDED Requirements

### Requirement: Peak metrics and series ingestion
When performance sheets exist, the parser MUST populate PerformanceProfile peak metrics and history lists.

#### Scenario: Full performance export
- GIVEN an xlsx with IOPS time series
- WHEN ParseFile completes
- THEN PeakIOPS > 0 and IoHistory is non-empty

#### Scenario: No performance sheets
- GIVEN an xlsx without performance data
- WHEN ParseFile completes
- THEN peak metrics remain 0 and history lists remain empty

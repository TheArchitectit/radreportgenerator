# Delta for Domain Models

## ADDED Requirements

### Requirement: Report data contract
A unified report data abstraction MUST allow multiple parsers and generators to interoperate.

#### Scenario: Parser to generator
- GIVEN any IDataParser producing IReportData
- WHEN any IReportGenerator consumes it
- THEN generation works without parser-specific casts

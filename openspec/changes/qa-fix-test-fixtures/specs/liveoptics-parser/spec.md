# Delta for LiveOptics Parser

## ADDED Requirements

### Requirement: Sample-data capable tests
Parser tests MUST be able to run against real sample workbooks when present, without failing when samples are absent.

#### Scenario: Sample present
- GIVEN docs sample xlsx exists
- WHEN fixture tests run
- THEN parse exercises real sheet names

#### Scenario: Sample absent
- GIVEN no sample file
- WHEN fixture tests run
- THEN those cases are skipped not failed

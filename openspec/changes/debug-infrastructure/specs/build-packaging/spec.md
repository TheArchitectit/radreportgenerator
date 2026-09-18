# Delta for Build & Packaging

## ADDED Requirements

### Requirement: Operational logging
The application MUST log parse/generate operations with timings to a file suitable for support diagnostics.

#### Scenario: Parse completes
- GIVEN a file load
- WHEN parsing finishes
- THEN a log entry records duration and server count

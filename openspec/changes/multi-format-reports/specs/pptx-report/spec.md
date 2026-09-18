# Delta for PPTX Report

## ADDED Requirements

### Requirement: Pluggable formats
Report generation MUST support selecting output format through a factory, including HTML and PDF in addition to PPTX.

#### Scenario: Format selection
- GIVEN a loaded project
- WHEN the user selects HTML
- THEN an HTML report file is produced

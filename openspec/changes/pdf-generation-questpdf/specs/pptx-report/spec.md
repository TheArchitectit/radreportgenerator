# Delta for PPTX Report

## ADDED Requirements

### Requirement: Multi-format export
In addition to PowerPoint, the application MUST generate PDF assessment reports via a pluggable report generator.

#### Scenario: PDF export
- GIVEN a loaded project
- WHEN the user chooses PDF export
- THEN a valid non-empty PDF is written

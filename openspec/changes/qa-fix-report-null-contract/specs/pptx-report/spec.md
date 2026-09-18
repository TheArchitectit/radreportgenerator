# Delta for PPTX Report

## MODIFIED Requirements

### Requirement: Null project behavior
Null project input MUST throw ArgumentNullException, not NullReferenceException.

#### Scenario: Null project
- GIVEN a null ProjectInfo
- WHEN GeneratePresentation is called
- THEN ArgumentNullException is thrown naming `project`

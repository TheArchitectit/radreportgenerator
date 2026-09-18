# Delta for Build & Packaging

## ADDED Requirements

### Requirement: Documented branch policy
The repo MUST document the supported branch workflow; sprint guides MUST not assume a branch that does not exist.

#### Scenario: Pre-sprint check
- GIVEN sprint execution guide
- WHEN it validates git branch
- THEN it matches the documented policy

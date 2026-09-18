# Delta for Build & Packaging

## ADDED Requirements

### Requirement: Modular project layout
The solution MUST separate domain models/interfaces (Core) from parsers, report generators, AI providers, and UI.

#### Scenario: Solution modules
- GIVEN the modular solution
- WHEN listing projects
- THEN Core, Parsers, Reporting, AI, UI.Wpf, Tests exist and build

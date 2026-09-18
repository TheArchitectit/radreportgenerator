# Delta for WPF UI

## ADDED Requirements

### Requirement: Service injection
ViewModels MUST receive parser, analysis, and report services via dependency injection rather than hard-coded constructors for production composition.

#### Scenario: Composition root
- GIVEN App startup
- WHEN the main window is created
- THEN MainViewModel dependencies are provided by the DI container

# Delta for WPF UI

## ADDED Requirements

### Requirement: Shared chart model
UI charts MUST be built from a shared chart data model so PDF and UI show the same series.

#### Scenario: Same metrics two surfaces
- GIVEN parsed performance metrics
- WHEN PDF and UI charts render
- THEN they use the same ChartData aggregates

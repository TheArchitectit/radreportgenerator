# WPF UI Specification

## Purpose
Desktop shell for loading Live Optics xlsx, viewing KPIs/charts, running mock AI analysis, and exporting PPTX.

## Requirements

### Requirement: File load flow
The user MUST be able to trigger a file open dialog filtered for `.xlsx`, parse the file off the UI thread, and see project name + server count on the dashboard.

#### Scenario: Successful load
- GIVEN a valid Live Optics xlsx
- WHEN the user confirms the open dialog
- THEN ProjectName and ServerCount update and status shows the project name

#### Scenario: Load failure
- GIVEN an invalid file
- WHEN parse throws
- THEN an error dialog is shown and status carries the error message

### Requirement: Report export
The user MUST be able to save a `.pptx` via SaveFileDialog once a project is loaded. Generate Report is disabled until load succeeds.

#### Scenario: Export after load
- GIVEN a loaded project
- WHEN the user saves a path
- THEN a PPTX is written and status shows the path

### Requirement: AI analysis action
The user MUST be able to run the research agent once a project is loaded. Insights appear in the sidebar list.

#### Scenario: Run analysis
- GIVEN a loaded project
- WHEN Run Analysis Agent is invoked
- THEN one or more insight strings are added to AiInsights

### Requirement: Charts visibility binding
Busy progress UI uses a boolean-to-visibility converter resource. That resource MUST be defined in application or window resources or XAML load fails at runtime.

#### Scenario: Converter missing
- GIVEN MainWindow.xaml references StaticResource BooleanToVisibilityConverter
- AND the resource is not defined
- WHEN the window loads
- THEN the current tree fails resource resolution (QA finding QA-07)

### Requirement: Chart data source
Charts SHOULD reflect parsed performance series when available. Today they always show hardcoded dummy series (QA-08).

# PPTX Report Specification

## Purpose
Generate a PowerPoint presentation summarizing a loaded Live Optics project.

## Requirements

### Requirement: Presentation generation
Given a non-null `ProjectInfo` and an output path, the generator MUST create a non-empty `.pptx` file using Open XML.

#### Scenario: Valid project
- GIVEN a project named TestProject with at least one server
- WHEN GeneratePresentation is called with a writable path
- THEN a .pptx file exists at that path and has length > 0

### Requirement: Slide content
The presentation MUST include at least a title slide, an executive summary with server/CPU/memory totals, and an AI-insights placeholder slide.

#### Scenario: Totals reflected
- GIVEN 2 servers totaling 48 CPU cores and 192 GB memory
- WHEN the executive summary slide is generated
- THEN the slide text includes those totals

### Requirement: Null project behavior
Null project input is currently undefined and throws NullReferenceException. Callers MUST NOT pass null; a future change should define a typed validation error.

#### Scenario: Null project
- GIVEN a null ProjectInfo
- WHEN GeneratePresentation is called
- THEN the current implementation throws NullReferenceException

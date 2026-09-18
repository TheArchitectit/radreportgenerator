# Proposal: Define typed error for null project in PPTX generator (QA-12)

## Intent
GeneratePresentation(null) throws NullReferenceException; tests assert NRE. Callers deserve ArgumentNullException or a validation result, not NRE.

## Approach
Throw ArgumentNullException when project is null; update tests to assert the typed exception; optionally add ValidationResult later for richer errors.

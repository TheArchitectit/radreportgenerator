# Delta for Build & Packaging

## ADDED Requirements

### Requirement: Documentation accuracy
Planning documents MUST distinguish aspirational roadmap items from verified implemented behavior.

#### Scenario: Status header
- GIVEN any sprint/completion claim in docs/
- WHEN code does not implement the claim
- THEN the doc marks it planned/not implemented and references OpenSpec changes

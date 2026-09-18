# Delta for Build & Packaging

## MODIFIED Requirements

### Requirement: Git hygiene
Build outputs, test results, and large portable binaries MUST be ignored by git.

#### Scenario: After test run
- GIVEN dotnet test writes TestResults/
- WHEN `git status` runs
- THEN TestResults is not listed as untracked noise requiring commit

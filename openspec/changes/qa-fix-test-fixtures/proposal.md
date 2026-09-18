# Proposal: Use sample workbooks as parser fixtures (QA-20)

## Intent
docs/ contains SizingWorkshop-RVTools.xlsx and Optical Prime / PowerProtect definition workbooks. Tests only synthesize tiny Excel files. Real samples unlock realistic parser work.

## Approach
Add test fixture helper that copies sample xlsx to test output when present; mark tests that need samples as skippable if file missing; later RVTools parser uses SizingWorkshop file.

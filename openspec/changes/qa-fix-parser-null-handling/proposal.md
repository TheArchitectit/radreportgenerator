# Proposal: Align parser null/empty name handling with tests (QA-11)

## Intent
Parser sets missing server names to `"Unknown"`; tests expect `string.Empty` for null/empty cells. Pick one contract and make code+tests agree.

## Approach
Recommend: empty string for missing names; `"Unknown"` only when a row exists but name cell is null *and* product requires display fallback — OR change tests to Unknown. Document the chosen contract in specs and implement consistently.

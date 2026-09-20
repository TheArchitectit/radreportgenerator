# Branch policy

**Decision (2026-09-18):** trunk-based development on `main`.

## Rules

1. `main` is always the integration branch.
2. Short-lived feature branches: `feature/<change-id>` or `fix/<change-id>` off `main`.
3. OpenSpec changes map to branches when useful; small Sprint 0/1 fixes may land directly on `main` when tests pass.
4. Do **not** require a long-lived `develop` branch — it does not exist in this repo.
5. Sprint guides must validate against `main` (or the active feature branch), not `develop`.

## Verification

```bat
build\verify-sprint0.cmd
git status
git push origin main
```

OpenSpec: `openspec/changes/qa-fix-git-branch-workflow/`


**2026-09-18 closure:** Optional develop branch **not created**. Trunk-based main remains the policy (OpenSpec qa-fix-git-branch-workflow task 1.3 closed as N/A by decision).

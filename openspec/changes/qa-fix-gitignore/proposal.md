# Proposal: Ignore build/test artifacts and stop tracking portable binaries (QA-17, QA-18)

## Intent
TestResults/, root zip, and PortableBuild/*.exe pollute the repo. .gitignore covers bin/obj/PortableBuild/ but TestResults and root-level zip/exe leftovers are still untracked noise or risk accidental commit.

## Approach
Extend .gitignore; git rm --cached any tracked binaries; document release artifact location.

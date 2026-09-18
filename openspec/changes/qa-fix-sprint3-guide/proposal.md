# Proposal: Fix invalid sample code in SPRINT-3-EXECUTION-GUIDE (QA-14)

## Intent
The guide's QuestPdfReportGenerator sample references `data` inside ComposeCoverPage/ComposeExecutiveSummary where `data` is not in scope (and has stray `\n` artifacts). Agents following the guide will produce non-compiling code.

## Approach
Correct the sample to pass IReportData into compose methods; mark guide as "planned template, not verified implementation".

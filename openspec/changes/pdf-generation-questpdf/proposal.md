# Proposal: PDF report generation with QuestPDF (claimed Sprint 3)

## Intent
Executive summary claims Sprint 3 PDF MVP complete with QuestPDF. No Reporting project or QuestPDF package exists. Implement real PDF generation for infrastructure assessment.

## Scope
In: OpenReportViewer.Reporting, QuestPDF package, IPdfReportGenerator, cover + executive summary + optional charts, export dialog path, tests.
Out: HTML/D3, full 50-page templates (later changes).

## Approach
Follow corrected SPRINT-3 guide; set QuestPDF license; generate MemoryStream PDF from IReportData; integrate with UI save dialog (PDF filter) alongside PPTX.

# Proposal: Repair packaging scripts for current tree (alias of QA-06 packaging)

## Intent
package.json and publish_portable.cmd still target LiveOptics.UI.Wpf paths. Ensure portable and installer builds work after OpenReportViewer folders.

## Approach
Update paths; smoke publish; ensure installer.iss exe name matches output.

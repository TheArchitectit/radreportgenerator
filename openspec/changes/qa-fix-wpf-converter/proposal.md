# Proposal: Define BooleanToVisibilityConverter resource (QA-07)

## Intent
MainWindow.xaml binds ProgressBar visibility to `{StaticResource BooleanToVisibilityConverter}` but the resource is not defined. Window load will fail at runtime when that binding is evaluated.

## Scope
In: App.xaml or MainWindow resources; optional code-behind cleanup.
Out: redesign of busy UI.

## Approach
Add standard WPF BooleanToVisibilityConverter (or CommunityToolkit equivalent) as an application-level resource.

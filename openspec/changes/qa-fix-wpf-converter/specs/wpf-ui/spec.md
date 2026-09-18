# Delta for WPF UI

## MODIFIED Requirements

### Requirement: Charts visibility binding
Busy progress UI uses a boolean-to-visibility converter resource. That resource MUST be defined in application or window resources or XAML load fails at runtime.

#### Scenario: Converter missing
- GIVEN MainWindow.xaml references StaticResource BooleanToVisibilityConverter
- AND the resource is not defined
- WHEN the window loads
- THEN resource resolution fails until the converter is added to App.xaml or window resources

#### Scenario: Resource defined
- GIVEN App.xaml defines BooleanToVisibilityConverter
- WHEN MainWindow loads and IsBusy changes
- THEN progress bar visibility toggles without resource errors

# Design: Converter resource

Add to App.xaml:
```xml
<Application.Resources>
  <BooleanToVisibilityConverter x:Key="BooleanToVisibilityConverter"/>
</Application.Resources>
```
Alternatively define in MainWindow.Resources if app resources are intentionally empty.

Files: src/OpenReportViewer.UI.Wpf/App.xaml (or MainWindow.xaml)

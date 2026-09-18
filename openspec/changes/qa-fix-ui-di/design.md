# Design: WPF DI

App startup:
```
services.AddSingleton<ILiveOpticsParser, LiveOpticsXlsxParser>();
services.AddSingleton<IResearchAgent, ResearchAgentService>();
services.AddSingleton<IReportGenerator, ReportGeneratorService>();
services.AddTransient<MainViewModel>();
```
MainWindow resolves MainViewModel.

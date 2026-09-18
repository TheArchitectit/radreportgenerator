# Design: Chart providers

```
ChartData { Title, Categories, Series, Options }
IChartProvider { ChartType Type; RenderResult Render(ChartData); }
```
WPF: LiveCharts adapter.
PDF: SkiaSharp/SVG image bytes.

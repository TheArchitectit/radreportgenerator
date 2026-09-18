using OpenReportViewer.Core.Models;

namespace OpenReportViewer.Core.Charts
{
    public enum ChartProviderKind
    {
        Bar,
        Line,
        Empty
    }

    public sealed class RenderableChart
    {
        public string Title { get; init; } = string.Empty;
        public ChartProviderKind Kind { get; init; } = ChartProviderKind.Empty;
        public IReadOnlyList<string> Labels { get; init; } = Array.Empty<string>();
        public IReadOnlyList<double> Values { get; init; } = Array.Empty<double>();
        public string ValueUnit { get; init; } = string.Empty;
        public string EmptyMessage { get; init; } = string.Empty;
        public double MaxValue => Values.Count == 0 ? 0 : Values.Max();

        /// <summary>ASCII bar preview for logs/tests; UI uses LiveCharts series.</summary>
        public string ToAsciiPreview(int width = 20)
        {
            if (Kind == ChartProviderKind.Empty || Values.Count == 0)
                return EmptyMessage;
            var max = MaxValue;
            if (max <= 0) max = 1;
            var lines = new List<string> { Title };
            for (var i = 0; i < Values.Count; i++)
            {
                var fill = (int)Math.Round(Values[i] / max * width);
                var bar = new string('#', Math.Clamp(fill, 0, width));
                lines.Add($"{Labels[i].PadRight(16).Substring(0, Math.Min(16, Labels[i].Length))} {bar} {Values[i]:0.#}{ValueUnit}");
            }
            return string.Join(Environment.NewLine, lines);
        }
    }

    public interface IChartProvider
    {
        ChartProviderKind Kind { get; }
        bool CanRender(ChartSeries series);
        RenderableChart Render(ChartSeries series);
    }

    public sealed class BarChartProvider : IChartProvider
    {
        public ChartProviderKind Kind => ChartProviderKind.Bar;

        public bool CanRender(ChartSeries series) =>
            series != null && series.Kind == ChartKind.BarCategories && series.Points.Count > 0;

        public RenderableChart Render(ChartSeries series)
        {
            if (series == null)
            {
                return new RenderableChart
                {
                    Kind = ChartProviderKind.Empty,
                    EmptyMessage = "No chart series provided."
                };
            }
            if (!CanRender(series))
            {
                return new RenderableChart
                {
                    Title = series.Title,
                    Kind = ChartProviderKind.Empty,
                    EmptyMessage = series.EmptyMessage,
                    ValueUnit = series.ValueUnit
                };
            }
            return new RenderableChart
            {
                Title = series.Title,
                Kind = ChartProviderKind.Bar,
                Labels = series.Points.Select(p => p.Label).ToArray(),
                Values = series.Points.Select(p => p.Value).ToArray(),
                ValueUnit = series.ValueUnit
            };
        }
    }

    public sealed class LineChartProvider : IChartProvider
    {
        public ChartProviderKind Kind => ChartProviderKind.Line;

        public bool CanRender(ChartSeries series) =>
            series != null && series.Points.Count > 0;

        public RenderableChart Render(ChartSeries series)
        {
            if (!CanRender(series!))
            {
                return new RenderableChart
                {
                    Kind = ChartProviderKind.Empty,
                    EmptyMessage = series?.EmptyMessage ?? "No series",
                    Title = series?.Title ?? ""
                };
            }
            return new RenderableChart
            {
                Title = series.Title,
                Kind = ChartProviderKind.Line,
                Labels = series.Points.Select(p => p.Label).ToArray(),
                Values = series.Points.Select(p => p.Value).ToArray(),
                ValueUnit = series.ValueUnit
            };
        }
    }

    public sealed class ChartProviderFactory
    {
        private readonly IReadOnlyList<IChartProvider> _providers;

        public ChartProviderFactory(IEnumerable<IChartProvider>? providers = null)
        {
            _providers = (providers ?? new IChartProvider[] { new BarChartProvider(), new LineChartProvider() }).ToList();
        }

        public RenderableChart Render(ChartSeries series)
        {
            if (series == null)
                return new RenderableChart { Kind = ChartProviderKind.Empty, EmptyMessage = "No series" };

            if (series.Kind == ChartKind.BarCategories)
            {
                var bar = _providers.FirstOrDefault(p => p.Kind == ChartProviderKind.Bar && p.CanRender(series))
                          ?? _providers.FirstOrDefault(p => p.CanRender(series));
                return bar?.Render(series) ?? new RenderableChart { Kind = ChartProviderKind.Empty, EmptyMessage = series.EmptyMessage };
            }

            var any = _providers.FirstOrDefault(p => p.CanRender(series));
            return any?.Render(series) ?? new RenderableChart
            {
                Title = series.Title,
                Kind = ChartProviderKind.Empty,
                EmptyMessage = series.EmptyMessage
            };
        }
    }
}

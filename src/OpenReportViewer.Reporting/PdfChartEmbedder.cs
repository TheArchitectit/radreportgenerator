using System.Net;
using System.Text;
using OpenReportViewer.Core.Charts;
using OpenReportViewer.Core.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace OpenReportViewer.Reporting
{
    public static class PdfChartEmbedder
    {
        /// <summary>
        /// Embeds category bar charts in PDF using the shared chart provider model.
        /// Renders vector-style horizontal bars via QuestPDF canvas (no raster dependency).
        /// </summary>
        public static void ComposeChartPage(IContainer container, string title, RenderableChart chart)
        {
            container.Column(column =>
            {
                column.Item().Text(title).FontSize(16).Bold();
                if (chart == null || chart.Kind == ChartProviderKind.Empty || chart.Values.Count == 0)
                {
                    column.Item().PaddingTop(10).Text(chart?.EmptyMessage ?? "No chart data.")
                        .FontSize(10).FontColor(Colors.Grey.Darken1);
                    return;
                }

                var max = chart.MaxValue <= 0 ? 1 : chart.MaxValue;
                var take = Math.Min(chart.Values.Count, 15);

                column.Item().PaddingTop(8).Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        c.RelativeColumn(3);
                        c.RelativeColumn(4);
                        c.RelativeColumn(2);
                    });
                    table.Header(h =>
                    {
                        h.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Label").Bold();
                        h.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Bar").Bold();
                        h.Cell().Background(Colors.Grey.Lighten3).Padding(4).AlignRight().Text(chart.ValueUnit).Bold();
                    });

                    for (var i = 0; i < take; i++)
                    {
                        var label = i < chart.Labels.Count ? chart.Labels[i] : "";
                        var value = chart.Values[i];
                        var pct = value / max;

                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4)
                            .Text(label);
                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3)
                            .Element(c => DrawBar(c, pct));
                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4)
                            .AlignRight().Text(value.ToString("0.#"));
                    }
                });

                column.Item().PaddingTop(8).Text(chart.ToAsciiPreview(24))
                    .FontFamily(Fonts.Consolas).FontSize(8).FontColor(Colors.Grey.Darken1);
            });
        }

        private static void DrawBar(IContainer container, double fraction)
        {
            var f = Math.Clamp(fraction, 0.02, 1.0);
            container.Height(14).Background(Colors.Grey.Lighten3).Padding(1).Row(row =>
                {
                    row.RelativeItem((int)Math.Max(1, f * 1000)).Background(Colors.Blue.Medium);
                    row.RelativeItem((int)Math.Max(1, (1 - f) * 1000)).Background(Colors.Grey.Lighten3);
                });
        }

        public static void ComposeProjectCharts(IContainer container, ProjectInfo project, ChartProviderFactory? factory = null)
        {
            factory ??= new ChartProviderFactory();
            var cpu = factory.Render(ChartDataBuilder.VmCpuTop(project));
            var mem = factory.Render(ChartDataBuilder.VmMemoryTop(project));
            var part = factory.Render(ChartDataBuilder.PartitionCapacityTop(project));

            container.Column(column =>
            {
                column.Item().Element(c => ComposeChartPage(c, cpu.Title, cpu));
                if (mem.Kind != ChartProviderKind.Empty)
                {
                    column.Item().PaddingTop(16).Element(c => ComposeChartPage(c, mem.Title, mem));
                }
                if (part.Kind != ChartProviderKind.Empty)
                {
                    column.Item().PaddingTop(16).Element(c => ComposeChartPage(c, part.Title, part));
                }
            });
        }
    }
}

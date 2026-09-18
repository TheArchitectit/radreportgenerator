using OpenReportViewer.Core.Charts;
using OpenReportViewer.Core.Models;
using Xunit;

namespace OpenReportViewer.Tests.Charts
{
    public class ChartProviderTests
    {
        [Fact]
        public void BarChartProvider_RendersCategories()
        {
            var series = new ChartSeries
            {
                Title = "Top",
                Kind = ChartKind.BarCategories,
                Points = new[] { ("a", 1.0), ("b", 4.0) },
                ValueUnit = "cores"
            };
            var provider = new BarChartProvider();
            Assert.True(provider.CanRender(series));
            var chart = provider.Render(series);
            Assert.Equal(ChartProviderKind.Bar, chart.Kind);
            Assert.Equal(2, chart.Values.Count);
            Assert.Equal(4, chart.MaxValue);
            Assert.Contains("Top", chart.ToAsciiPreview());
        }

        [Fact]
        public void BarChartProvider_EmptySeries_ReturnsEmptyMessage()
        {
            var series = new ChartSeries { Title = "x", Kind = ChartKind.Empty, EmptyMessage = "none" };
            var chart = new BarChartProvider().Render(series);
            Assert.Equal(ChartProviderKind.Empty, chart.Kind);
            Assert.Equal("none", chart.EmptyMessage);
        }

        [Fact]
        public void ChartProviderFactory_SelectsBarForBarSeries()
        {
            var series = ChartDataBuilder.VmCpuTop(BuildVmProject());
            var render = new ChartProviderFactory().Render(series);
            Assert.Equal(ChartProviderKind.Bar, render.Kind);
            Assert.True(render.Values.Count > 0);
        }

        [Fact]
        public void ChartDataBuilder_EmptyProject_EmptyChart()
        {
            var render = new ChartProviderFactory().Render(ChartDataBuilder.VmCpuTop(new ProjectInfo()));
            Assert.Equal(ChartProviderKind.Empty, render.Kind);
        }

        private static ProjectInfo BuildVmProject()
        {
            var p = new ProjectInfo();
            p.VirtualMachines.Add(new VirtualMachine { Name = "v1", CpuCount = 2 });
            p.VirtualMachines.Add(new VirtualMachine { Name = "v2", CpuCount = 8 });
            return p;
        }
    }
}

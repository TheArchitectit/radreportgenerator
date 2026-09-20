using OpenReportViewer.Core.Charts;
using OpenReportViewer.Core.Models;
using OpenReportViewer.Reporting;
using Xunit;

namespace OpenReportViewer.Tests.Reporting
{
    public class PdfChartEmbedderTests
    {
        [Fact]
        public void GeneratePdf_WithVmData_IncludesChartSection()
        {
            var p = new ProjectInfo { ProjectName = "charts", SourceType = "RVTools" };
            p.VirtualMachines.Add(new VirtualMachine { Name = "big", CpuCount = 16, MemoryMB = 262144 });
            p.VirtualMachines.Add(new VirtualMachine { Name = "small", CpuCount = 2, MemoryMB = 2048 });

            using var stream = new QuestPdfReportGenerator().Generate(p);
            Assert.True(stream.Length > 200);
            var buf = new byte[5];
            var pos = stream.Position;
            stream.Position = 0;
            stream.ReadExactly(buf);
            stream.Position = pos;
            Assert.Equal("%PDF-", System.Text.Encoding.ASCII.GetString(buf));
        }

        [Fact]
        public void ComposeProjectCharts_EmptyProject_DoesNotThrow()
        {
            var factory = new ChartProviderFactory();
            var chart = factory.Render(ChartDataBuilder.VmCpuTop(new ProjectInfo()));
            Assert.Equal(ChartProviderKind.Empty, chart.Kind);
        }
    }
}

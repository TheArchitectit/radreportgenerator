using OpenReportViewer.Core.Interfaces;
using OpenReportViewer.Core.Models;
using OpenReportViewer.Parsers;
using Xunit;

namespace OpenReportViewer.Tests.Parsers
{
    public class RVToolsParserTests
    {
        private static string SamplePath =>
            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "docs", "SizingWorkshop-RVTools.xlsx"));

        private static string? FindSample()
        {
            var candidates = new[]
            {
                SamplePath,
                Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "docs", "SizingWorkshop-RVTools.xlsx")),
                Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "docs", "SizingWorkshop-RVTools.xlsx")),
                Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../../docs/SizingWorkshop-RVTools.xlsx")),
            };
            // walk up from test assembly
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null)
            {
                var p = Path.Combine(dir.FullName, "docs", "SizingWorkshop-RVTools.xlsx");
                if (File.Exists(p)) return p;
                dir = dir.Parent;
            }
            foreach (var c in candidates)
                if (File.Exists(c)) return c;
            return null;
        }

        [Fact]
        public void CanParse_WithMissingFile_ReturnsFalse()
        {
            var parser = new RVToolsParser();
            Assert.False(parser.CanParse(Path.Combine(Path.GetTempPath(), "nope.xlsx")));
        }

        [Fact]
        public void ParseFile_WithNullPath_Throws()
        {
            var parser = new RVToolsParser();
            Assert.Throws<ArgumentException>(() => parser.ParseFile(null!));
        }

        [Fact]
        public void ParseFile_WithSampleRvTools_PopulatesVmsHostsPartitions()
        {
            var sample = FindSample();
            if (sample == null)
            {
                // skip when sample not present
                return;
            }

            var parser = new RVToolsParser();
            Assert.True(parser.CanParse(sample));
            var project = parser.ParseFile(sample);

            Assert.Equal("RVTools", project.SourceType);
            Assert.True(project.VirtualMachines.Count > 0);
            Assert.True(project.Hosts.Count > 0);
            Assert.True(project.Partitions.Count > 0);
            Assert.True(project.Servers.Count > 0);

            var cpu = ProjectAggregates.TotalCpuCores(project);
            Assert.True(cpu > 0);
            Assert.True(ProjectAggregates.TopVmCpu(project, 5).Count > 0);

            var host = project.Hosts[0];
            Assert.False(string.IsNullOrWhiteSpace(host.HostName));
        }

        [Fact]
        public void ParserFactory_ResolvesRvToolsSample()
        {
            var sample = FindSample();
            if (sample == null) return;

            var factory = new ParserFactory(new IDataParser[] { new RVToolsParser(), new LiveOpticsXlsxParser() });
            var project = factory.Parse(sample);
            Assert.Equal("RVTools", project.SourceType);
            Assert.True(project.VirtualMachines.Count > 0);
        }

        [Fact]
        public void ChartDataBuilder_FromRvTools_ReturnsBarSeries()
        {
            var sample = FindSample();
            if (sample == null) return;

            var project = new RVToolsParser().ParseFile(sample);
            var cpuChart = ChartDataBuilder.VmCpuTop(project);
            Assert.Equal(ChartKind.BarCategories, cpuChart.Kind);
            Assert.True(cpuChart.Points.Count > 0);
        }
    }
}

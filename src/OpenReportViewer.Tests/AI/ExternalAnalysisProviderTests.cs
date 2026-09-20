using OpenReportViewer.AI;
using OpenReportViewer.Core.Models;
using Xunit;

namespace OpenReportViewer.Tests.AI
{
    public class ExternalAnalysisProviderTests
    {
        [Fact]
        public async Task Heuristic_FromRvToolsLikeProject_ReturnsActionableLines()
        {
            var p = new ProjectInfo { SourceType = "RVTools" };
            for (int i = 0; i < 40; i++)
                p.VirtualMachines.Add(new VirtualMachine { Name = $"vm{i}", CpuCount = i % 3 == 0 ? 16 : 2, MemoryMB = i % 3 == 0 ? 131072 : 2048, PowerState = i == 1 ? "poweredOff" : "poweredOn" });
            p.Hosts.Add(new HostNode { HostName = "h1", TotalCores = 48, VmCount = 30, EsxVersion = "7.0 U3" });

            var svc = new HeuristicAnalysisService();
            Assert.False(svc.IsDemoProvider);
            var req = AnalysisRequestFactory.FromProject(p);
            var lines = await svc.AnalyzeAsync(req);

            Assert.NotEmpty(lines);
            Assert.Contains(lines, l => l.Contains("Heuristic"));
            Assert.Contains(lines, l => l.Contains("Right-size") || l.Contains("Density") || l.Contains("Host h1") || l.Contains("powered off"));
        }

        [Fact]
        public async Task OpenAiCompatible_WithoutConfig_FallsBackToHeuristic()
        {
            var svc = new OpenAiCompatibleAnalysisService(null, null);
            Assert.False(svc.IsConfigured);
            Assert.False(svc.IsDemoProvider);
            var lines = await svc.AnalyzeAsync(AnalysisRequestFactory.FromProject(new ProjectInfo { SourceType = "LiveOptics" }));
            Assert.NotEmpty(lines);
        }

        [Fact]
        public void AnalysisRequestFactory_MapsAggregates()
        {
            var p = new ProjectInfo { SourceType = "RVTools" };
            p.VirtualMachines.Add(new VirtualMachine { Name = "a", CpuCount = 4, MemoryMB = 8192, PowerState = "poweredOn" });
            var req = AnalysisRequestFactory.FromProject(p);
            Assert.Equal("RVTools", req.SourceType);
            Assert.Equal(1, req.VmCount);
            Assert.Equal(4, req.TotalCpuCores);
            Assert.Single(req.TopVms);
        }
    }
}

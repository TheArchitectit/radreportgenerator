using Xunit;
using OpenReportViewer.Core.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OpenReportViewer.Tests.Services
{
    public class ResearchAgentServiceTests
    {
        private readonly ResearchAgentService _service;

        public ResearchAgentServiceTests()
        {
            _service = new ResearchAgentService();
        }

        [Fact]
        public async Task AnalyzePerformanceAsync_ReturnsAnalysis()
        {
            var query = "High IOPS on database server";
            var result = await _service.AnalyzePerformanceAsync(query);

            Assert.NotNull(result);
            Assert.Contains(query, result);
        }

        [Fact]
        public async Task AnalyzePerformanceAsync_WithNullQuery_ReturnsAnalysis()
        {
            var result = await _service.AnalyzePerformanceAsync(null!);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task AnalyzePerformanceAsync_WithEmptyQuery_ReturnsAnalysis()
        {
            var result = await _service.AnalyzePerformanceAsync("");

            Assert.NotNull(result);
        }

        [Fact]
        public async Task ResearchHardwareAsync_ReturnsHardwareInfo()
        {
            var model = "Dell R740";
            var result = await _service.ResearchHardwareAsync(model);

            Assert.NotNull(result);
            Assert.Contains(model, result);
        }

        [Fact]
        public async Task ResearchHardwareAsync_WithNullModel_ReturnsHardwareInfo()
        {
            var result = await _service.ResearchHardwareAsync(null!);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task ResearchHardwareAsync_WithEmptyModel_ReturnsHardwareInfo()
        {
            var result = await _service.ResearchHardwareAsync("");

            Assert.NotNull(result);
        }

        [Fact]
        public async Task AnalyzePerformanceAsync_DifferentQueries_ReturnsDifferentAnalysis()
        {
            var result1 = await _service.AnalyzePerformanceAsync("Query 1");
            var result2 = await _service.AnalyzePerformanceAsync("Query 2");

            Assert.Contains("Query 1", result1);
            Assert.Contains("Query 2", result2);
            Assert.NotEqual(result1, result2);
        }
    }
}

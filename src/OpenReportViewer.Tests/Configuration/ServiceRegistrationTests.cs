using Microsoft.Extensions.DependencyInjection;
using OpenReportViewer.AI;
using OpenReportViewer.Core.Configuration;
using OpenReportViewer.Core.Diagnostics;
using OpenReportViewer.Core.Interfaces;
using OpenReportViewer.Parsers;
using OpenReportViewer.Reporting;
using Xunit;

namespace OpenReportViewer.Tests.Configuration
{
    public class ServiceRegistrationTests
    {
        [Fact]
        public void AddOpenReportViewerModules_ResolvesKeyServices()
        {
            var sc = new ServiceCollection();
            sc.AddOpenReportViewerCore();
            sc.AddOpenReportViewerLogging();
            sc.AddOpenReportViewerParsers();
            sc.AddOpenReportViewerReporting();
            sc.AddOpenReportViewerPdf();
            sc.AddOpenReportViewerHtml();
            sc.AddOpenReportViewerAI();
            using var sp = sc.BuildServiceProvider();

            Assert.NotNull(sp.GetRequiredService<IAppLog>());
            Assert.NotNull(sp.GetRequiredService<IDebugService>());
            Assert.NotNull(sp.GetRequiredService<IDataParser>());
            Assert.NotNull(sp.GetRequiredService<ParserFactory>());
            Assert.NotNull(sp.GetRequiredService<IPptxReportGenerator>());
            Assert.NotNull(sp.GetRequiredService<IPdfReportGenerator>());
            Assert.NotNull(sp.GetRequiredService<HtmlReportGenerator>());
            Assert.NotNull(sp.GetRequiredService<IAnalysisService>());
            Assert.True(sp.GetRequiredService<IAnalysisService>().IsDemoProvider);
        }
    }
}

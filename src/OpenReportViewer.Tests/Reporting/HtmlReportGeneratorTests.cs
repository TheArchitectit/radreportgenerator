using OpenReportViewer.Core.Models;
using OpenReportViewer.Reporting;
using Xunit;

namespace OpenReportViewer.Tests.Reporting
{
    public class HtmlReportGeneratorTests
    {
        [Fact]
        public void GenerateHtml_WithVms_ContainsTableRows()
        {
            var p = new ProjectInfo { ProjectName = "HtmlProj", SourceType = "RVTools" };
            p.VirtualMachines.Add(new VirtualMachine { Name = "web1", CpuCount = 4, MemoryMB = 8192, HostName = "h1", PowerState = "poweredOn" });
            p.Hosts.Add(new HostNode { HostName = "h1", TotalCores = 32, MemoryMB = 262144, VmCount = 10, EsxVersion = "7.0" });

            var html = new HtmlReportGenerator().GenerateHtml(p);

            Assert.Contains("HtmlProj", html);
            Assert.Contains("web1", html);
            Assert.Contains("h1", html);
            Assert.Contains("<table>", html);
            Assert.Contains("<!DOCTYPE html>", html);
        }

        [Fact]
        public void GenerateToFile_WritesHtmlFile()
        {
            var p = new ProjectInfo { ProjectName = "P" };
            p.Servers.Add(new ServerNode { ServerName = "s1", CPUCount = 2, MemoryGB = 4 });
            var path = Path.Combine(Path.GetTempPath(), $"orp-{Guid.NewGuid():N}.html");
            try
            {
                new HtmlReportGenerator().GenerateToFile(p, path);
                Assert.True(File.Exists(path));
                Assert.Contains("s1", File.ReadAllText(path));
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        [Fact]
        public void ReportFormatFactory_PdfAndPptxAndHtml_Route()
        {
            var pptx = new ReportGeneratorService();
            var pdf = new QuestPdfReportGenerator();
            var html = new HtmlReportGenerator();
            var factory = new ReportFormatFactory(pptx, pdf, html);
            var p = new ProjectInfo { ProjectName = "route" };
            var dir = Path.Combine(Path.GetTempPath(), $"orp-route-{Guid.NewGuid():N}");
            Directory.CreateDirectory(dir);
            try
            {
                var htmlPath = Path.Combine(dir, "r.html");
                factory.Generate(p, htmlPath);
                Assert.True(File.Exists(htmlPath));

                var pdfPath = Path.Combine(dir, "r.pdf");
                factory.Generate(p, pdfPath);
                Assert.True(new FileInfo(pdfPath).Length > 50);

                Assert.Throws<NotSupportedException>(() => factory.Generate(p, Path.Combine(dir, "r.docx")));
            }
            finally
            {
                try { Directory.Delete(dir, true); } catch { }
            }
        }
    }
}

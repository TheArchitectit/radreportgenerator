using System.IO;
using OpenReportViewer.Core.Models;
using OpenReportViewer.Reporting;
using Xunit;

namespace OpenReportViewer.Tests.Reporting
{
    public class QuestPdfReportGeneratorTests
    {
        private readonly QuestPdfReportGenerator _generator = new();

        private static ProjectInfo CreateProject()
        {
            var p = new ProjectInfo { ProjectName = "PdfTest" };
            p.Servers.Add(new ServerNode { ServerName = "S1", OS = "Windows", CPUCount = 8, MemoryGB = 32 });
            p.Servers.Add(new ServerNode { ServerName = "S2", OS = "Linux", CPUCount = 16, MemoryGB = 64 });
            return p;
        }

        [Fact]
        public void Generate_WithValidProject_ReturnsPdfStream()
        {
            var project = CreateProject();

            using var stream = _generator.Generate(project);

            Assert.NotNull(stream);
            Assert.True(stream.Length > 100);
            var header = new byte[5];
            var pos = stream.Position;
            stream.Position = 0;
            stream.ReadExactly(header);
            stream.Position = pos;
            var magic = System.Text.Encoding.ASCII.GetString(header);
            Assert.Equal("%PDF-", magic);
        }

        [Fact]
        public void Generate_WithNullProject_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _generator.Generate(null!));
        }

        [Fact]
        public void GenerateToFile_WritesNonEmptyPdf()
        {
            var project = CreateProject();
            var path = Path.Combine(Path.GetTempPath(), $"orp-test-{Guid.NewGuid():N}.pdf");
            try
            {
                _generator.GenerateToFile(project, path);
                Assert.True(File.Exists(path));
                Assert.True(new FileInfo(path).Length > 100);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }
    }
}

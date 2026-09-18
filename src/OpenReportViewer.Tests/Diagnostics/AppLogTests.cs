using OpenReportViewer.Core.Diagnostics;
using Xunit;

namespace OpenReportViewer.Tests.Diagnostics
{
    public class AppLogTests
    {
        [Fact]
        public void FileAppLog_Info_WritesAndKeepsRecent()
        {
            var dir = Path.Combine(Path.GetTempPath(), $"orp-log-{Guid.NewGuid():N}");
            try
            {
                var log = new FileAppLog(dir);
                log.Info("hello world");
                log.Warn("careful");
                Assert.Contains(log.Recent, r => r.Contains("hello world"));
                var files = Directory.GetFiles(dir, "openreportviewer-*.log");
                Assert.Single(files);
                Assert.Contains("hello world", File.ReadAllText(files[0]));
            }
            finally
            {
                try { Directory.Delete(dir, true); } catch { }
            }
        }

        [Fact]
        public void DebugService_Trace_ReturnsRecent()
        {
            var log = new FileAppLog(Path.Combine(Path.GetTempPath(), $"orp-dbg-{Guid.NewGuid():N}"));
            var dbg = new DebugService(log);
            var t = dbg.Trace("Parse", "rvtools");
            Assert.True(t.Success);
            Assert.Equal("Parse", t.Name);
            Assert.NotEmpty(dbg.Recent);
        }
    }
}

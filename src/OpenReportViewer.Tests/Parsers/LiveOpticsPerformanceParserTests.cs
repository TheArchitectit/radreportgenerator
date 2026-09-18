using System.Data;
using OpenReportViewer.Core.Models;
using OpenReportViewer.Parsers;
using Xunit;

namespace OpenReportViewer.Tests.Parsers
{
    public class LiveOpticsPerformanceParserTests
    {
        private static DataSet BuildSynthetic()
        {
            var ds = new DataSet();
            var t = new DataTable("Performance_IO");
            t.Columns.Add("Timestamp", typeof(DateTime));
            t.Columns.Add("Server", typeof(string));
            t.Columns.Add("IOPS", typeof(double));
            t.Columns.Add("Throughput", typeof(double));
            t.Columns.Add("CPU %", typeof(double));
            t.Rows.Add(new DateTime(2026, 1, 1, 2, 0, 0), "srv1", 100, 10, 20);
            t.Rows.Add(new DateTime(2026, 1, 1, 3, 0, 0), "srv1", 4000, 80, 90);
            t.Rows.Add(new DateTime(2026, 1, 1, 4, 0, 0), "srv1", 200, 20, 30);
            ds.Tables.Add(t);
            return ds;
        }

        [Fact]
        public void Parse_WithTimestampedIops_PopulatesHistoryAndPeaks()
        {
            var project = new ProjectInfo { ProjectName = "perf" };
            project.Servers.Add(new ServerNode { ServerName = "srv1" });

            LiveOpticsPerformanceParser.Parse(BuildSynthetic(), project);

            var perf = project.Servers[0].Performance;
            Assert.Equal(3, perf.IoHistory.Count);
            Assert.Equal(4000, perf.PeakIOPS, 3);
            Assert.Equal(80, perf.PeakThroughputMBps, 3);
            Assert.Equal(3, perf.CpuHistory.Count);
        }

        [Fact]
        public void Parse_WithoutMetrics_LeavesHistoryEmpty()
        {
            var ds = new DataSet();
            var t = new DataTable("Notes");
            t.Columns.Add("Comment", typeof(string));
            t.Rows.Add("hello");
            ds.Tables.Add(t);

            var project = new ProjectInfo();
            project.Servers.Add(new ServerNode { ServerName = "s" });
            LiveOpticsPerformanceParser.Parse(ds, project);
            Assert.Empty(project.Servers[0].Performance.IoHistory);
            Assert.Equal(0, project.Servers[0].Performance.PeakIOPS);
        }

        [Fact]
        public void Parse_CreatesServerWhenColumnNamesServer()
        {
            var project = new ProjectInfo();
            LiveOpticsPerformanceParser.Parse(BuildSynthetic(), project);
            Assert.Contains(project.Servers, s => s.ServerName == "srv1");
        }
    }
}

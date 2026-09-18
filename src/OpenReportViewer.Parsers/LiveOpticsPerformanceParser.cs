using System.Data;
using System.Globalization;
using OpenReportViewer.Core.Diagnostics;
using OpenReportViewer.Core.Models;

namespace OpenReportViewer.Parsers
{
    public static class LiveOpticsPerformanceParser
    {
        /// <summary>
        /// Best-effort scan of Live Optics / Optical Prime style sheets for timestamped metrics.
        /// Sheet names vary by export version; detect by column headers rather than fixed names.
        /// </summary>
        public static void Parse(DataSet data, ProjectInfo project, IAppLog? log = null)
        {
            if (data == null || project == null) return;

            var seriesCount = 0;
            foreach (DataTable table in data.Tables)
            {
                if (table == null || table.Rows.Count == 0) continue;

                var timeCol = FindColumn(table, "Timestamp", "Time", "Date", "DateTime", "Sample Time", "Collection Time");
                var iopsCol = FindColumn(table, "IOPS", "Total IOPS", "IO/s", "IO Operations", "IOPS Total", "Read IOPS", "IOPS Read");
                var writeIopsCol = FindColumn(table, "Write IOPS", "IOPS Write");
                var throughputCol = FindColumn(table, "Throughput", "MBps", "MB/s", "Throughput MBps", "Total MBps");
                var cpuCol = FindColumn(table, "CPU", "CPU %", "CPU%", "CPU Usage", "CPU usage %");
                var serverCol = FindColumn(table, "Server", "Server Name", "Host", "Host Name", "VM", "Name", "ESX Host");

                var hasMetric = iopsCol != null || writeIopsCol != null || throughputCol != null || cpuCol != null;
                if (!hasMetric) continue;

                log?.Info($"Performance scan: table '{table.TableName}' time={timeCol ?? "-"} iops={iopsCol ?? "-"} thr={throughputCol ?? "-"} cpu={cpuCol ?? "-"}");

                foreach (DataRow row in table.Rows)
                {
                    var ts = ParseTimestamp(timeCol != null ? row[timeCol] : null);
                    if (ts == null && timeCol != null) continue;

                    var serverName = serverCol != null ? row[serverCol]?.ToString()?.Trim() ?? "" : "";
                    var target = ResolveServer(project, serverName);
                    if (target == null)
                    {
                        // Aggregate onto first synthetic profile bucket via project servers if named empty
                        if (project.Servers.Count == 0)
                        {
                            target = new ServerNode { ServerName = string.IsNullOrWhiteSpace(serverName) ? "Aggregate" : serverName };
                            project.Servers.Add(target);
                        }
                        else if (string.IsNullOrWhiteSpace(serverName))
                        {
                            target = project.Servers[0];
                        }
                        else
                        {
                            target = new ServerNode { ServerName = serverName };
                            project.Servers.Add(target);
                        }
                    }

                    var stamp = ts ?? DateTime.UtcNow;

                    if (iopsCol != null)
                    {
                        var v = ParseDouble(row[iopsCol]);
                        if (writeIopsCol != null) v += ParseDouble(row[writeIopsCol]);
                        if (v > target.Performance.PeakIOPS) target.Performance.PeakIOPS = v;
                        target.Performance.IoHistory.Add(new MetricPoint { Timestamp = stamp, Value = v });
                        seriesCount++;
                    }

                    if (throughputCol != null)
                    {
                        var v = ParseDouble(row[throughputCol]);
                        if (v > target.Performance.PeakThroughputMBps) target.Performance.PeakThroughputMBps = v;
                        seriesCount++;
                    }

                    if (cpuCol != null)
                    {
                        var v = ParseDouble(row[cpuCol]);
                        target.Performance.CpuHistory.Add(new MetricPoint { Timestamp = stamp, Value = v });
                        seriesCount++;
                    }
                }
            }

            if (seriesCount == 0)
            {
                project.Servers.ForEach(s => s.Performance.IoHistory.Clear());
                // leave peaks at 0 — no fabricated metrics
                log?.Info("LiveOptics performance parse: no timestamped metric columns found; history left empty.");
            }
            else
            {
                log?.Info($"LiveOptics performance parse: {seriesCount} metric samples mapped.");
            }
        }

        private static ServerNode? ResolveServer(ProjectInfo project, string serverName)
        {
            if (string.IsNullOrWhiteSpace(serverName)) return null;
            return project.Servers.FirstOrDefault(s =>
                string.Equals(s.ServerName, serverName, StringComparison.OrdinalIgnoreCase));
        }

        private static string? FindColumn(DataTable table, params string[] candidates)
        {
            foreach (var c in candidates)
            {
                foreach (DataColumn col in table.Columns)
                {
                    var n = col.ColumnName?.Trim() ?? "";
                    if (n.Equals(c, StringComparison.OrdinalIgnoreCase)) return col.ColumnName;
                }
            }
            foreach (var c in candidates)
            {
                foreach (DataColumn col in table.Columns)
                {
                    var n = col.ColumnName?.Trim() ?? "";
                    if (n.Contains(c, StringComparison.OrdinalIgnoreCase)) return col.ColumnName;
                }
            }
            return null;
        }

        private static DateTime? ParseTimestamp(object? raw)
        {
            if (raw == null) return null;
            if (raw is DateTime dt) return dt;
            var s = raw.ToString()?.Trim();
            if (string.IsNullOrWhiteSpace(s)) return null;
            if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var parsed))
                return parsed;
            return null;
        }

        private static double ParseDouble(object? raw)
        {
            if (raw == null) return 0;
            if (raw is double d) return d;
            if (raw is float f) return f;
            if (raw is int i) return i;
            if (raw is long l) return l;
            var s = raw.ToString()?.Trim();
            if (string.IsNullOrWhiteSpace(s)) return 0;
            return double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var v) ? v : 0;
        }
    }
}

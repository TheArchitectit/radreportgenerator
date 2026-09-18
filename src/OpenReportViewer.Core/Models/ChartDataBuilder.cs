namespace OpenReportViewer.Core.Models
{
    public enum ChartKind
    {
        Empty,
        BarCategories
    }

    public sealed class ChartSeries
    {
        public string Title { get; init; } = string.Empty;
        public ChartKind Kind { get; init; } = ChartKind.Empty;
        public IReadOnlyList<(string Label, double Value)> Points { get; init; } = Array.Empty<(string, double)>();
        public string ValueUnit { get; init; } = string.Empty;
        public string EmptyMessage { get; init; } = string.Empty;
    }

    public static class ChartDataBuilder
    {
        public static ChartSeries VmCpuTop(ProjectInfo p)
        {
            var pts = ProjectAggregates.TopVmCpu(p, 15);
            return new ChartSeries
            {
                Title = "Top VMs by CPU count",
                Kind = pts.Count == 0 ? ChartKind.Empty : ChartKind.BarCategories,
                Points = pts,
                ValueUnit = "cores",
                EmptyMessage = "No VM inventory in this export."
            };
        }

        public static ChartSeries VmMemoryTop(ProjectInfo p)
        {
            var pts = ProjectAggregates.TopVmMemoryMb(p, 15);
            return new ChartSeries
            {
                Title = "Top VMs by memory (MB)",
                Kind = pts.Count == 0 ? ChartKind.Empty : ChartKind.BarCategories,
                Points = pts,
                ValueUnit = "MB",
                EmptyMessage = "No VM inventory in this export."
            };
        }

        public static ChartSeries PartitionCapacityTop(ProjectInfo p)
        {
            var pts = ProjectAggregates.TopPartitionsCapacityMb(p, 15);
            return new ChartSeries
            {
                Title = "Top partitions by capacity (MB)",
                Kind = pts.Count == 0 ? ChartKind.Empty : ChartKind.BarCategories,
                Points = pts,
                ValueUnit = "MB",
                EmptyMessage = "No partition data in this export."
            };
        }

        public static ChartSeries IoHistory(ProjectInfo p)
        {
            // Aggregate any server IO history if present (Live Optics path; usually empty today)
            var pts = new List<(string Label, double Value)>();
            if (p?.Servers != null)
            {
                var idx = 0;
                foreach (var s in p.Servers)
                {
                    foreach (var pt in s.Performance?.IoHistory ?? new List<MetricPoint>())
                    {
                        pts.Add((pt.Timestamp.ToString("HH:mm"), pt.Value));
                        idx++;
                    }
                }
            }
            return new ChartSeries
            {
                Title = "IOPS history",
                Kind = pts.Count == 0 ? ChartKind.Empty : ChartKind.BarCategories,
                Points = pts,
                ValueUnit = "IOPS",
                EmptyMessage = "No performance series in this export yet."
            };
        }
    }
}

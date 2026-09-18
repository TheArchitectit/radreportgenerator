using System;
using System.Collections.Generic;

namespace LiveOptics.Core.Models
{
    public class ProjectInfo
    {
        public string ProjectName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public int ProjectId { get; set; }
        public List<ServerNode> Servers { get; set; } = new();
    }

    public class ServerNode
    {
        public string ServerName { get; set; } = string.Empty;
        public string OS { get; set; } = string.Empty;
        public int CPUCount { get; set; }
        public double MemoryGB { get; set; }
        public List<DiskDrive> Disks { get; set; } = new();
        public PerformanceProfile Performance { get; set; } = new();
    }

    public class DiskDrive
    {
        public string DiskName { get; set; } = string.Empty;
        public double CapacityGB { get; set; }
        public double FreeSpaceGB { get; set; }
    }

    public class PerformanceProfile
    {
        public double PeakIOPS { get; set; }
        public double PeakThroughputMBps { get; set; }
        public double AvgLatencyMs { get; set; }
        
        // Time-series data points for graphing
        public List<MetricPoint> IoHistory { get; set; } = new();
        public List<MetricPoint> CpuHistory { get; set; } = new();
    }

    public class MetricPoint
    {
        public DateTime Timestamp { get; set; }
        public double Value { get; set; }
    }
}

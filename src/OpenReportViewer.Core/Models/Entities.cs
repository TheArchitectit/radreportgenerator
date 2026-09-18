using System;
using System.Collections.Generic;

namespace OpenReportViewer.Core.Models
{
    public class ProjectInfo
    {
        public string ProjectName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public int ProjectId { get; set; }
        public string SourceType { get; set; } = "LiveOptics";
        public List<ServerNode> Servers { get; set; } = new();
        public List<VirtualMachine> VirtualMachines { get; set; } = new();
        public List<HostNode> Hosts { get; set; } = new();
        public List<PartitionInfo> Partitions { get; set; } = new();
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
        public List<MetricPoint> IoHistory { get; set; } = new();
        public List<MetricPoint> CpuHistory { get; set; } = new();
    }

    public class MetricPoint
    {
        public DateTime Timestamp { get; set; }
        public double Value { get; set; }
    }

    public class VirtualMachine
    {
        public string Name { get; set; } = string.Empty;
        public string PowerState { get; set; } = string.Empty;
        public bool IsTemplate { get; set; }
        public int CpuCount { get; set; }
        public double MemoryMB { get; set; }
        public int NicCount { get; set; }
        public int DiskCount { get; set; }
        public double ProvisionedMB { get; set; }
        public double InUseMB { get; set; }
        public string Datacenter { get; set; } = string.Empty;
        public string Cluster { get; set; } = string.Empty;
        public string HostName { get; set; } = string.Empty;
        public string OperatingSystem { get; set; } = string.Empty;
        public List<PartitionInfo> Partitions { get; set; } = new();
    }

    public class PartitionInfo
    {
        public string VmName { get; set; } = string.Empty;
        public string Disk { get; set; } = string.Empty;
        public double CapacityMB { get; set; }
        public double ConsumedMB { get; set; }
        public double FreeMB { get; set; }
        public double FreePercent { get; set; }
        public string HostName { get; set; } = string.Empty;
    }

    public class HostNode
    {
        public string HostName { get; set; } = string.Empty;
        public string Datacenter { get; set; } = string.Empty;
        public string Cluster { get; set; } = string.Empty;
        public string ConfigStatus { get; set; } = string.Empty;
        public string CpuModel { get; set; } = string.Empty;
        public int SocketCount { get; set; }
        public int CoresPerCpu { get; set; }
        public int TotalCores { get; set; }
        public double CpuUsagePercent { get; set; }
        public double MemoryMB { get; set; }
        public double MemoryUsagePercent { get; set; }
        public int VmCount { get; set; }
        public int VcpuCount { get; set; }
        public string EsxVersion { get; set; } = string.Empty;
        public string Vendor { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
    }

    public static class ProjectAggregates
    {
        public static int TotalVmCount(ProjectInfo p) => p?.VirtualMachines?.Count ?? 0;
        public static int TotalHostCount(ProjectInfo p) => p?.Hosts?.Count ?? 0;
        public static int TotalCpuCores(ProjectInfo p)
        {
            if (p == null) return 0;
            if (p.VirtualMachines.Count > 0)
                return p.VirtualMachines.Sum(v => v.CpuCount);
            return p.Servers.Sum(s => s.CPUCount);
        }

        public static double TotalMemoryMb(ProjectInfo p)
        {
            if (p == null) return 0;
            if (p.VirtualMachines.Count > 0)
                return p.VirtualMachines.Sum(v => v.MemoryMB);
            return p.Servers.Sum(s => s.MemoryGB) * 1024.0;
        }

        public static double TotalProvisionedMb(ProjectInfo p) =>
            p?.VirtualMachines?.Sum(v => v.ProvisionedMB) ?? 0;

        public static double TotalInUseMb(ProjectInfo p) =>
            p?.VirtualMachines?.Sum(v => v.InUseMB) ?? 0;

        public static List<(string Label, double Value)> TopVmCpu(ProjectInfo p, int take = 15) =>
            (p?.VirtualMachines ?? new List<VirtualMachine>())
                .OrderByDescending(v => v.CpuCount)
                .ThenBy(v => v.Name)
                .Take(take)
                .Select(v => (v.Name, (double)v.CpuCount))
                .ToList();

        public static List<(string Label, double Value)> TopVmMemoryMb(ProjectInfo p, int take = 15) =>
            (p?.VirtualMachines ?? new List<VirtualMachine>())
                .OrderByDescending(v => v.MemoryMB)
                .ThenBy(v => v.Name)
                .Take(take)
                .Select(v => (v.Name, v.MemoryMB))
                .ToList();

        public static List<(string Label, double Value)> TopPartitionsCapacityMb(ProjectInfo p, int take = 15)
        {
            var parts = p?.Partitions ?? new List<PartitionInfo>();
            if (parts.Count == 0 && p?.VirtualMachines != null)
                parts = p.VirtualMachines.SelectMany(v => v.Partitions).ToList();
            return parts
                .OrderByDescending(x => x.CapacityMB)
                .Take(take)
                .Select(x => ($"{x.VmName} {x.Disk}", x.CapacityMB))
                .ToList();
        }
    }
}

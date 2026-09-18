using Xunit;
using OpenReportViewer.Core.Models;
using System;

namespace OpenReportViewer.Tests.Models
{
    public class ProjectInfoTests
    {
        [Fact]
        public void Constructor_InitializesWithDefaults()
        {
            var project = new ProjectInfo();

            Assert.NotNull(project.Servers);
            Assert.Empty(project.Servers);
            Assert.Equal(string.Empty, project.ProjectName);
            Assert.Equal(default(DateTime), project.CreatedDate);
            Assert.Equal(default(int), project.ProjectId);
        }

        [Fact]
        public void Servers_CanAddServer()
        {
            var project = new ProjectInfo();
            var server = new ServerNode { ServerName = "Test" };

            project.Servers.Add(server);

            Assert.Single(project.Servers);
            Assert.Equal(server, project.Servers[0]);
        }

        [Fact]
        public void Servers_CanAddMultipleServers()
        {
            var project = new ProjectInfo();
            project.Servers.Add(new ServerNode { ServerName = "S1" });
            project.Servers.Add(new ServerNode { ServerName = "S2" });
            project.Servers.Add(new ServerNode { ServerName = "S3" });

            Assert.Equal(3, project.Servers.Count);
        }

        [Fact]
        public void ProjectName_CanBeSet()
        {
            var project = new ProjectInfo { ProjectName = "My Project" };

            Assert.Equal("My Project", project.ProjectName);
        }

        [Fact]
        public void ProjectName_CanSetEmptyString()
        {
            var project = new ProjectInfo { ProjectName = "" };

            Assert.Equal("", project.ProjectName);
        }

        [Fact]
        public void ProjectName_CanBeNull()
        {
            var project = new ProjectInfo { ProjectName = null! };

            Assert.Null(project.ProjectName);
        }

        [Fact]
        public void CreatedDate_CanBeSet()
        {
            var date = DateTime.Now;
            var project = new ProjectInfo { CreatedDate = date };

            Assert.Equal(date, project.CreatedDate);
        }
    }

    public class ServerNodeTests
    {
        [Fact]
        public void Constructor_InitializesWithDefaults()
        {
            var server = new ServerNode();

            Assert.NotNull(server.Disks);
            Assert.NotNull(server.Performance);
            Assert.Empty(server.Disks);
            Assert.Empty(server.Performance.IoHistory);
            Assert.Empty(server.Performance.CpuHistory);
            Assert.Equal(string.Empty, server.ServerName);
            Assert.Equal(string.Empty, server.OS);
            Assert.Equal(0, server.CPUCount);
            Assert.Equal(0.0, server.MemoryGB);
        }

        [Fact]
        public void ServerName_CanBeSet()
        {
            var server = new ServerNode { ServerName = "MyServer" };

            Assert.Equal("MyServer", server.ServerName);
        }

        [Fact]
        public void CPUCount_CanBeSet()
        {
            var server = new ServerNode { CPUCount = 32 };

            Assert.Equal(32, server.CPUCount);
        }

        [Fact]
        public void MemoryGB_CanBeSet()
        {
            var server = new ServerNode { MemoryGB = 256.5 };

            Assert.Equal(256.5, server.MemoryGB);
        }

        [Fact]
        public void Disks_CanAddDisk()
        {
            var server = new ServerNode();
            var disk = new DiskDrive { DiskName = "Disk0" };

            server.Disks.Add(disk);

            Assert.Single(server.Disks);
        }

        [Fact]
        public void Disks_CanAddMultipleDisks()
        {
            var server = new ServerNode();
            server.Disks.Add(new DiskDrive { DiskName = "D0" });
            server.Disks.Add(new DiskDrive { DiskName = "D1" });

            Assert.Equal(2, server.Disks.Count);
        }
    }

    public class DiskDriveTests
    {
        [Fact]
        public void Constructor_InitializesWithDefaults()
        {
            var disk = new DiskDrive();

            Assert.Equal(string.Empty, disk.DiskName);
            Assert.Equal(0.0, disk.CapacityGB);
            Assert.Equal(0.0, disk.FreeSpaceGB);
        }

        [Fact]
        public void CapacityGB_CanBeSet()
        {
            var disk = new DiskDrive { CapacityGB = 1024 };

            Assert.Equal(1024, disk.CapacityGB);
        }

        [Fact]
        public void FreeSpaceGB_CanBeSet()
        {
            var disk = new DiskDrive { FreeSpaceGB = 512 };

            Assert.Equal(512, disk.FreeSpaceGB);
        }
    }

    public class PerformanceProfileTests
    {
        [Fact]
        public void Constructor_InitializesWithDefaults()
        {
            var perf = new PerformanceProfile();

            Assert.NotNull(perf.IoHistory);
            Assert.NotNull(perf.CpuHistory);
            Assert.Empty(perf.IoHistory);
            Assert.Empty(perf.CpuHistory);
            Assert.Equal(0.0, perf.PeakIOPS);
            Assert.Equal(0.0, perf.PeakThroughputMBps);
            Assert.Equal(0.0, perf.AvgLatencyMs);
        }

        [Fact]
        public void PeakIOPS_CanBeSet()
        {
            var perf = new PerformanceProfile { PeakIOPS = 50000 };

            Assert.Equal(50000, perf.PeakIOPS);
        }

        [Fact]
        public void IoHistory_CanAddMetricPoint()
        {
            var perf = new PerformanceProfile();
            var point = new MetricPoint { Timestamp = DateTime.Now, Value = 100 };

            perf.IoHistory.Add(point);

            Assert.Single(perf.IoHistory);
        }

        [Fact]
        public void CpuHistory_CanAddMultipleMetricPoints()
        {
            var perf = new PerformanceProfile();
            perf.CpuHistory.Add(new MetricPoint { Timestamp = DateTime.Now, Value = 50 });
            perf.CpuHistory.Add(new MetricPoint { Timestamp = DateTime.Now, Value = 75 });

            Assert.Equal(2, perf.CpuHistory.Count);
        }
    }

    public class MetricPointTests
    {
        [Fact]
        public void Constructor_InitializesWithDefaults()
        {
            var point = new MetricPoint();

            Assert.Equal(default(DateTime), point.Timestamp);
            Assert.Equal(0.0, point.Value);
        }

        [Fact]
        public void Timestamp_CanBeSet()
        {
            var now = DateTime.Now;
            var point = new MetricPoint { Timestamp = now };

            Assert.Equal(now, point.Timestamp);
        }

        [Fact]
        public void Value_CanBeSet()
        {
            var point = new MetricPoint { Value = 123.45 };

            Assert.Equal(123.45, point.Value);
        }

        [Fact]
        public void Value_CanBeNegative()
        {
            var point = new MetricPoint { Value = -10 };

            Assert.Equal(-10, point.Value);
        }

        [Fact]
        public void Value_CanBeZero()
        {
            var point = new MetricPoint { Value = 0 };

            Assert.Equal(0, point.Value);
        }
    }
}

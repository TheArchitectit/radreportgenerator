using OpenReportViewer.Core.Models;
using Xunit;

namespace OpenReportViewer.Tests.Models
{
    public class ProjectAggregatesTests
    {
        [Fact]
        public void Aggregates_FromVmInventory_ComputeTotalsAndTops()
        {
            var p = new ProjectInfo { SourceType = "RVTools", ProjectName = "agg" };
            p.VirtualMachines.Add(new VirtualMachine { Name = "a", CpuCount = 4, MemoryMB = 1024, ProvisionedMB = 100, InUseMB = 50 });
            p.VirtualMachines.Add(new VirtualMachine { Name = "b", CpuCount = 8, MemoryMB = 2048, ProvisionedMB = 200, InUseMB = 80 });
            p.Hosts.Add(new HostNode { HostName = "h1", TotalCores = 32, MemoryMB = 262144 });
            p.Partitions.Add(new PartitionInfo { VmName = "b", Disk = "C:\\", CapacityMB = 500 });

            Assert.Equal(2, ProjectAggregates.TotalVmCount(p));
            Assert.Equal(1, ProjectAggregates.TotalHostCount(p));
            Assert.Equal(12, ProjectAggregates.TotalCpuCores(p));
            Assert.Equal(3072, ProjectAggregates.TotalMemoryMb(p), 3);
            Assert.Equal(300, ProjectAggregates.TotalProvisionedMb(p), 3);
            Assert.Equal("b", ProjectAggregates.TopVmCpu(p, 1)[0].Label);
            Assert.Equal(2048, ProjectAggregates.TopVmMemoryMb(p, 1)[0].Value, 3);
            Assert.NotEmpty(ProjectAggregates.TopPartitionsCapacityMb(p, 5));
        }

        [Fact]
        public void Aggregates_LegacyServerList_StillWorks()
        {
            var p = new ProjectInfo();
            p.Servers.Add(new ServerNode { CPUCount = 16, MemoryGB = 64 });
            Assert.Equal(16, ProjectAggregates.TotalCpuCores(p));
            Assert.Equal(64 * 1024, ProjectAggregates.TotalMemoryMb(p), 3);
        }
    }
}

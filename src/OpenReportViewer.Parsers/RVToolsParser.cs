using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using ExcelDataReader;
using OpenReportViewer.Core.Interfaces;
using OpenReportViewer.Core.Models;

namespace OpenReportViewer.Parsers
{
    public class RVToolsParser : IDataParser
    {
        public bool CanParse(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                return false;
            if (!filePath.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase) &&
                !filePath.EndsWith(".xls", StringComparison.OrdinalIgnoreCase))
                return false;

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = ExcelReaderFactory.CreateReader(stream);
            var result = reader.AsDataSet(Config());
            return result.Tables.Contains("vInfo") || result.Tables.Contains("vHost");
        }

        public ProjectInfo Parse(string filePath) => ParseFile(filePath);

        public ProjectInfo ParseFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Excel file not found", filePath);

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = ExcelReaderFactory.CreateReader(stream);
            var ds = reader.AsDataSet(Config());

            var project = new ProjectInfo
            {
                SourceType = "RVTools",
                ProjectName = Path.GetFileNameWithoutExtension(filePath)
            };

            if (ds.Tables.Contains("vHost") && ds.Tables["vHost"] is DataTable hostTable)
                ParseHosts(hostTable, project);

            if (ds.Tables.Contains("vInfo") && ds.Tables["vInfo"] is DataTable vmTable)
                ParseVms(vmTable, project);

            if (ds.Tables.Contains("vPartition") && ds.Tables["vPartition"] is DataTable partTable)
                ParsePartitions(partTable, project);

            // Materialize hosts as ServerNode list for legacy dashboard KPIs
            foreach (var h in project.Hosts)
            {
                project.Servers.Add(new ServerNode
                {
                    ServerName = h.HostName,
                    OS = h.EsxVersion,
                    CPUCount = h.TotalCores > 0 ? h.TotalCores : h.SocketCount * h.CoresPerCpu,
                    MemoryGB = h.MemoryMB / 1024.0
                });
            }

            return project;
        }

        private static ExcelDataSetConfiguration Config() => new()
        {
            ConfigureDataTable = _ => new ExcelDataTableConfiguration
            {
                UseHeaderRow = true
            }
        };

        private static void ParseHosts(DataTable table, ProjectInfo project)
        {
            foreach (DataRow row in table.Rows)
            {
                var host = new HostNode
                {
                    HostName = Str(row, "Host"),
                    Datacenter = Str(row, "Datacenter"),
                    Cluster = Str(row, "Cluster"),
                    ConfigStatus = Str(row, "Config status"),
                    CpuModel = Str(row, "CPU Model"),
                    SocketCount = Int(row, "# CPU"),
                    CoresPerCpu = Int(row, "Cores per CPU"),
                    TotalCores = Int(row, "# Cores"),
                    CpuUsagePercent = Dbl(row, "CPU usage %"),
                    MemoryMB = Dbl(row, "# Memory"),
                    MemoryUsagePercent = Dbl(row, "Memory usage %"),
                    VmCount = Int(row, "# VMs"),
                    VcpuCount = Int(row, "# vCPUs"),
                    EsxVersion = Str(row, "ESX Version"),
                    Vendor = Str(row, "Vendor"),
                    Model = Str(row, "Model")
                };
                if (string.IsNullOrWhiteSpace(host.HostName)) continue;
                project.Hosts.Add(host);
            }
        }

        private static void ParseVms(DataTable table, ProjectInfo project)
        {
            foreach (DataRow row in table.Rows)
            {
                var name = Str(row, "VM");
                if (string.IsNullOrWhiteSpace(name)) continue;

                var os = Str(row, "OS according to the configuration file");
                if (string.IsNullOrWhiteSpace(os))
                    os = Str(row, "OS according to the VMware Tools");

                var vm = new VirtualMachine
                {
                    Name = name,
                    PowerState = Str(row, "Powerstate"),
                    IsTemplate = bool.TryParse(Str(row, "Template"), out var t) && t,
                    CpuCount = Int(row, "CPUs"),
                    MemoryMB = Dbl(row, "Memory"),
                    NicCount = Int(row, "NICs"),
                    DiskCount = Int(row, "Disks"),
                    ProvisionedMB = Dbl(row, "Provisioned MB"),
                    InUseMB = Dbl(row, "In Use MB"),
                    Datacenter = Str(row, "Datacenter"),
                    Cluster = Str(row, "Cluster"),
                    HostName = Str(row, "Host"),
                    OperatingSystem = os
                };
                project.VirtualMachines.Add(vm);
            }
        }

        private static void ParsePartitions(DataTable table, ProjectInfo project)
        {
            var byVm = new Dictionary<string, VirtualMachine>(StringComparer.OrdinalIgnoreCase);
            foreach (var vm in project.VirtualMachines)
                byVm[vm.Name] = vm;

            foreach (DataRow row in table.Rows)
            {
                var vmName = Str(row, "VM");
                if (string.IsNullOrWhiteSpace(vmName)) continue;

                var part = new PartitionInfo
                {
                    VmName = vmName,
                    Disk = Str(row, "Disk"),
                    CapacityMB = Dbl(row, "Capacity MB"),
                    ConsumedMB = Dbl(row, "Consumed MB"),
                    FreeMB = Dbl(row, "Free MB"),
                    FreePercent = Dbl(row, "Free %"),
                    HostName = Str(row, "Host")
                };
                // column may be "Free % " with trailing space
                if (part.FreePercent == 0)
                    part.FreePercent = Dbl(row, "Free % ");

                project.Partitions.Add(part);
                if (byVm.TryGetValue(vmName, out var vm))
                    vm.Partitions.Add(part);
            }
        }

        private static string Str(DataRow row, string col)
        {
            if (!row.Table.Columns.Contains(col)) return string.Empty;
            return row[col]?.ToString()?.Trim() ?? string.Empty;
        }

        private static int Int(DataRow row, string col)
        {
            var s = Str(row, col);
            if (string.IsNullOrWhiteSpace(s)) return 0;
            return int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i) ? i : 0;
        }

        private static double Dbl(DataRow row, string col)
        {
            var s = Str(row, col);
            if (string.IsNullOrWhiteSpace(s)) return 0;
            return double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var d) ? d : 0;
        }
    }

    public class ParserFactory
    {
        private readonly IEnumerable<IDataParser> _parsers;
        private readonly OpenReportViewer.Core.Diagnostics.IAppLog? _log;

        public ParserFactory(IEnumerable<IDataParser> parsers, OpenReportViewer.Core.Diagnostics.IAppLog? log = null)
        {
            _parsers = parsers;
            _log = log;
        }

        public IDataParser Resolve(string filePath)
        {
            foreach (var p in _parsers)
            {
                if (p.CanParse(filePath)) return p;
            }
            _log?.Warn($"No parser matched: {filePath}");
            throw new InvalidOperationException($"No parser can handle file: {filePath}");
        }

        public ProjectInfo Parse(string filePath)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var parser = Resolve(filePath);
            var project = parser.Parse(filePath);
            sw.Stop();
            _log?.Info($"Parsed {Path.GetFileName(filePath)} via {parser.GetType().Name} in {sw.ElapsedMilliseconds}ms; VMs={project.VirtualMachines.Count} hosts={project.Hosts.Count} servers={project.Servers.Count}");
            return project;
        }
    }
}

namespace OpenReportViewer.Parsers
{
    using Microsoft.Extensions.DependencyInjection;
    using OpenReportViewer.Core.Interfaces;

    public static class RvToolsServiceCollectionExtensions
    {
        public static IServiceCollection AddOpenReportViewerRvTools(this IServiceCollection services)
        {
            services.AddSingleton<RVToolsParser>();
            services.AddSingleton<IDataParser>(sp => sp.GetRequiredService<RVToolsParser>());
            services.AddSingleton<ParserFactory>();
            return services;
        }
    }
}

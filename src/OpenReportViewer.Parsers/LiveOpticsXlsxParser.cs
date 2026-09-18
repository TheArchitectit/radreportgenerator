using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using ExcelDataReader;
using OpenReportViewer.Core.Models;
using OpenReportViewer.Core.Interfaces;

namespace OpenReportViewer.Parsers
{
    public interface ILiveOpticsParser
    {
        ProjectInfo ParseFile(string filePath);
    }

    public class LiveOpticsXlsxParser : ILiveOpticsParser, IDataParser
    {
        public bool CanParse(string filePath)
        {
            return !string.IsNullOrWhiteSpace(filePath)
                && filePath.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase);
        }

        public ProjectInfo Parse(string filePath) => ParseFile(filePath);

        public ProjectInfo ParseFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException("Excel file not found", filePath);

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            var project = new ProjectInfo();
            
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                });

                // 1. Parse Project Info (Assuming a "Project" or "Summary" tab exists)
                if (result.Tables.Contains("Project Info"))
                {
                    var table = result.Tables["Project Info"];
                    if (table != null && table.Rows.Count > 0 && table.Columns.Contains("Project Name"))
                    {
                        var projectName = table.Rows[0]["Project Name"];
                        project.ProjectName = projectName?.ToString() ?? "Unknown";
                        // ... parsing other project fields
                    }
                }

                // 2. Parse Server Inventory
                if (result.Tables.Contains("Server Inventory"))
                {
                    var table = result.Tables["Server Inventory"];
                    if (table != null)
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            try
                            {
                                var server = new ServerNode
                                {
                                    ServerName = table.Columns.Contains("Server Name")
                                        ? row["Server Name"]?.ToString() ?? string.Empty
                                        : string.Empty,
                                    OS = table.Columns.Contains("OS") ? row["OS"]?.ToString() ?? "Unknown" : "Unknown",
                                };

                                if (table.Columns.Contains("CPU Count") && int.TryParse(row["CPU Count"]?.ToString(), out int cpu))
                                    server.CPUCount = cpu;
                                if (table.Columns.Contains("Total Memory (GB)") && double.TryParse(row["Total Memory (GB)"]?.ToString(), out double mem))
                                    server.MemoryGB = mem;

                                project.Servers.Add(server);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error parsing server row: {ex.Message}");
                            }
                        }
                    }
                }
                
                // 3. Parse Performance Data usually found in "Aggregated Data" or specific server tabs
                // This is simplified; real structure depends on specific Live Optics report version
                ParsePerformanceData(result, project);
            }

            return project;
        }

        private void ParsePerformanceData(DataSet data, ProjectInfo project)
        {
            // Performance series not implemented yet (see openspec/changes/liveoptics-performance-data).
            // Keep history empty so the UI can show an empty state instead of fabricated metrics.
            Console.WriteLine("LiveOpticsXlsxParser: performance series parse not implemented; history left empty.");
        }
    }
}

namespace OpenReportViewer.Parsers
{
    using Microsoft.Extensions.DependencyInjection;
    using OpenReportViewer.Core.Interfaces;

    public static class ParserServiceCollectionExtensions
    {
        public static IServiceCollection AddOpenReportViewerParsers(this IServiceCollection services)
        {
            services.AddSingleton<LiveOpticsXlsxParser>();
            services.AddSingleton<ILiveOpticsParser>(sp => sp.GetRequiredService<LiveOpticsXlsxParser>());
            services.AddSingleton<IDataParser>(sp => sp.GetRequiredService<LiveOpticsXlsxParser>());
            return services;
        }
    }
}

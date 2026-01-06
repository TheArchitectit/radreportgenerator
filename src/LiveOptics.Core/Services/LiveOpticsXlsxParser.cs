using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using ExcelDataReader;
using LiveOptics.Core.Models;

namespace LiveOptics.Core.Services
{
    public interface ILiveOpticsParser
    {
        ProjectInfo ParseFile(string filePath);
    }

    public class LiveOpticsXlsxParser : ILiveOpticsParser
    {
        public ProjectInfo ParseFile(string filePath)
        {
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
                    if (table.Rows.Count > 0)
                    {
                        project.ProjectName = table.Rows[0]["Project Name"]?.ToString() ?? "Unknown";
                        // ... parsing other project fields
                    }
                }

                // 2. Parse Server Inventory
                if (result.Tables.Contains("Server Inventory"))
                {
                    var table = result.Tables["Server Inventory"];
                    foreach (DataRow row in table.Rows)
                    {
                        var server = new ServerNode
                        {
                            ServerName = row["Server Name"]?.ToString() ?? "Unknown",
                            OS = row["OS"]?.ToString() ?? "Unknown",
                        };

                        if (int.TryParse(row["CPU Count"]?.ToString(), out int cpu)) server.CPUCount = cpu;
                        if (double.TryParse(row["Total Memory (GB)"]?.ToString(), out double mem)) server.MemoryGB = mem;

                        project.Servers.Add(server);
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
            // Placeholder logic: iterate tables looking for timestamped data
            // real implementation would filter by known table names like "Performance_IO"
        }
    }
}

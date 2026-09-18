using Xunit;
using OpenReportViewer.Parsers;
using OpenReportViewer.Reporting;
using OpenReportViewer.AI;
using OpenReportViewer.Core.Interfaces;
using System;
using System.IO;
using System.Linq;

namespace OpenReportViewer.Tests.Services
{
    public class LiveOpticsXlsxParserTests : IDisposable
    {
        private readonly LiveOpticsXlsxParser _parser;
        private readonly string _testFilesPath;

        public LiveOpticsXlsxParserTests()
        {
            _parser = new LiveOpticsXlsxParser();
            _testFilesPath = Path.Combine(Directory.GetCurrentDirectory(), "TestData");
            Directory.CreateDirectory(_testFilesPath);
        }

        public void Dispose()
        {
            if (Directory.Exists(_testFilesPath))
            {
                try
                {
                    Directory.Delete(_testFilesPath, true);
                }
                catch
                {
                }
            }
        }

        [Fact]
        public void ParseFile_WithNullPath_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _parser.ParseFile(null!));
        }

        [Fact]
        public void ParseFile_WithEmptyPath_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _parser.ParseFile(""));
        }

        [Fact]
        public void ParseFile_WithWhitespacePath_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _parser.ParseFile("   "));
        }

        [Fact]
        public void ParseFile_WithNonExistentFile_ThrowsFileNotFoundException()
        {
            var path = Path.Combine(_testFilesPath, "nonexistent.xlsx");
            Assert.Throws<FileNotFoundException>(() => _parser.ParseFile(path));
        }

        [Fact]
        public void ParseFile_WithInvalidExtension_ThrowsException()
        {
            var path = Path.Combine(_testFilesPath, "test.txt");
            File.WriteAllText(path, "not an excel file");

            try
            {
                _parser.ParseFile(path);
                Assert.Fail("Expected exception for invalid file");
            }
            catch
            {
                File.Delete(path);
            }
        }

        [Fact]
        public void ParseFile_WithEmptyExcelFile_ReturnsProjectWithDefaults()
        {
            var path = CreateMinimalExcelFile("empty.xlsx");

            try
            {
                var result = _parser.ParseFile(path);

                Assert.NotNull(result);
                Assert.NotNull(result.Servers);
                Assert.Empty(result.Servers);
                Assert.Equal(string.Empty, result.ProjectName);
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Fact]
        public void ParseFile_WithProjectInfoSheet_ParsesProjectName()
        {
            var path = CreateExcelWithProjectInfo("with_project.xlsx", "Test Project");

            try
            {
                var result = _parser.ParseFile(path);

                Assert.NotNull(result);
                Assert.Equal("Test Project", result.ProjectName);
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Fact]
        public void ParseFile_WithMissingProjectNameColumn_UsesUnknown()
        {
            var path = CreateExcelWithoutColumnName("no_col.xlsx");

            try
            {
                var result = _parser.ParseFile(path);

                Assert.NotNull(result);
                Assert.Equal(string.Empty, result.ProjectName);
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Fact]
        public void ParseFile_WithServerInventorySheet_ParsesServers()
        {
            var path = CreateExcelWithServers("with_servers.xlsx");

            try
            {
                var result = _parser.ParseFile(path);

                Assert.NotNull(result);
                Assert.Equal(2, result.Servers.Count);
                Assert.Equal("Server1", result.Servers[0].ServerName);
                Assert.Equal("Server2", result.Servers[1].ServerName);
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Fact]
        public void ParseFile_WithNullServerData_ParsesSuccessfully()
        {
            var path = CreateExcelWithNullServerData("null_servers.xlsx");

            try
            {
                var result = _parser.ParseFile(path);

                Assert.NotNull(result);
                Assert.Equal(2, result.Servers.Count);
                Assert.Equal(string.Empty, result.Servers[0].ServerName);
                Assert.Equal(0, result.Servers[1].CPUCount);
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Fact]
        public void ParseFile_WithMalformedData_ParsesValidRowsOnly()
        {
            var path = CreateExcelWithMixedData("mixed.xlsx");

            try
            {
                var result = _parser.ParseFile(path);

                Assert.NotNull(result);
                Assert.True(result.Servers.Count >= 1);
            }
            finally
            {
                File.Delete(path);
            }
        }

        private string CreateMinimalExcelFile(string filename)
        {
            var path = Path.Combine(_testFilesPath, filename);
            
            using (var fs = new FileStream(path, FileMode.Create))
            using (var package = new OfficeOpenXml.ExcelPackage(fs))
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                package.Save();
            }

            return path;
        }

        private string CreateExcelWithProjectInfo(string filename, string projectName)
        {
            var path = Path.Combine(_testFilesPath, filename);
            
            using (var fs = new FileStream(path, FileMode.Create))
            using (var package = new OfficeOpenXml.ExcelPackage(fs))
            {
                var worksheet = package.Workbook.Worksheets.Add("Project Info");
                worksheet.Cells["A1"].Value = "Project Name";
                worksheet.Cells["A2"].Value = projectName;
                package.Save();
            }

            return path;
        }

        private string CreateExcelWithoutColumnName(string filename)
        {
            var path = Path.Combine(_testFilesPath, filename);
            
            using (var fs = new FileStream(path, FileMode.Create))
            using (var package = new OfficeOpenXml.ExcelPackage(fs))
            {
                var worksheet = package.Workbook.Worksheets.Add("Project Info");
                worksheet.Cells["A1"].Value = "Some Other Column";
                worksheet.Cells["A2"].Value = "Value";
                package.Save();
            }

            return path;
        }

        private string CreateExcelWithServers(string filename)
        {
            var path = Path.Combine(_testFilesPath, filename);
            
            using (var fs = new FileStream(path, FileMode.Create))
            using (var package = new OfficeOpenXml.ExcelPackage(fs))
            {
                var worksheet = package.Workbook.Worksheets.Add("Server Inventory");
                worksheet.Cells["A1"].Value = "Server Name";
                worksheet.Cells["B1"].Value = "OS";
                worksheet.Cells["C1"].Value = "CPU Count";
                worksheet.Cells["D1"].Value = "Total Memory (GB)";
                
                worksheet.Cells["A2"].Value = "Server1";
                worksheet.Cells["B2"].Value = "Windows Server 2019";
                worksheet.Cells["C2"].Value = 16;
                worksheet.Cells["D2"].Value = 64;
                
                worksheet.Cells["A3"].Value = "Server2";
                worksheet.Cells["B3"].Value = "Linux";
                worksheet.Cells["C3"].Value = 32;
                worksheet.Cells["D3"].Value = 128;
                
                package.Save();
            }

            return path;
        }

        private string CreateExcelWithNullServerData(string filename)
        {
            var path = Path.Combine(_testFilesPath, filename);
            
            using (var fs = new FileStream(path, FileMode.Create))
            using (var package = new OfficeOpenXml.ExcelPackage(fs))
            {
                var worksheet = package.Workbook.Worksheets.Add("Server Inventory");
                worksheet.Cells["A1"].Value = "Server Name";
                worksheet.Cells["B1"].Value = "OS";
                worksheet.Cells["C1"].Value = "CPU Count";
                worksheet.Cells["D1"].Value = "Total Memory (GB)";
                
                worksheet.Cells["A2"].Value = "";
                worksheet.Cells["B2"].Value = "";
                worksheet.Cells["C2"].Value = "";
                worksheet.Cells["D2"].Value = "";
                
                worksheet.Cells["A3"].Value = "Server2";
                worksheet.Cells["B3"].Value = "Linux";
                worksheet.Cells["C3"].Value = "";
                worksheet.Cells["D3"].Value = "";
                
                package.Save();
            }

            return path;
        }

        private string CreateExcelWithMixedData(string filename)
        {
            var path = Path.Combine(_testFilesPath, filename);
            
            using (var fs = new FileStream(path, FileMode.Create))
            using (var package = new OfficeOpenXml.ExcelPackage(fs))
            {
                var worksheet = package.Workbook.Worksheets.Add("Server Inventory");
                worksheet.Cells["A1"].Value = "Server Name";
                worksheet.Cells["B1"].Value = "OS";
                worksheet.Cells["C1"].Value = "CPU Count";
                worksheet.Cells["D1"].Value = "Total Memory (GB)";
                
                worksheet.Cells["A2"].Value = "Server1";
                worksheet.Cells["B2"].Value = "Windows";
                worksheet.Cells["C2"].Value = "16";
                worksheet.Cells["D2"].Value = "64";
                
                package.Save();
            }

            return path;
        }
    }
}

using Xunit;
using OpenReportViewer.Core.Models;
using OpenReportViewer.Core.Services;
using System;
using System.IO;
using System.Linq;

namespace OpenReportViewer.Tests.Services
{
    public class ReportGeneratorServiceTests : IDisposable
    {
        private readonly ReportGeneratorService _service;
        private readonly string _testOutputPath;

        public ReportGeneratorServiceTests()
        {
            _service = new ReportGeneratorService();
            _testOutputPath = Path.Combine(Directory.GetCurrentDirectory(), "TestOutput");
            Directory.CreateDirectory(_testOutputPath);
        }

        public void Dispose()
        {
            if (Directory.Exists(_testOutputPath))
            {
                try
                {
                    Directory.Delete(_testOutputPath, true);
                }
                catch
                {
                }
            }
        }

        [Fact]
        public void GeneratePresentation_WithValidProject_CreatesFile()
        {
            var project = CreateTestProject("TestProject");
            var outputPath = Path.Combine(_testOutputPath, "test.pptx");

            try
            {
                _service.GeneratePresentation(project, outputPath);

                Assert.True(File.Exists(outputPath));
                Assert.True(new FileInfo(outputPath).Length > 0);
            }
            finally
            {
                if (File.Exists(outputPath))
                    File.Delete(outputPath);
            }
        }

        [Fact]
        public void GeneratePresentation_WithNullProject_ThrowsArgumentNullException()
        {
            var outputPath = Path.Combine(_testOutputPath, "test.pptx");

            try
            {
                var ex = Assert.Throws<ArgumentNullException>(() => _service.GeneratePresentation(null!, outputPath));
                Assert.Equal("project", ex.ParamName);
            }
            finally
            {
                if (File.Exists(outputPath))
                    File.Delete(outputPath);
            }
        }

        [Fact]
        public void GeneratePresentation_WithEmptyPath_CreatesFileInCurrentDirectory()
        {
            var project = CreateTestProject("TestProject");
            var outputPath = "test_output.pptx";

            try
            {
                _service.GeneratePresentation(project, outputPath);

                Assert.True(File.Exists(outputPath));
            }
            finally
            {
                if (File.Exists(outputPath))
                    File.Delete(outputPath);
            }
        }

        [Fact]
        public void GeneratePresentation_WithNonExistentDirectory_ThrowsDirectoryNotFound()
        {
            var project = CreateTestProject("TestProject");
            var outputPath = Path.Combine(_testOutputPath, "SubDir", "test.pptx");

            Assert.Throws<DirectoryNotFoundException>(() => _service.GeneratePresentation(project, outputPath));
        }

        [Fact]
        public void GeneratePresentation_WithMultipleServers_CalculatesTotals()
        {
            var project = new ProjectInfo
            {
                ProjectName = "MultiServer",
                Servers = new List<ServerNode>
                {
                    new ServerNode { CPUCount = 16, MemoryGB = 64 },
                    new ServerNode { CPUCount = 32, MemoryGB = 128 },
                    new ServerNode { CPUCount = 8, MemoryGB = 32 }
                }
            };
            var outputPath = Path.Combine(_testOutputPath, "test.pptx");

            try
            {
                _service.GeneratePresentation(project, outputPath);

                Assert.True(File.Exists(outputPath));
            }
            finally
            {
                if (File.Exists(outputPath))
                    File.Delete(outputPath);
            }
        }

        [Fact]
        public void GeneratePresentation_WithEmptyServers_DoesNotThrow()
        {
            var project = new ProjectInfo
            {
                ProjectName = "NoServers",
                Servers = new List<ServerNode>()
            };
            var outputPath = Path.Combine(_testOutputPath, "test.pptx");

            try
            {
                _service.GeneratePresentation(project, outputPath);

                Assert.True(File.Exists(outputPath));
            }
            finally
            {
                if (File.Exists(outputPath))
                    File.Delete(outputPath);
            }
        }

        [Fact]
        public void GeneratePresentation_WithNullServerList_UsesZeroInSummary()
        {
            var project = new ProjectInfo
            {
                ProjectName = "NullServers",
                Servers = null!
            };
            var outputPath = Path.Combine(_testOutputPath, "test.pptx");

            try
            {
                _service.GeneratePresentation(project, outputPath);

                Assert.True(File.Exists(outputPath));
            }
            finally
            {
                if (File.Exists(outputPath))
                    File.Delete(outputPath);
            }
        }

        [Fact]
        public void GeneratePresentation_WithLongProjectName_CreatesValidFile()
        {
            var longName = new string('A', 500);
            var project = CreateTestProject(longName);
            var outputPath = Path.Combine(_testOutputPath, "test.pptx");

            try
            {
                _service.GeneratePresentation(project, outputPath);

                Assert.True(File.Exists(outputPath));
            }
            finally
            {
                if (File.Exists(outputPath))
                    File.Delete(outputPath);
            }
        }

        [Fact]
        public void GeneratePresentation_WithSpecialCharacters_CreatesValidFile()
        {
            var specialName = "Project <Test> & \"Quotes\" | Pipes";
            var project = CreateTestProject(specialName);
            var outputPath = Path.Combine(_testOutputPath, "test.pptx");

            try
            {
                _service.GeneratePresentation(project, outputPath);

                Assert.True(File.Exists(outputPath));
            }
            finally
            {
                if (File.Exists(outputPath))
                    File.Delete(outputPath);
            }
        }

        [Fact]
        public void GeneratePresentation_OverwritesExistingFile()
        {
            var project = CreateTestProject("TestProject");
            var outputPath = Path.Combine(_testOutputPath, "test.pptx");

            try
            {
                _service.GeneratePresentation(project, outputPath);
                var firstSize = new FileInfo(outputPath).Length;

                _service.GeneratePresentation(project, outputPath);
                var secondSize = new FileInfo(outputPath).Length;

                Assert.True(File.Exists(outputPath));
                Assert.True(secondSize > 0);
            }
            finally
            {
                if (File.Exists(outputPath))
                    File.Delete(outputPath);
            }
        }

        private ProjectInfo CreateTestProject(string projectName)
        {
            return new ProjectInfo
            {
                ProjectName = projectName,
                Servers = new List<ServerNode>
                {
                    new ServerNode
                    {
                        ServerName = "Server1",
                        OS = "Windows Server 2019",
                        CPUCount = 16,
                        MemoryGB = 64
                    }
                }
            };
        }
    }
}

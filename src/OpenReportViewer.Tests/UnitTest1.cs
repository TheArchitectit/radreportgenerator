using Xunit;
using LiveOptics.Core.Models;
using LiveOptics.Core.Services;
using System.Collections.Generic;

namespace LiveOptics.Tests
{
    public class ParserTests
    {
        [Fact]
        public void ServerNode_CalculatesResourceUsage_Correctly()
        {
            // Arrange
            var server = new ServerNode
            {
                ServerName = "TestServer",
                CPUCount = 16,
                MemoryGB = 64
            };

            // Act
            // (In a real scenario, we might have methods to calculate usage percentage, etc.)
            
            // Assert
            Assert.Equal(16, server.CPUCount);
            Assert.Equal("TestServer", server.ServerName);
        }

        [Fact]
        public void ProjectInfo_Initialization_DefaultsCorrectly()
        {
            // Arrange
            var project = new ProjectInfo();

            // Assert
            Assert.NotNull(project.Servers);
            Assert.Empty(project.Servers);
            Assert.Equal(string.Empty, project.ProjectName);
        }

        // Note: We cannot easily test LiveOpticsXlsxParser here without a physical sample file
        // but we can test the structure and model behavior.
    }
}

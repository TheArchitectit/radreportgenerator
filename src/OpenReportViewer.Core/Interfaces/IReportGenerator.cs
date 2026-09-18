using OpenReportViewer.Core.Models;

namespace OpenReportViewer.Core.Interfaces
{
    public interface IReportGenerator
    {
        string Format { get; }
        void Generate(ProjectInfo project, string outputPath);
    }
}

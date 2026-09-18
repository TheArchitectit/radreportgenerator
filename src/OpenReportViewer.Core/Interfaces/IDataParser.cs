using OpenReportViewer.Core.Models;

namespace OpenReportViewer.Core.Interfaces
{
    public interface IDataParser
    {
        bool CanParse(string filePath);
        ProjectInfo Parse(string filePath);
    }
}

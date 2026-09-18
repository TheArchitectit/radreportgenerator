namespace OpenReportViewer.Core.Interfaces
{
    public interface IAnalysisService
    {
        Task<string> AnalyzePerformanceAsync(string query);
        Task<string> ResearchHardwareAsync(string hardwareModel);
        bool IsDemoProvider { get; }
    }
}

using System;
using System.Threading.Tasks;

namespace OpenReportViewer.AI
{
    public interface IResearchAgent
    {
        Task<string> AnalyzePerformanceAsync(string query);
        Task<string> ResearchHardwareAsync(string hardwareModel);
    }

    public class ResearchAgentService : IResearchAgent, OpenReportViewer.Core.Interfaces.IAnalysisService
    {
        // In a real app, this would perform HTTP requests to an LLM API (OpenAI/Anthropic/Gemini)
        
        public bool IsDemoProvider => true;

        public async Task<string> AnalyzePerformanceAsync(string query)
        {
            // Simulate agent "thinking"
            await Task.Delay(1000); 
            
            return $"[AI Analysis] Based on the IOPS pattern '{query}', the system shows signs of latency spikes during backup windows (2:00 AM - 4:00 AM). Recommendation: Review backup scheduling or move to Flash tier.";
        }

        public async Task<string> ResearchHardwareAsync(string hardwareModel)
        {
            await Task.Delay(1000);
            
            // Mock database of hardware knowledge
            return $"[AI Research] {hardwareModel}: Released ~2017. Specs: Intel Xeon Scalable (Skylake). EOSL expected 2025. Upgrade recommended for workloads > 50k IOPS.";
        }
    }
}

namespace OpenReportViewer.AI
{
    using Microsoft.Extensions.DependencyInjection;
    using OpenReportViewer.Core.Interfaces;

    public static class AiServiceCollectionExtensions
    {
        public static IServiceCollection AddOpenReportViewerAI(this IServiceCollection services)
        {
            services.AddSingleton<ResearchAgentService>();
            services.AddSingleton<IResearchAgent>(sp => sp.GetRequiredService<ResearchAgentService>());
            services.AddSingleton<IAnalysisService>(sp => sp.GetRequiredService<ResearchAgentService>());
            return services;
        }
    }
}

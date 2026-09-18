using Microsoft.Extensions.DependencyInjection;
using OpenReportViewer.Core.Interfaces;

namespace OpenReportViewer.Core.Configuration
{
    public static class ServiceConfiguration
    {
        /// <summary>
        /// Registers OpenReportViewer services. Call module Add* methods for concrete implementations.
        /// </summary>
        public static IServiceCollection AddOpenReportViewerCore(this IServiceCollection services)
        {
            return services;
        }
    }
}

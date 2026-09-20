using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using OpenReportViewer.AI;
using OpenReportViewer.Core.Configuration;
using OpenReportViewer.Core.Diagnostics;
using OpenReportViewer.Parsers;
using OpenReportViewer.Reporting;
using OpenReportViewer.UI.Wpf.ViewModels;

namespace OpenReportViewer.UI.Wpf
{
    public partial class App : Application
    {
        private ServiceProvider? _services;

        public IServiceProvider Services => _services ?? throw new InvalidOperationException("DI not initialized");

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var sc = new ServiceCollection();
            sc.AddOpenReportViewerCore();
            sc.AddOpenReportViewerParsers();
            sc.AddOpenReportViewerReporting();
            sc.AddOpenReportViewerPdf();
            sc.AddOpenReportViewerHtml();
            sc.AddOpenReportViewerLogging();
            sc.AddOpenReportViewerAI();
            sc.AddOpenReportViewerHeuristicAi();
            sc.AddSingleton<MainViewModel>();
            _services = sc.BuildServiceProvider();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _services?.Dispose();
            base.OnExit(e);
        }
    }
}

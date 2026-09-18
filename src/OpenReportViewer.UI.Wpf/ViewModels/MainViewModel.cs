using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using OpenReportViewer.Core.Models;
using OpenReportViewer.Core.Interfaces;
using OpenReportViewer.Parsers;
using OpenReportViewer.AI;
using OpenReportViewer.Reporting;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OpenReportViewer.UI.Wpf.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly LiveOpticsXlsxParser _parser;
        private readonly IAnalysisService _researchAgent;
        private readonly IPptxReportGenerator _reportGenerator;
        private readonly IPdfReportGenerator? _pdfGenerator;

        private ProjectInfo? _currentProject;
        private string _statusMessage = "Ready";
        private bool _isBusy;

        public MainViewModel()
            : this(new LiveOpticsXlsxParser(), new ResearchAgentService(), new ReportGeneratorService(), new QuestPdfReportGenerator())
        {
        }

        public MainViewModel(
            LiveOpticsXlsxParser parser,
            IAnalysisService researchAgent,
            IPptxReportGenerator reportGenerator,
            IPdfReportGenerator? pdfGenerator = null)
        {
            _parser = parser;
            _researchAgent = researchAgent;
            _reportGenerator = reportGenerator;
            _pdfGenerator = pdfGenerator;

            LoadFileCommand = new RelayCommand(LoadFile);
            GenerateReportCommand = new RelayCommand(GenerateReport, _ => _currentProject != null);
            AnalyzeWithAiCommand = new RelayCommand(async _ => await AnalyzeAsync(), _ => _currentProject != null);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public bool IsDemoAi => _researchAgent.IsDemoProvider;

        public string AiProviderLabel => _researchAgent.IsDemoProvider
            ? "AI Research Agent (demo insights)"
            : "AI Research Agent";

        public ProjectInfo? CurrentProject
        {
            get => _currentProject;
            set
            {
                if (SetProperty(ref _currentProject, value))
                {
                    OnPropertyChanged(nameof(ProjectName));
                    OnPropertyChanged(nameof(ServerCount));
                    UpdateCharts();
                }
            }
        }

        public string ProjectName => _currentProject?.ProjectName ?? "No Project Loaded";
        public int ServerCount => _currentProject?.Servers?.Count ?? 0;

        public ISeries[] IOPSSeries { get; set; } = Array.Empty<ISeries>();
        public ISeries[] ThroughputSeries { get; set; } = Array.Empty<ISeries>();
        public string ChartEmptyMessage { get; set; } = "Load a Live Optics export with performance series to plot charts.";

        public ObservableCollection<string> AiInsights { get; } = new();

        public ICommand LoadFileCommand { get; }
        public ICommand GenerateReportCommand { get; }
        public ICommand AnalyzeWithAiCommand { get; }

        private async void LoadFile(object? param)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Live Optics Excel|*.xlsx|All Files|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    IsBusy = true;
                    StatusMessage = "Parsing file...";

                    var project = await Task.Run(() => _parser.ParseFile(dialog.FileName));
                    CurrentProject = project;
                    StatusMessage = "Loaded " + (project?.ProjectName ?? "Unknown");
                }
                catch (Exception ex)
                {
                    StatusMessage = "Error: " + ex.Message;
                    MessageBox.Show($"Failed to load file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private async void GenerateReport(object? param)
        {
            if (_currentProject == null) return;

            var dialog = new SaveFileDialog
            {
                Filter = "PDF report|*.pdf|PowerPoint Presentation|*.pptx",
                FileName = $"Report_{_currentProject.ProjectName}_{DateTime.Now:yyyyMMdd}.pdf"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    IsBusy = true;
                    var ext = Path.GetExtension(dialog.FileName).ToLowerInvariant();
                    if (ext == ".pdf")
                    {
                        if (_pdfGenerator == null)
                        {
                            StatusMessage = "PDF generator not registered";
                            return;
                        }
                        StatusMessage = "Generating PDF...";
                        await Task.Run(() => _pdfGenerator.GenerateToFile(_currentProject, dialog.FileName));
                    }
                    else
                    {
                        StatusMessage = "Generating PPTX...";
                        await Task.Run(() => _reportGenerator.GeneratePresentation(_currentProject, dialog.FileName));
                    }
                    StatusMessage = "Report Generated: " + dialog.FileName;
                }
                catch (Exception ex)
                {
                    StatusMessage = "Error: " + ex.Message;
                    MessageBox.Show($"Failed to generate report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private async Task AnalyzeAsync()
        {
            if (_currentProject == null) return;

            try
            {
                IsBusy = true;
                StatusMessage = _researchAgent.IsDemoProvider
                    ? "AI Agent Requesting Analysis (demo)..."
                    : "AI Agent Requesting Analysis...";

                AiInsights.Clear();

                if (_researchAgent.IsDemoProvider)
                {
                    AiInsights.Add("[DEMO] Insights below are simulated and not produced by a live LLM.");
                }

                var analysis = await _researchAgent.AnalyzePerformanceAsync("High Latency detected on Disk 0");
                AiInsights.Add(analysis);

                if (_currentProject.Servers != null)
                {
                    foreach (var server in _currentProject.Servers)
                    {
                        if (server.CPUCount > 32)
                        {
                            var hardwareResearch = await _researchAgent.ResearchHardwareAsync("High Core Count Server");
                            AiInsights.Add($"Server {server.ServerName}: {hardwareResearch}");
                        }
                    }
                }

                StatusMessage = "Analysis Complete";
            }
            catch (Exception ex)
            {
                StatusMessage = "Analysis Error: " + ex.Message;
                MessageBox.Show($"AI Analysis failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void UpdateCharts()
        {
            // Do not fabricate series. Real performance parsing is tracked in
            // openspec/changes/liveoptics-performance-data and qa-fix-dummy-charts.
            IOPSSeries = Array.Empty<ISeries>();
            ThroughputSeries = Array.Empty<ISeries>();
            ChartEmptyMessage = "No performance series in this export yet (parser stub).";
            OnPropertyChanged(nameof(IOPSSeries));
            OnPropertyChanged(nameof(ThroughputSeries));
            OnPropertyChanged(nameof(ChartEmptyMessage));
        }
    }
}

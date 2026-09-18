using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using OpenReportViewer.Core.Models;
using OpenReportViewer.Core.Interfaces;
using OpenReportViewer.Parsers;
using OpenReportViewer.AI;
using OpenReportViewer.Reporting;
using OpenReportViewer.Core.Diagnostics;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OpenReportViewer.UI.Wpf.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ParserFactory _parserFactory;
        private readonly IAnalysisService _researchAgent;
        private readonly IPptxReportGenerator _reportGenerator;
        private readonly IPdfReportGenerator? _pdfGenerator;
        private readonly ReportFormatFactory? _formatFactory;
        private readonly IAppLog? _log;

        private ProjectInfo? _currentProject;
        private string _statusMessage = "Ready";
        private bool _isBusy;

        public MainViewModel()
            : this(
                new ParserFactory(new IDataParser[] { new RVToolsParser(), new LiveOpticsXlsxParser() }),
                new ResearchAgentService(),
                new ReportGeneratorService(),
                new QuestPdfReportGenerator(), formatFactory: null, log: null)
        {
        }

        public MainViewModel(
            ParserFactory parserFactory,
            IAnalysisService researchAgent,
            IPptxReportGenerator reportGenerator,
            IPdfReportGenerator? pdfGenerator = null,
            ReportFormatFactory? formatFactory = null,
            IAppLog? log = null)
        {
            _parserFactory = parserFactory;
            _researchAgent = researchAgent;
            _reportGenerator = reportGenerator;
            _pdfGenerator = pdfGenerator;
            _formatFactory = formatFactory;
            _log = log;

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
                    OnPropertyChanged(nameof(VmCount));
                    OnPropertyChanged(nameof(HostCount));
                    OnPropertyChanged(nameof(SourceType));
                    UpdateCharts();
                }
            }
        }

        public string ProjectName => _currentProject?.ProjectName ?? "No Project Loaded";
        public int ServerCount => _currentProject?.Servers?.Count ?? 0;
        public int VmCount => _currentProject == null ? 0 : ProjectAggregates.TotalVmCount(_currentProject);
        public int HostCount => _currentProject == null ? 0 : ProjectAggregates.TotalHostCount(_currentProject);
        public string SourceType => _currentProject?.SourceType ?? "—";

        public ISeries[] IOPSSeries { get; set; } = Array.Empty<ISeries>();
        public ISeries[] ThroughputSeries { get; set; } = Array.Empty<ISeries>();
        public string ChartEmptyMessage { get; set; } = "Load an RVTools or Live Optics .xlsx export.";
        public string ChartTopTitle { get; set; } = "Top metrics";
        public string ChartBottomTitle { get; set; } = "Storage / performance";

        public ObservableCollection<string> AiInsights { get; } = new();

        public ICommand LoadFileCommand { get; }
        public ICommand GenerateReportCommand { get; }
        public ICommand AnalyzeWithAiCommand { get; }

        private async void LoadFile(object? param)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Excel exports|*.xlsx;*.xls|All Files|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    IsBusy = true;
                    StatusMessage = "Parsing file...";

                    var project = await Task.Run(() => _parserFactory.Parse(dialog.FileName));
                    CurrentProject = project;
                    StatusMessage = $"Loaded {project?.ProjectName ?? "Unknown"} ({project?.SourceType ?? "?"})";
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
                Filter = "PDF report|*.pdf|PowerPoint Presentation|*.pptx|HTML report|*.html",
                FileName = $"Report_{_currentProject.ProjectName}_{DateTime.Now:yyyyMMdd}.pdf"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    IsBusy = true;
                    var ext = Path.GetExtension(dialog.FileName).ToLowerInvariant();
                    if (ext == ".html" || ext == ".htm")
                    {
                        if (_formatFactory == null)
                        {
                            StatusMessage = "HTML generator not registered";
                            return;
                        }
                        StatusMessage = "Generating HTML...";
                        await Task.Run(() => _formatFactory.Generate(_currentProject, dialog.FileName));
                        _log?.Info($"HTML report written: {dialog.FileName}");
                    }
                    else if (ext == ".pdf")
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

                var analysis = await _researchAgent.AnalyzePerformanceAsync(
                    $"Source={_currentProject.SourceType}, VMs={VmCount}, Hosts={HostCount}");
                AiInsights.Add(analysis);

                var hosts = _currentProject.Hosts;
                if (hosts is { Count: > 0 })
                {
                    foreach (var host in hosts.Where(h => h.TotalCores >= 32 || h.VmCount >= 20).Take(5))
                    {
                        var hardwareResearch = await _researchAgent.ResearchHardwareAsync(
                            string.IsNullOrWhiteSpace(host.CpuModel) ? "High density host" : host.CpuModel);
                        AiInsights.Add($"Host {host.HostName} ({host.VmCount} VMs, {host.TotalCores} cores): {hardwareResearch}");
                    }
                }
                else if (_currentProject.Servers != null)
                {
                    foreach (var server in _currentProject.Servers.Where(s => s.CPUCount > 32).Take(5))
                    {
                        var hardwareResearch = await _researchAgent.ResearchHardwareAsync("High Core Count Server");
                        AiInsights.Add($"Server {server.ServerName}: {hardwareResearch}");
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
            var p = _currentProject;
            if (p == null)
            {
                IOPSSeries = Array.Empty<ISeries>();
                ThroughputSeries = Array.Empty<ISeries>();
                ChartEmptyMessage = "Load an RVTools or Live Optics .xlsx export.";
                OnPropertyChanged(nameof(IOPSSeries));
                OnPropertyChanged(nameof(ThroughputSeries));
                OnPropertyChanged(nameof(ChartEmptyMessage));
                return;
            }

            var topCpu = ChartDataBuilder.VmCpuTop(p);
            var topMem = ChartDataBuilder.VmMemoryTop(p);
            var topPart = ChartDataBuilder.PartitionCapacityTop(p);

            ChartTopTitle = topCpu.Title;
            ChartBottomTitle = topPart.Kind != ChartKind.Empty ? topPart.Title : topMem.Title;

            IOPSSeries = ToColumnSeries(topCpu, SKColors.RoyalBlue);
            ThroughputSeries = ToColumnSeries(
                topPart.Kind != ChartKind.Empty ? topPart : topMem,
                SKColors.Teal);

            ChartEmptyMessage =
                (topCpu.Kind == ChartKind.Empty && topPart.Kind == ChartKind.Empty && topMem.Kind == ChartKind.Empty)
                    ? (p.SourceType == "RVTools"
                        ? "RVTools file parsed but no VM/host metrics found."
                        : "No performance series in this Live Optics export yet.")
                    : $"Source: {p.SourceType} · VMs: {VmCount} · Hosts: {HostCount}";

            OnPropertyChanged(nameof(IOPSSeries));
            OnPropertyChanged(nameof(ThroughputSeries));
            OnPropertyChanged(nameof(ChartEmptyMessage));
            OnPropertyChanged(nameof(ChartTopTitle));
            OnPropertyChanged(nameof(ChartBottomTitle));
        }

        private static ISeries[] ToColumnSeries(ChartSeries series, SKColor color)
        {
            if (series.Kind == ChartKind.Empty || series.Points.Count == 0)
                return Array.Empty<ISeries>();

            var labels = series.Points.Select(p => p.Label).ToArray();
            var values = series.Points.Select(p => p.Value).ToArray();

            return new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Name = series.Title,
                    Values = values,
                    Fill = new SolidColorPaint(color),
                    DataLabelsPaint = new SolidColorPaint(SKColors.Black),
                    DataLabelsFormatter = point => point.Model.ToString("0.#")
                }
            };
        }
    }
}

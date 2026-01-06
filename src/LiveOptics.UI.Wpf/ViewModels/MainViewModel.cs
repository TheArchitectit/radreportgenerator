using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.Win32;
using LiveOptics.Core.Models;
using LiveOptics.Core.Services;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LiveOptics.UI.Wpf.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly LiveOpticsXlsxParser _parser;
        private readonly IResearchAgent _researchAgent;
        private readonly IReportGenerator _reportGenerator;
        
        private ProjectInfo? _currentProject;
        private string _statusMessage = "Ready";
        private bool _isBusy;

        public MainViewModel()
        {
            _parser = new LiveOpticsXlsxParser();
            _researchAgent = new ResearchAgentService(); // DI would be better here
            _reportGenerator = new ReportGeneratorService();

            LoadFileCommand = new RelayCommand(LoadFile);
            GenerateReportCommand = new RelayCommand(GenerateReport, _ => _currentProject != null);
            AnalyzeWithAiCommand = new RelayCommand(async _ => await AnalyzeAsync(), _ => _currentProject != null);
        }

        // Properties
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

        public ProjectInfo? CurrentProject
        {
            get => _currentProject;
            set 
            {
                if(SetProperty(ref _currentProject, value))
                {
                    OnPropertyChanged(nameof(ProjectName));
                    OnPropertyChanged(nameof(ServerCount));
                    UpdateCharts();
                }
            }
        }

        public string ProjectName => _currentProject?.ProjectName ?? "No Project Loaded";
        public int ServerCount => _currentProject?.Servers?.Count ?? 0;

        // Charting
        public ISeries[] IOPSSeries { get; set; } = Array.Empty<ISeries>();
        public ISeries[] ThroughputSeries { get; set; } = Array.Empty<ISeries>();

        public ObservableCollection<string> AiInsights { get; } = new();

        // Commands
        public ICommand LoadFileCommand { get; }
        public ICommand GenerateReportCommand { get; }
        public ICommand AnalyzeWithAiCommand { get; }

        private void LoadFile(object? param)
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
                    
                    // Run on background thread to keep UI responsive
                    Task.Run(() => 
                    {
                        var project = _parser.ParseFile(dialog.FileName);
                        Application.Current.Dispatcher.Invoke(() => 
                        {
                            CurrentProject = project;
                            StatusMessage = "Loaded " + project.ProjectName;
                            IsBusy = false;
                        });
                    });
                }
                catch (Exception ex)
                {
                    StatusMessage = "Error: " + ex.Message;
                    IsBusy = false;
                }
            }
        }

        private void GenerateReport(object? param)
        {
            if (_currentProject == null) return;

            var dialog = new SaveFileDialog
            {
                Filter = "PowerPoint Presentation|*.pptx",
                FileName = $"Report_{_currentProject.ProjectName}_{DateTime.Now:yyyyMMdd}.pptx"
            };

            if (dialog.ShowDialog() == true)
            {
                StatusMessage = "Generating PPTX...";
                _reportGenerator.GeneratePresentation(_currentProject, dialog.FileName);
                StatusMessage = "Report Genrated: " + dialog.FileName;
            }
        }

        private async Task AnalyzeAsync()
        {
            if (_currentProject == null) return;

            IsBusy = true;
            StatusMessage = "AI Agent Requesting Analysis...";
            
            AiInsights.Clear();

            // Simulate sending project metrics to AI
            var analysis = await _researchAgent.AnalyzePerformanceAsync("High Latency detected on Disk 0");
            AiInsights.Add(analysis);

            foreach(var server in _currentProject.Servers)
            {
                if(server.CPUCount > 32)
                {
                    var hardwareResearch = await _researchAgent.ResearchHardwareAsync("High Core Count Server");
                    AiInsights.Add($"Server {server.ServerName}: {hardwareResearch}");
                }
            }

            StatusMessage = "Analysis Complete";
            IsBusy = false;
        }

        private void UpdateCharts()
        {
            // Dummy data for visualization if parsing didn't find time-series
            IOPSSeries = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = new double[] { 200, 500, 1200, 800, 1500, 4000, 2000 },
                    Name = "Total IOPS",
                    Fill = null
                }
            };
            
            ThroughputSeries = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = new double[] { 100, 250, 600, 400, 750, 2000, 1000 },
                    Name = "Throughput (MB/s)",
                    Fill = null
                }
            };
            
            OnPropertyChanged(nameof(IOPSSeries));
            OnPropertyChanged(nameof(ThroughputSeries));
        }
    }
}

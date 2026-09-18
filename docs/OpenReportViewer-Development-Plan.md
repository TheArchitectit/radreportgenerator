# OpenReportViewer - Refactoring & Development Plan

## Executive Summary

Building a production-ready, modular report generator that ingests LiveOptics & RVTools data, generates PDF reports with bar charts, integrates GitHub Copilot AI, and includes comprehensive debugging infrastructure.

**Key Features:**
- Modular plugin-based architecture
- RVTools and LiveOptics data ingestion
- PDF report generation with bar chart visualizations
- GitHub Copilot AI integration for insights
- Comprehensive debugging and logging
- 90%+ test coverage

---

## Architecture Overview

### Project Structure

```
src/
├── OpenReportViewer.Core/              # Domain models & interfaces
├── OpenReportViewer.Parsers/           # Data source parsers
├── OpenReportViewer.Visualizations/    # Chart generation engine
├── OpenReportViewer.Generators/        # PDF, PPTX, HTML generators
├── OpenReportViewer.AI.Copilot/        # GitHub Copilot integration
├── OpenReportViewer.UI.Wpf/           # WPF application
└── OpenReportViewer.Tests/            # Comprehensive test suite

debug/
├── OpenReportViewer.Debugger/          # Interactive debugging tool
└── OpenReportViewer.Benchmarks/        # Performance profiling
```

### Technology Stack

- **.NET 8.0** with C# 12
- **WPF** with MVVM pattern
- **LiveCharts2** for interactive charts
- **QuestPDF** or **PdfSharp** for PDF generation
- **ExcelDataReader** for Excel parsing
- **Microsoft.Extensions.DependencyInjection** for DI
- **GitHub Copilot API** for AI insights
- **xUnit** for testing
- **Serilog** for structured logging

---

## Phase 0: Foundation & Tooling (Week 1)

### Goals
- Set up modular architecture
- Implement comprehensive debugging
- Create extensible plugin system
- Configure dependency injection

### Key Components

#### 0.1 Debug Infrastructure

```csharp
public interface IDebugService
{
    Task<ParseDebugInfo> TraceParseAsync(string filePath);
    Task<ChartDebugInfo> TraceChartGenerationAsync(ChartData data);
    Task<ReportDebugInfo> TraceReportGenerationAsync(IReportData data);
    Task<PerformanceMetrics> ProfileOperationAsync(string operationName);
}
```

**Debug Dashboard Features:**
- Step-through debugging for parsers
- Chart generation performance metrics
- Memory allocation tracking
- File I/O operation logs
- AI prompt/response logging (sanitized)

#### 0.2 Dependency Injection Container

```csharp
public static class ServiceConfiguration
{
    public static IServiceCollection AddOpenReportViewer(this IServiceCollection services)
    {
        // Core services
        services.AddSingleton<IDebugService, DebugService>();
        services.AddSingleton<ILogger>(sp => new LoggerConfiguration()
            .WriteTo.File("logs/debug-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger());
        
        // Parsers
        services.AddTransient<IDataParser, RVToolsParser>();
        services.AddTransient<IDataParser, LiveOpticsParser>();
        services.AddTransient<ParserFactory>();
        
        // Visualizations
        services.AddTransient<IChartProvider, BarChartProvider>();
        services.AddTransient<IChartProvider, LineChartProvider>();
        services.AddTransient<ChartFactory>();
        
        // Generators
        services.AddTransient<IReportGenerator, PdfReportGenerator>();
        services.AddTransient<IReportGenerator, PowerPointReportGenerator>();
        
        // AI
        services.AddTransient<IAnalysisService, CopilotAnalysisService>();
        
        return services;
    }
}
```

---

## Phase 1: Parser Ecosystem (Weeks 2-3)

### RVTools Parser Implementation

**Data Structure Analysis (from docs/SizingWorkshop-RVTools.xlsx):**
- **vInfo**: 805 VMs × 14 columns
- **vPartition**: 1,618 partitions × 13 columns
- **vHost**: 34 hosts × 28 columns

**Parser Interface:**
```csharp
public interface IDataParser
{
    bool CanParse(string filePath);
    Task<IReportData> ParseAsync(string filePath, CancellationToken token = default);
}
```

**RVTools Parser Features:**
- Parse vInfo (VM inventory, power state, resources)
- Parse vPartition (disk usage, free space)
- Parse vHost (ESXi host configuration)
- Cross-reference VMs to hosts
- Calculate aggregate metrics
- Validate data completeness

**Example Parse Flow:**
```csharp
public async Task<IReportData> ParseAsync(string filePath, CancellationToken token)
{
    Debug.Log($"Parsing RVTools: {Path.GetFileName(filePath)}");
    
    var vms = await ParseVInfoAsync(filePath, token);
    Debug.Log($"✓ Parsed {vms.Count} VMs");
    
    var partitions = await ParseVPartitionAsync(filePath, token);
    Debug.Log($"✓ Parsed {partitions.Count} partitions");
    
    var hosts = await ParseVHostAsync(filePath, token);
    Debug.Log($"✓ Parsed {hosts.Count} hosts");
    
    return MergeData(vms, partitions, hosts);
}
```

### Data Models

```csharp
public interface IReportData
{
    string SourceType { get; }
    DateTime ExportedDate { get; }
    List<IVirtualMachine> VirtualMachines { get; }
    List<IHost> Hosts { get; }
    List<IDatastore> Datastores { get; }
    Dictionary<string, object> Metadata { get; }
}

public interface IVirtualMachine
{
    string Name { get; }
    PowerState PowerState { get; }
    int CpuCount { get; }
    long MemoryMB { get; }
    long ProvisionedMB { get; }
    long InUseMB { get; }
    string OperatingSystem { get; }
    string Datacenter { get; }
    string Cluster { get; }
    string HostName { get; }
    List<IPartition> Partitions { get; }
}
```

---

## Phase 2: Visualization Engine (Week 4)

### Chart Provider System

```csharp
public interface IChartProvider
{
    ChartType SupportedType { get; }
    IChart CreateChart(ChartData data, ChartOptions options);
}

public enum ChartType
{
    Bar,
    Column,
    Line,
    Pie,
    StackedBar
}
```

### Bar Chart Gallery (Prioritized for PDF)

**1. VM Resource Overview**
- Top 20 VMs by CPU Count (Horizontal Bar)
- Top 20 VMs by Memory Allocation (Horizontal Bar)
- VM Distribution by Power State (Stacked Bar)

**2. Storage Analysis**
- Top 15 Largest Partitions (Horizontal Bar)
- Capacity vs Free Space (Grouped Bar)
- Free Space % Distribution (Column)

**3. Host Infrastructure**
- Host CPU Utilization (Bar)
- Host Memory Usage (Stacked Bar)
- VMs per Host Distribution (Column)

**4. Snapshot Analysis**
- Snapshot Age Distribution (Bar)
- VM Snapshot Count (Bar)
- Largest Snapshots (Horizontal Bar)

### Chart Generation Example

```csharp
var chartData = new ChartData
{
    Title = "Top 15 VMs by CPU Count",
    Categories = vmNames.Take(15).ToList(),
    Values = vmCpuCounts.Take(15).ToList(),
    Options = new BarChartOptions
    {
        Orientation = BarOrientation.Horizontal,
        SortOrder = SortOrder.Descending,
        ColorPalette = "HighContrast",
        ShowGridLines = true,
        ShowValueLabels = true,
        ValueLabelFormat = "{0} cores"
    }
};

var chart = chartFactory.CreateChart(ChartType.Bar, chartData);
```

### Chart Export

All charts exportable as:
- SVG (vector, lossless)
- PNG (raster, embedded in PDF)
- Base64 (for HTML reports)

---

## Phase 3: PDF Report Generator (Week 5) - HIGHEST PRIORITY

### PDF Architecture

```csharp
public class PdfReportGenerator : IReportGenerator
{
    public string Format => "PDF";
    
    public async Task<string> GenerateAsync(
        IReportData data, 
        string outputPath, 
        ReportOptions options)
    {
        var report = new PdfDocument();
        
        await AddCoverPageAsync(report, data, options);
        await AddExecutiveSummaryAsync(report, data, options);
        await AddVmInventorySectionAsync(report, data, options);
        await AddStorageAnalysisSectionAsync(report, data, options);
        await AddHostInfrastructureSectionAsync(report, data, options);
        
        if (options.IncludeAiInsights)
        {
            await AddAiInsightsSectionAsync(report, data, options);
        }
        
        await AddAppendixAsync(report, data, options);
        
        await report.SaveAsync(outputPath);
        return outputPath;
    }
}
```

### PDF Content Structure

**Page 1: Cover Page**
- Report title with branding
- Source type (RVTools / LiveOptics)
- Export date
- Key metrics (VM count, hosts, total storage)
- Company logo

**Page 2: Executive Summary**
- Project overview
- Critical findings
- Resource totals
- Top recommendations

**Page 3-5: VM Inventory Section**
- Overview metrics
- Top 20 VMs by CPU (Bar Chart)
- Top 20 VMs by Memory (Bar Chart)
- VM distribution tables

**Page 6-8: Storage Analysis**
- Storage totals
- Top 15 largest partitions (Bar Chart)
- Capacity vs consumed (Grouped Bar)
- Free space distribution

**Page 9-11: Host Infrastructure**
- Host inventory
- CPU utilization (Bar Chart)
- Memory usage (Stacked Bar)
- Host specifications table

**Page 12-14: AI Insights (Optional)**
- Performance insights
- Optimization recommendations
- Risk assessments
- Cost optimization tips

**Page 15+: Appendix**
- Raw data tables
- Settings used
- Generation metadata

### PDF Design Specifications

**Layout:**
- Page size: A4 (210 × 297 mm)
- Margins: 1 inch all sides
- Font: Segoe UI / Helvetica
- Colors: Professional blue/gray theme
- Headers: Dark background with white text

**Charts in PDF:**
- Vector graphics for crisp scaling
- DPI: 300 for print quality
- Caption: Below each chart
- Page numbers: Bottom center
- Table of contents: Auto-generated

### Debug Features for PDF

**PDF Debug Mode:**
```csharp
public class PdfDebugReport
{
    public List<PdfDebugStep> Steps { get; set; }
    public long GenerationTimeMs { get; set; }
    public long FileSizeBytes { get; set; }
    public int PageCount { get; set; }
    public List<string> Warnings { get; set; }
}
```

**Debug Steps Tracked:**
- Document initialization
- Each section generation time
- Chart embedding duration
- File I/O operations
- Memory usage at each stage

---

## Phase 4: GitHub Copilot AI Integration (Week 6)

### Copilot API Integration

```csharp
public class CopilotAnalysisService : IAnalysisService
{
    private readonly IGitHubCopilotClient _copilotClient;
    
    public async Task<List<Insight>> AnalyzeReportAsync(IReportData data)
    {
        var prompt = BuildAnalysisPrompt(data);
        var response = await _copilotClient.GetCompletionAsync(prompt);
        return ParseInsights(response);
    }
    
    private string BuildAnalysisPrompt(IReportData data)
    {
        return $@"
You are a VMware infrastructure expert analyzing an {data.SourceType} report.

SUMMARY:
- {data.VirtualMachines.Count} VMs across {data.Hosts.Count} hosts
- Total Memory: {data.TotalMemoryTB:F2} TB
- Total Storage: {data.TotalStorageTB:F2} TB
- Total CPU Cores: {data.TotalCpuCores:N0}

PERFORMANCE INSIGHTS NEEDED:
1. Identify over-provisioned VMs
2. Highlight performance bottlenecks
3. Snapshot cleanup recommendations
4. Resource reclamation opportunities
5. Security/compliance issues

Provide 5-7 specific, actionable insights.
Format: [CATEGORY] Insight - Recommendation
";
    }
}
```

### Insight Categories

**Performance:**
- Over/under-provisioned VMs
- CPU/memory bottlenecks
- Disk latency issues
- Network congestion

**Optimization:**
- Snapshot cleanup opportunities
- Resource reclamation
- VM right-sizing
- Storage tier recommendations

**Risk:**
- Security/compliance gaps
- Outdated VMware Tools
- Unsupported OS versions
- Single points of failure

**Cost:**
- Right-sizing savings
- License optimization
- Storage cost reduction
- Resource consolidation

### AI-Powered Chart Recommendations

```csharp
public class AIChartRecommender
{
    public List<ChartRecommendation> RecommendCharts(IReportData data)
    {
        var recommendations = new List<ChartRecommendation>();
        
        if (data.VirtualMachines.Count > 50)
        {
            recommendations.Add(new ChartRecommendation
            {
                Type = ChartType.Bar,
                Title = "Top 20 VMs by CPU Count",
                Priority = Priority.High,
                Reasoning = "Large environment - identify outliers"
            });
        }
        
        if (data.Snapshots.Count > 100)
        {
            recommendations.Add(new ChartRecommendation
            {
                Type = ChartType.Bar,
                Title = "Snapshot Age Distribution",
                Priority = Priority.Critical,
                Reasoning = "High snapshot count - cleanup needed"
            });
        }
        
        return recommendations;
    }
}
```

---

## Phase 5: WPF UI Enhancement (Week 7)

### Tabbed Interface

```
┌─────────────────────────────────────────────────┐
│ 📁 Load Report │ 📊 Dashboard │ 📈 Charts │ 🤖 AI │
└─────────────────────────────────────────────────┘
```

**Dashboard Tab:**
- Drag-and-drop file upload
- KPI cards (VM count, hosts, storage)
- Quick stats overview
- Recent reports list
- Real-time parsing progress

**Charts Tab:**
- Gallery of all charts
- Filter by category
- Preview mode
- Export individual charts
- Interactive tooltips

**AI Tab:**
- Analysis results display
- Insight categories (Performance, Optimization, Risk, Cost)
- Regenerate analysis button
- Prompt history
- Confidence scores

**Debug Tab:**
- Real-time log streaming
- Operation profiling
- Memory usage graph
- Parser tracing
- Chart generation metrics

### Debug Console

```
┌─ Debug Console ─────────────────────────┐
│ [17:23:45] [INFO] Parser started       │
│ [17:23:48] [✓] vInfo: 805 VMs parsed   │
│ [17:23:50] [✓] vPartition: 1,618 rows  │
│ [17:23:51] [✓] vHost: 34 hosts         │
│ [17:23:52] [INFO] Chart generation: CPU│
│ [17:23:53] [✓] Chart complete (0.8s)   │
│ [17:23:55] [INFO] PDF export started   │
│ [17:24:03] [✓] PDF saved (8.2s)        │
│                                          │
│ [Export Log] [Clear] [Filter]          │
└──────────────────────────────────────────┘
```

---

## Phase 6: Comprehensive Testing (Ongoing)

### Test Coverage Goals

**Current: 55 tests (5% coverage)**
**Target: 200+ tests (90%+ coverage)**

### Test Categories

**Unit Tests (70%):**
- Parser validation
- Chart calculations
- Model validation
- AI prompt building
- Report formatting

**Integration Tests (20%):**
- End-to-end parse → generate workflow
- Multi-format export
- Error handling
- Memory management

**UI Tests (10%):**
- WPF data binding
- User workflows
- Accessibility
- Performance under load

### Performance Benchmarks

**Target Metrics:**
- Parse 1,000 VMs in < 3 seconds
- Generate PDF with 20 charts in < 15 seconds
- Memory usage: < 500MB for large reports
- No memory leaks (verified with dotMemory)
- UI responsive during background operations

---

## Phase 7: Documentation & Deployment (Week 8)

### Documentation

**User Guide:**
- Installation instructions
- Loading RVTools/LiveOptics files
- Understanding charts
- Generating PDF reports
- Using AI insights
- Troubleshooting

**Developer Guide:**
- Architecture overview
- Adding new parsers
- Creating custom charts
- AI integration patterns
- Unit testing guidelines

**API Documentation:**
- Interface specifications
- Extension points
- Configuration options
- Sample code

### Deployment Options

**1. Portable Executable:**
- Single .exe file
- Self-contained .NET runtime
- No installation required

**2. Windows Installer:**
- MSI installer
- Start menu shortcut
- File association (.xlsx, .rvtools)

**3. Microsoft Store:**
- Store package
- Auto-updates
- Enterprise deployment

---

## Detailed Implementation Checklist

### Week 1: Foundation
- [ ] Create project structure with 7 modules
- [ ] Set up DI container with Microsoft.Extensions.DependencyInjection
- [ ] Implement debug logging with Serilog
- [ ] Create base interfaces for all components
- [ ] Set up unit test projects with xUnit
- [ ] Configure build pipeline
- [ ] Create debug dashboard UI

### Week 2: RVTools Parser
- [ ] Implement IDataParser interface
- [ ] Parse vInfo sheet (VM inventory)
- [ ] Parse vPartition sheet (disk usage)
- [ ] Parse vHost sheet (ESXi hosts)
- [ ] Cross-reference VMs to hosts
- [ ] Calculate aggregate metrics
- [ ] Add parser validation tests
- [ ] Create debug parser tracer

### Week 3: LiveOptics Parser & Data Models
- [ ] Refactor LiveOptics parser into separate project
- [ ] Implement unified data model
- [ ] Complete performance data parsing
- [ ] Add data validation layer
- [ ] Create sample data generator for testing
- [ ] Parser performance optimization
- [ ] Add comprehensive parser tests

### Week 4: Visualization Engine
- [ ] Implement IChartProvider interface
- [ ] Create BarChartProvider
- [ ] Create LineChartProvider
- [ ] Create chart data transformation layer
- [ ] Implement chart export (SVG, PNG)
- [ ] Add chart preview in UI
- [ ] Performance testing with large datasets
- [ ] Chart debugging tools

### Week 5: PDF Generator (PRIORITY)
- [ ] Implement IReportGenerator for PDF
- [ ] Create cover page template
- [ ] Build executive summary section
- [ ] Add VM inventory section with bar charts
- [ ] Add storage analysis section
- [ ] Add host infrastructure section
- [ ] Implement table of contents
- [ ] Add headers/footers with page numbers
- [ ] Test PDF with real data
- [ ] Optimize PDF generation performance
- [ ] Add PDF debug reporting

### Week 6: GitHub Copilot AI
- [ ] Set up GitHub Copilot API access
- [ ] Implement IAnalysisService
- [ ] Build prompt generation engine
- [ ] Add AI insights to PDF
- [ ] Create AI settings UI
- [ ] Implement prompt history
- [ ] Add AI confidence scoring
- [ ] AI debug panel in UI
- [ ] Rate limiting and error handling

### Week 7: UI Enhancement
- [ ] Implement tabbed interface
- [ ] Create dashboard with KPI cards
- [ ] Add chart gallery view
- [ ] Create interactive chart tooltips
- [ ] Implement debug console
- [ ] Add drag-and-drop file loading
- [ ] Create settings/preferences dialog
- [ ] Add progress indicators
- [ ] Implement cancellation tokens
- [ ] Accessibility improvements

### Week 8: Testing & Documentation
- [ ] Write unit tests for all parsers
- [ ] Write unit tests for all chart providers
- [ ] Write integration tests for end-to-end workflows
- [ ] Performance benchmarking
- [ ] Memory leak testing
- [ ] Create user guide
- [ ] Create developer guide
- [ ] Create API documentation
- [ ] Build installer (Inno Setup)
- [ ] Create portable build script
- [ ] Final validation

---

## Debug Features Implementation

### 1. Parser Debugging

```csharp
public class ParserDebugger
{
    public ParseDebugResult DebugParse(string filePath)
    {
        return new ParseDebugResult
        {
            Steps = new[]
            {
                new DebugStep { 
                    Name = "File Validation", 
                    Duration = "12ms", 
                    Status = DebugStatus.Success 
                },
                new DebugStep { 
                    Name = "vInfo Parse", 
                    Duration = "3.2s", 
                    Status = DebugStatus.Success, 
                    RowsParsed = 805 
                },
                new DebugStep { 
                    Name = "Data Validation", 
                    Duration = "45ms", 
                    Status = DebugStatus.Warning,
                    Warnings = new[] { "12 VMs missing OS data" }
                }
            },
            TotalDuration = "6.2s",
            MemoryUsedMB = 128,
            FileSizeMB = 291
        };
    }
}
```

### 2. Chart Debugging

```csharp
public class ChartDebugInspector
{
    public ChartDebugReport InspectChart(IChart chart)
    {
        return new ChartDebugReport
        {
            ChartId = chart.Id,
            Title = chart.Title,
            DataPoints = chart.Series.Sum(s => s.Values.Count),
            GenerationTimeMs = chart.DebugInfo.GenerationTime,
            RenderTimeMs = chart.DebugInfo.RenderTime,
            MemoryUsageMB = chart.DebugInfo.MemoryUsed,
            Warnings = ValidateChartData(chart)
        };
    }
    
    private List<string> ValidateChartData(IChart chart)
    {
        var warnings = new List<string>();
        
        if (chart.Series.Any(s => s.Values.Count == 0))
            warnings.Add("Empty series detected");
            
        if (chart.Series.Sum(s => s.Values.Count) > 1000)
            warnings.Add("Large dataset may impact PDF generation");
            
        return warnings;
    }
}
```

### 3. PDF Generation Debugging

```csharp
public class PdfDebugTracker
{
    private readonly List<PdfDebugStep> _steps = new();
    
    public void TrackStep(string name, Action action)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            action();
            sw.Stop();
            _steps.Add(new PdfDebugStep { 
                Name = name, 
                DurationMs = sw.ElapsedMilliseconds, 
                Status = PdfDebugStatus.Success 
            });
        }
        catch (Exception ex)
        {
            sw.Stop();
            _steps.Add(new PdfDebugStep { 
                Name = name, 
                DurationMs = sw.ElapsedMilliseconds, 
                Status = PdfDebugStatus.Error,
                Error = ex.Message
            });
            throw;
        }
    }
    
    public PdfDebugReport GetReport()
    {
        return new PdfDebugReport
        {
            Steps = _steps,
            TotalDurationMs = _steps.Sum(s => s.DurationMs),
            PageCount = _steps.Count(s => s.Name.Contains("Page")),
            MemoryPeakMB = GetPeakMemoryUsage()
        };
    }
}
```

---

## Key Technical Decisions

### 1. PDF Library Selection

**QuestPDF**
- ✅ Modern fluent API
- ✅ Commercial-friendly license
- ✅ Excellent chart/image support
- ✅ Good performance
- ✅ Active development

**PdfSharp + MigraDoc**
- ✅ Free and open source
- ✅ Mature and stable
- ❌ Older API
- ❌ Limited modern features

**Decision:** Use QuestPDF for primary PDF generation

### 2. Chart Library

**Recommendation:** Continue with LiveCharts2
- ✅ Already integrated
- ✅ Good performance
- ✅ Supports export to SVG/PNG
- ✅ Modern WPF controls
- ❌ Limited bar chart customization (workaround with custom renderers)

### 3. AI Integration

**GitHub Copilot API**
- ✅ Already have access
- ✅ Good for code analysis
- ✅ Cost-effective
- ❌ Limited non-code capabilities

**Fallback:** Direct OpenAI API
- ✅ Full GPT-4 access
- ✅ More flexible
- ❌ Additional cost

**Decision:** Start with Copilot, add OpenAI as option

### 4. Parser Strategy

**RVTools**
- Excel format: Direct parsing with ExcelDataReader
- CSV format: Directory scanning + pattern matching
- Structure: Multiple sheets, relational data

**LiveOptics**
- Excel format: Consolidated sheets
- Structure: Single file, embedded performance data

**Decision:** Separate parsers, unified output model

---

## Files to Create/Modify

### New Files (Core)
- `src/OpenReportViewer.Core/Interfaces/IDataParser.cs`
- `src/OpenReportViewer.Core/Interfaces/IReportGenerator.cs`
- `src/OpenReportViewer.Core/Interfaces/IChartProvider.cs`
- `src/OpenReportViewer.Core/Interfaces/IAnalysisService.cs`
- `src/OpenReportViewer.Core/Interfaces/IDebugService.cs`
- `src/OpenReportViewer.Core/Models/IReportData.cs`
- `src/OpenReportViewer.Core/Models/IVirtualMachine.cs`
- `src/OpenReportViewer.Core/Models/IHost.cs`
- `src/OpenReportViewer.Core/Models/IChart.cs`
- `src/OpenReportViewer.Core/Models/ChartData.cs`

### New Files (Parsers)
- `src/OpenReportViewer.Parsers/RVTools/RVToolsParser.cs`
- `src/OpenReportViewer.Parsers/RVTools/Models/VInfoRow.cs`
- `src/OpenReportViewer.Parsers/RVTools/Models/VPartitionRow.cs`
- `src/OpenReportViewer.Parsers/RVTools/Models/VHostRow.cs`
- `src/OpenReportViewer.Parsers/LiveOptics/LiveOpticsParser.cs`

### New Files (Visualizations)
- `src/OpenReportViewer.Visualizations/Providers/BarChartProvider.cs`
- `src/OpenReportViewer.Visualizations/Providers/LineChartProvider.cs`
- `src/OpenReportViewer.Visualizations/ChartFactory.cs`
- `src/OpenReportViewer.Visualizations/ChartExportService.cs`

### New Files (Generators)
- `src/OpenReportViewer.Generators/Pdf/PdfReportGenerator.cs`
- `src/OpenReportViewer.Generators/Pdf/Templates/CoverPage.cs`
- `src/OpenReportViewer.Generators/Pdf/Templates/ExecutiveSummary.cs`
- `src/OpenReportViewer.Generators/Pdf/Templates/VmInventorySection.cs`
- `src/OpenReportViewer.Generators/Pdf/Templates/StorageAnalysisSection.cs`
- `src/OpenReportViewer.Generators/Pdf/Templates/HostInfrastructureSection.cs`
- `src/OpenReportViewer.Generators/Pdf/Templates/Appendix.cs`

### New Files (AI)
- `src/OpenReportViewer.AI.Copilot/CopilotAnalysisService.cs`
- `src/OpenReportViewer.AI.Copilot/GitHubCopilotClient.cs`
- `src/OpenReportViewer.AI.Copilot/Models/Insight.cs`
- `src/OpenReportViewer.AI.Copilot/Models/InsightCategory.cs`
- `src/OpenReportViewer.AI.Copilot/PromptBuilder.cs`

### New Files (Debugger)
- `debug/OpenReportViewer.Debugger/Views/DebugConsole.xaml`
- `debug/OpenReportViewer.Debugger/Services/DebugService.cs`
- `debug/OpenReportViewer.Debugger/Models/DebugStep.cs`
- `debug/OpenReportViewer.Debugger/Models/DebugReport.cs`

### Modified Files
- `src/OpenReportViewer.UI.Wpf/ViewModels/MainViewModel.cs` - Refactor for DI
- `src/OpenReportViewer.UI.Wpf/Views/MainWindow.xaml` - Add tabs
- `src/OpenReportViewer.Tests/*` - Expand to 200+ tests

---

## Success Metrics

### Functional
- ✅ Parse RVTools files with 1000+ VMs in < 3 seconds
- ✅ Generate 20-page PDF report in < 15 seconds
- ✅ 95%+ accuracy in data extraction
- ✅ AI generates 5-7 actionable insights per report

### Technical
- ✅ 90%+ unit test coverage
- ✅ Zero memory leaks (verified with profiling)
- ✅ All 3 parsers implemented (RVTools, LiveOptics, CSV)
- ✅ 10+ chart types available

### UX
- ✅ 4.5+ star user rating
- ✅ Load file → PDF in < 5 clicks
- ✅ Debug console shows real-time progress
- ✅ Responsive UI during long operations

---

## Rollback Plan

**If PDF generation fails in testing:**
1. Fallback to PowerPoint generation (existing working code)
2. Research alternative PDF libraries (iTextSharp, Syncfusion)
3. Delay PDF feature to Phase 2

**If Copilot API insufficient:**
1. Integrate OpenAI API directly
2. Add configuration for API selection
3. Maintain mock AI for offline use

**If performance targets not met:**
1. Implement parallel parsing
2. Add streaming PDF generation
3. Use background workers for UI responsiveness

---

## Next Steps

1. **Week 1 Day 1:**
   - Set up new project structure
   - Create OpenReportViewer.sln
   - Add all 7 module projects
   - Configure DI container

2. **Week 1 Day 2-3:**
   - Create debug infrastructure
   - Set up logging with Serilog
   - Create debug console UI
   - Implement base interfaces

3. **Week 1 Day 4-5:**
   - Implement RVTools parser (vInfo only)
   - Add validation tests
   - Create debug parser tracer

4. **Week 1 Review:** Code review with team

Ready to start Week 1?
</content>
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
namespace OpenReportViewer.AI
{
    /// <summary>
    /// Non-demo analysis providers. <see cref="HeuristicAnalysisService"/> is production-path
    /// (deterministic rules over assessment data, no LLM). <see cref="OpenAiCompatibleAnalysisService"/>
    /// is the external HTTP seam (used when BaseUrl/ApiKey are configured).
    /// </summary>
    public interface IExternalAnalysisProvider
    {
        string ProviderName { get; }
        bool IsDemoProvider { get; }
        Task<IReadOnlyList<string>> AnalyzeAsync(AnalysisRequest request, CancellationToken ct = default);
    }

    public sealed record AnalysisRequest(
        string SourceType,
        int VmCount,
        int HostCount,
        int TotalCpuCores,
        double TotalMemoryGb,
        double TotalProvisionedGb,
        IReadOnlyList<(string Name, int Cpu, double MemoryMb, string PowerState)> TopVms,
        IReadOnlyList<(string Host, int Cores, int VmCount, string Esx)> Hosts);

    /// <summary>
    /// Offline, data-driven insights. Not labeled as an LLM demo.
    /// </summary>
    public sealed class HeuristicAnalysisService : IExternalAnalysisProvider
    {
        public string ProviderName => "Heuristic";
        public bool IsDemoProvider => false;

        public Task<IReadOnlyList<string>> AnalyzeAsync(AnalysisRequest request, CancellationToken ct = default)
        {
            var insights = new List<string>();
            if (request == null)
            {
                insights.Add("[Heuristic] No assessment data provided.");
                return Task.FromResult<IReadOnlyList<string>>(insights);
            }

            insights.Add($"[Heuristic] Source={request.SourceType}, VMs={request.VmCount}, Hosts={request.HostCount}, cores={request.TotalCpuCores}, memory={request.TotalMemoryGb:0.#} GB.");

            if (request.VmCount > 0 && request.HostCount > 0)
            {
                var vmPerHost = (double)request.VmCount / request.HostCount;
                if (vmPerHost > 25)
                    insights.Add($"[Heuristic] Density: {vmPerHost:0.0} VMs/host — review cluster balance and HA admission control.");
                else if (vmPerHost < 5)
                    insights.Add($"[Heuristic] Low density ({vmPerHost:0.0} VMs/host) — possible consolidation opportunity.");
            }

            var big = request.TopVms.Where(v => v.Cpu >= 8 || v.MemoryMb >= 65536).Take(5).ToList();
            if (big.Count > 0)
            {
                insights.Add("[Heuristic] Right-size candidates (high vCPU or ≥64 GB): " +
                    string.Join(", ", big.Select(v => $"{v.Name} ({v.Cpu} vCPU, {v.MemoryMb / 1024.0:0.#} GB)")));
            }

            var poweredOff = request.TopVms.Count(v => !string.IsNullOrEmpty(v.PowerState) &&
                v.PowerState.Contains("off", StringComparison.OrdinalIgnoreCase));
            if (poweredOff > 0)
                insights.Add($"[Heuristic] {poweredOff} VM(s) in the top set appear powered off — reclaim resources if unused.");

            var denseHosts = request.Hosts.Where(h => h.VmCount >= 20 || h.Cores >= 48).Take(5).ToList();
            foreach (var h in denseHosts)
                insights.Add($"[Heuristic] Host {h.Host}: {h.VmCount} VMs on {h.Cores} cores ({h.Esx}) — check contention and maintenance windows.");

            if (request.TotalProvisionedGb > 0 && request.TotalMemoryGb > 0 && request.TotalProvisionedGb > request.TotalMemoryGb * 50)
                insights.Add("[Heuristic] Provisioned storage greatly exceeds memory footprint — validate thin/thick policies and orphaned VMDKs.");

            if (insights.Count == 1)
                insights.Add("[Heuristic] No high-severity thresholds crossed in the provided aggregates.");

            return Task.FromResult<IReadOnlyList<string>>(insights);
        }
    }

    /// <summary>
    /// External LLM seam. Without BaseUrl/ApiKey, falls back to <see cref="HeuristicAnalysisService"/>.
    /// When configured, posts a chat completion request to an OpenAI-compatible endpoint.
    /// </summary>
    public sealed class OpenAiCompatibleAnalysisService : IExternalAnalysisProvider, OpenReportViewer.Core.Interfaces.IAnalysisService
    {
        private readonly HeuristicAnalysisService _fallback = new();
        private readonly string? _baseUrl;
        private readonly string? _apiKey;
        private readonly string _model;

        public OpenAiCompatibleAnalysisService(string? baseUrl, string? apiKey, string model = "gpt-4o-mini")
        {
            _baseUrl = baseUrl;
            _apiKey = apiKey;
            _model = model;
        }

        public string ProviderName => IsConfigured ? $"OpenAI-compatible ({_model})" : "Heuristic";
        public bool IsDemoProvider => false;
        public bool IsConfigured => !string.IsNullOrWhiteSpace(_baseUrl) && !string.IsNullOrWhiteSpace(_apiKey);

        public async Task<IReadOnlyList<string>> AnalyzeAsync(AnalysisRequest request, CancellationToken ct = default)
        {
            if (!IsConfigured)
                return await _fallback.AnalyzeAsync(request, ct);

            try
            {
                using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
                http.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);

                var prompt = BuildPrompt(request);
                var payload = new
                {
                    model = _model,
                    messages = new object[]
                    {
                        new { role = "system", content = "You are a VMware infrastructure expert. Return 5-7 short actionable insights, one per line, prefixed with - ." },
                        new { role = "user", content = prompt }
                    },
                    temperature = 0.2
                };
                var json = System.Text.Json.JsonSerializer.Serialize(payload);
                var resp = await http.PostAsync(
                    _baseUrl!.TrimEnd('/') + "/v1/chat/completions",
                    new StringContent(json, Encoding.UTF8, "application/json"),
                    ct);
                if (!resp.IsSuccessStatusCode)
                    return await _fallback.AnalyzeAsync(request, ct);

                var body = await resp.Content.ReadAsStringAsync(ct);
                using var doc = System.Text.Json.JsonDocument.Parse(body);
                var content = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? "";
                var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Where(l => l.Length > 0)
                    .Select(l => l.TrimStart('-', '*', ' '))
                    .Select(l => $"[LLM] {l}")
                    .ToList();
                return lines.Count > 0 ? lines : await _fallback.AnalyzeAsync(request, ct);
            }
            catch
            {
                return await _fallback.AnalyzeAsync(request, ct);
            }
        }

        public async Task<string> AnalyzePerformanceAsync(string query)
        {
            var req = new AnalysisRequest("unknown", 0, 0, 0, 0, 0,
                Array.Empty<(string, int, double, string)>(),
                Array.Empty<(string, int, int, string)>());
            var lines = await AnalyzeAsync(req);
            return $"{ProviderName}: {query}\n" + string.Join("\n", lines);
        }

        public async Task<string> ResearchHardwareAsync(string hardwareModel)
        {
            return await Task.FromResult(
                $"[{ProviderName}] {hardwareModel}: validate vendor lifecycle/EOSL and benchmark against current workload peaks before refresh.");
        }

        private static string BuildPrompt(AnalysisRequest r)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Source={r.SourceType} VMs={r.VmCount} Hosts={r.HostCount} Cores={r.TotalCpuCores} MemoryGB={r.TotalMemoryGb:0.#} ProvGB={r.TotalProvisionedGb:0.#}");
            foreach (var v in r.TopVms.Take(15))
                sb.AppendLine($"VM {v.Name}: {v.Cpu} vCPU, {v.MemoryMb:0} MB, {v.PowerState}");
            foreach (var h in r.Hosts.Take(15))
                sb.AppendLine($"Host {h.Host}: {h.Cores} cores, {h.VmCount} VMs, {h.Esx}");
            sb.AppendLine("Identify over-provisioning, density risks, cleanup, and refresh candidates.");
            return sb.ToString();
        }
    }

    /// <summary>
    /// Adapter from ProjectInfo aggregates into <see cref="AnalysisRequest"/>.
    /// </summary>
    public static class AnalysisRequestFactory
    {
        public static AnalysisRequest FromProject(OpenReportViewer.Core.Models.ProjectInfo p)
        {
            p ??= new OpenReportViewer.Core.Models.ProjectInfo();
            var topVms = (p.VirtualMachines ?? new List<OpenReportViewer.Core.Models.VirtualMachine>())
                .OrderByDescending(v => v.CpuCount)
                .ThenByDescending(v => v.MemoryMB)
                .Take(30)
                .Select(v => (v.Name, v.CpuCount, v.MemoryMB, v.PowerState))
                .ToList();
            var hosts = (p.Hosts ?? new List<OpenReportViewer.Core.Models.HostNode>())
                .OrderByDescending(h => h.VmCount)
                .Take(20)
                .Select(h => (h.HostName, h.TotalCores, h.VmCount, h.EsxVersion))
                .ToList();
            return new AnalysisRequest(
                p.SourceType,
                OpenReportViewer.Core.Models.ProjectAggregates.TotalVmCount(p),
                OpenReportViewer.Core.Models.ProjectAggregates.TotalHostCount(p),
                OpenReportViewer.Core.Models.ProjectAggregates.TotalCpuCores(p),
                OpenReportViewer.Core.Models.ProjectAggregates.TotalMemoryMb(p) / 1024.0,
                OpenReportViewer.Core.Models.ProjectAggregates.TotalProvisionedMb(p) / 1024.0,
                topVms,
                hosts);
        }
    }
}

namespace OpenReportViewer.AI
{
    using Microsoft.Extensions.DependencyInjection;
    using OpenReportViewer.Core.Interfaces;

    public static class ExternalAiServiceCollectionExtensions
    {
        public static IServiceCollection AddOpenReportViewerHeuristicAi(this IServiceCollection services)
        {
            services.AddSingleton<HeuristicAnalysisService>();
            services.AddSingleton<IExternalAnalysisProvider>(sp => sp.GetRequiredService<HeuristicAnalysisService>());
            // Prefer heuristic as IAnalysisService when explicitly registered after mock
            services.AddSingleton<IAnalysisService>(sp =>
            {
                var envUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL");
                var envKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
                var envModel = Environment.GetEnvironmentVariable("OPENAI_MODEL") ?? "gpt-4o-mini";
                if (!string.IsNullOrWhiteSpace(envUrl) && !string.IsNullOrWhiteSpace(envKey))
                    return new OpenAiCompatibleAnalysisService(envUrl, envKey, envModel);
                return new OpenAiCompatibleAnalysisService(null, null, envModel);
            });
            return services;
        }
    }
}

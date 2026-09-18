using System.Net;
using System.Text;
using OpenReportViewer.Core.Models;

namespace OpenReportViewer.Reporting
{
    public class HtmlReportGenerator
    {
        public string Format => "html";

        public void GenerateToFile(ProjectInfo project, string outputPath)
        {
            ArgumentNullException.ThrowIfNull(project);
            if (string.IsNullOrWhiteSpace(outputPath))
                throw new ArgumentException("Output path cannot be null or empty", nameof(outputPath));
            File.WriteAllText(outputPath, GenerateHtml(project), Encoding.UTF8);
        }

        public string GenerateHtml(ProjectInfo project)
        {
            ArgumentNullException.ThrowIfNull(project);
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"en\"><head><meta charset=\"utf-8\"/>");
            sb.AppendLine("<title>OpenReportViewer — " + WebUtility.HtmlEncode(project.ProjectName) + "</title>");
            sb.AppendLine("<style>");
            sb.AppendLine("body{font-family:Segoe UI,Arial,sans-serif;margin:2rem;color:#222}");
            sb.AppendLine("h1,h2{color:#0b3d91} table{border-collapse:collapse;width:100%;margin:1rem 0}");
            sb.AppendLine("th,td{border:1px solid #ccc;padding:6px 8px;text-align:left}");
            sb.AppendLine("th{background:#eef2f7} .kpi{display:inline-block;background:#f5f7fa;border-radius:8px;padding:12px 16px;margin:4px 8px 4px 0}");
            sb.AppendLine(".note{color:#666;font-size:0.9rem} code{background:#f0f0f0;padding:1px 4px}");
            sb.AppendLine("</style></head><body>");

            sb.AppendLine("<h1>Infrastructure Assessment Report</h1>");
            sb.AppendLine($"<p><strong>Project:</strong> {WebUtility.HtmlEncode(project.ProjectName)}<br/>");
            sb.AppendLine($"<strong>Source:</strong> {WebUtility.HtmlEncode(project.SourceType)}<br/>");
            sb.AppendLine($"<strong>Generated:</strong> {DateTime.Now:yyyy-MM-dd HH:mm}</p>");

            var cpu = ProjectAggregates.TotalCpuCores(project);
            var memGb = ProjectAggregates.TotalMemoryMb(project) / 1024.0;
            sb.AppendLine("<div class=\"kpi\"><div>VMs</div><strong>" + ProjectAggregates.TotalVmCount(project) + "</strong></div>");
            sb.AppendLine("<div class=\"kpi\"><div>Hosts</div><strong>" + ProjectAggregates.TotalHostCount(project) + "</strong></div>");
            sb.AppendLine("<div class=\"kpi\"><div>CPU cores</div><strong>" + cpu + "</strong></div>");
            sb.AppendLine("<div class=\"kpi\"><div>Memory GB</div><strong>" + memGb.ToString("0.#") + "</strong></div>");

            if (project.VirtualMachines.Count > 0)
            {
                sb.AppendLine("<h2>Top VMs by CPU</h2>");
                sb.AppendLine("<table><tr><th>VM</th><th>vCPU</th><th>Memory MB</th><th>Host</th><th>Power</th></tr>");
                foreach (var vm in project.VirtualMachines.OrderByDescending(v => v.CpuCount).Take(25))
                {
                    sb.AppendLine($"<tr><td>{WebUtility.HtmlEncode(vm.Name)}</td><td>{vm.CpuCount}</td><td>{vm.MemoryMB:0}</td><td>{WebUtility.HtmlEncode(vm.HostName)}</td><td>{WebUtility.HtmlEncode(vm.PowerState)}</td></tr>");
                }
                sb.AppendLine("</table>");
            }

            if (project.Hosts.Count > 0)
            {
                sb.AppendLine("<h2>ESXi Hosts</h2>");
                sb.AppendLine("<table><tr><th>Host</th><th>Cores</th><th>Memory MB</th><th>VMs</th><th>ESX</th><th>Model</th></tr>");
                foreach (var h in project.Hosts.Take(50))
                {
                    sb.AppendLine($"<tr><td>{WebUtility.HtmlEncode(h.HostName)}</td><td>{h.TotalCores}</td><td>{h.MemoryMB:0}</td><td>{h.VmCount}</td><td>{WebUtility.HtmlEncode(h.EsxVersion)}</td><td>{WebUtility.HtmlEncode(h.Model)}</td></tr>");
                }
                sb.AppendLine("</table>");
            }

            if (project.Servers.Count > 0 && project.VirtualMachines.Count == 0)
            {
                sb.AppendLine("<h2>Server Inventory</h2>");
                sb.AppendLine("<table><tr><th>Server</th><th>OS</th><th>CPU</th><th>Memory GB</th></tr>");
                foreach (var s in project.Servers.Take(50))
                {
                    sb.AppendLine($"<tr><td>{WebUtility.HtmlEncode(s.ServerName)}</td><td>{WebUtility.HtmlEncode(s.OS)}</td><td>{s.CPUCount}</td><td>{s.MemoryGB:0.#}</td></tr>");
                }
                sb.AppendLine("</table>");
            }

            if (project.Partitions.Count > 0)
            {
                sb.AppendLine("<h2>Top Partitions by Capacity</h2>");
                sb.AppendLine("<table><tr><th>VM</th><th>Disk</th><th>Capacity MB</th><th>Free MB</th><th>Free %</th></tr>");
                foreach (var part in project.Partitions.OrderByDescending(p => p.CapacityMB).Take(25))
                {
                    sb.AppendLine($"<tr><td>{WebUtility.HtmlEncode(part.VmName)}</td><td>{WebUtility.HtmlEncode(part.Disk)}</td><td>{part.CapacityMB:0}</td><td>{part.FreeMB:0}</td><td>{part.FreePercent:0.#}</td></tr>");
                }
                sb.AppendLine("</table>");
            }

            sb.AppendLine("<p class=\"note\">Generated by OpenReportViewer HTML report generator. Demo AI insights and live performance series are out of scope for this format.</p>");
            sb.AppendLine("</body></html>");
            return sb.ToString();
        }
    }

    public class ReportFormatFactory
    {
        private readonly IPptxReportGenerator _pptx;
        private readonly IPdfReportGenerator _pdf;
        private readonly HtmlReportGenerator _html;

        public ReportFormatFactory(IPptxReportGenerator pptx, IPdfReportGenerator pdf, HtmlReportGenerator html)
        {
            _pptx = pptx;
            _pdf = pdf;
            _html = html;
        }

        public void Generate(ProjectInfo project, string outputPath)
        {
            var ext = Path.GetExtension(outputPath)?.ToLowerInvariant() ?? "";
            switch (ext)
            {
                case ".pdf":
                    _pdf.GenerateToFile(project, outputPath);
                    break;
                case ".html":
                case ".htm":
                    _html.GenerateToFile(project, outputPath);
                    break;
                case ".pptx":
                    _pptx.GeneratePresentation(project, outputPath);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported report extension: {ext}");
            }
        }
    }
}

namespace OpenReportViewer.Reporting
{
    using Microsoft.Extensions.DependencyInjection;

    public static class HtmlServiceCollectionExtensions
    {
        public static IServiceCollection AddOpenReportViewerHtml(this IServiceCollection services)
        {
            services.AddSingleton<HtmlReportGenerator>();
            services.AddSingleton<ReportFormatFactory>();
            return services;
        }
    }
}

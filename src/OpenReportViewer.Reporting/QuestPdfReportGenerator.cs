using System.IO;
using OpenReportViewer.Core.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace OpenReportViewer.Reporting
{
    public interface IPdfReportGenerator
    {
        Stream Generate(ProjectInfo project, PdfGenerationOptions? options = null);
        void GenerateToFile(ProjectInfo project, string outputPath, PdfGenerationOptions? options = null);
    }

    public sealed record PdfGenerationOptions
    {
        public bool IncludeCoverPage { get; init; } = true;
        public bool IncludeExecutiveSummary { get; init; } = true;
        public bool IncludeServerInventory { get; init; } = true;
        public string FontFamily { get; init; } = "Segoe UI";
    }

    public class QuestPdfReportGenerator : IPdfReportGenerator
    {
        static QuestPdfReportGenerator()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public Stream Generate(ProjectInfo project, PdfGenerationOptions? options = null)
        {
            ArgumentNullException.ThrowIfNull(project);
            options ??= new PdfGenerationOptions();

            var stream = new MemoryStream();
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontFamily(options.FontFamily).FontSize(11));
                    page.Header().Element(ComposeHeader);
                    page.Content().Column(column =>
                    {
                        if (options.IncludeCoverPage)
                        {
                            column.Item().Element(c => ComposeCover(c, project));
                            column.Item().PageBreak();
                        }
                        if (options.IncludeExecutiveSummary)
                        {
                            column.Item().Element(c => ComposeSummary(c, project));
                            column.Item().PageBreak();
                        }
                        if (options.IncludeServerInventory)
                        {
                            column.Item().Element(c => ComposeServers(c, project));
                        }
                        if (project.VirtualMachines.Count > 0)
                        {
                            column.Item().PageBreak();
                            column.Item().Element(c => ComposeTopVms(c, project));
                        }
                    });
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Page ");
                        text.CurrentPageNumber();
                        text.Span(" / ");
                        text.TotalPages();
                    });
                });
            }).GeneratePdf(stream);

            stream.Position = 0;
            return stream;
        }

        public void GenerateToFile(ProjectInfo project, string outputPath, PdfGenerationOptions? options = null)
        {
            if (string.IsNullOrWhiteSpace(outputPath))
                throw new ArgumentException("Output path cannot be null or empty", nameof(outputPath));

            using var stream = Generate(project, options);
            using var file = File.Create(outputPath);
            stream.CopyTo(file);
        }

        private static void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Text("OpenReportViewer").Bold().FontSize(14);
                row.ConstantItem(120).AlignRight().Text(DateTime.Now.ToString("MMM dd, yyyy"));
            });
        }

        private static void ComposeCover(IContainer container, ProjectInfo project)
        {
            var cpu = ProjectAggregates.TotalCpuCores(project);
            var memMb = ProjectAggregates.TotalMemoryMb(project);
            container.PaddingTop(6, Unit.Centimetre).AlignCenter().Column(column =>
            {
                column.Item().Text("Infrastructure Assessment Report").FontSize(24).Bold().FontColor(Colors.Blue.Darken2);
                column.Item().PaddingTop(1, Unit.Centimetre).Text($"Project: {project.ProjectName}").FontSize(16);
                column.Item().PaddingTop(0.25f, Unit.Centimetre).Text($"Source: {project.SourceType}");
                column.Item().PaddingTop(0.25f, Unit.Centimetre).Text($"Generated: {DateTime.Now:MMMM dd, yyyy}");
                column.Item().PaddingTop(0.5f, Unit.Centimetre).Text($"VMs: {ProjectAggregates.TotalVmCount(project)}");
                column.Item().Text($"Hosts/Servers: {ProjectAggregates.TotalHostCount(project) + (project.VirtualMachines.Count == 0 ? (project.Servers?.Count ?? 0) : 0)}");
                column.Item().Text($"Total CPU cores: {cpu:N0}");
                column.Item().Text($"Total memory: {memMb / 1024.0:N1} GB");
            });
        }

        private static void ComposeSummary(IContainer container, ProjectInfo project)
        {
            var cpu = ProjectAggregates.TotalCpuCores(project);
            var memGb = ProjectAggregates.TotalMemoryMb(project) / 1024.0;
            container.Column(column =>
            {
                column.Item().Text("Executive Summary").FontSize(16).Bold();
                column.Item().PaddingTop(10).Text(
                    $"This report summarizes {project.SourceType} assessment data for project '{project.ProjectName}'.");
                column.Item().PaddingTop(8).Text(
                    $"Analyzed {ProjectAggregates.TotalVmCount(project)} VMs and {ProjectAggregates.TotalHostCount(project)} hosts ({project.Servers?.Count ?? 0} inventory rows).");
                column.Item().Text($"Total CPU cores: {cpu:N0}");
                column.Item().Text($"Total memory: {memGb:N1} GB");
                column.Item().Text($"Provisioned storage: {ProjectAggregates.TotalProvisionedMb(project) / 1024.0:N1} GB");
                column.Item().Text($"In-use storage: {ProjectAggregates.TotalInUseMb(project) / 1024.0:N1} GB");
                column.Item().PaddingTop(8).Text(
                    "Note: Live performance time-series and live AI insights are not included in this MVP PDF.")
                    .FontSize(10).FontColor(Colors.Grey.Darken1);
            });
        }

        private static void ComposeTopVms(IContainer container, ProjectInfo project)
        {
            var top = ProjectAggregates.TopVmCpu(project, 15);
            container.Column(column =>
            {
                column.Item().Text("Top VMs by CPU").FontSize(16).Bold();
                column.Item().PaddingTop(10).Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        c.RelativeColumn(4);
                        c.RelativeColumn(2);
                    });
                    table.Header(h =>
                    {
                        h.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("VM").Bold();
                        h.Cell().Background(Colors.Grey.Lighten3).Padding(4).AlignRight().Text("vCPU").Bold();
                    });
                    if (top.Count == 0)
                    {
                        table.Cell().ColumnSpan(2).Padding(8).Text("No VM data.");
                        return;
                    }
                    foreach (var (label, value) in top)
                    {
                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(label);
                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).AlignRight().Text(value.ToString("0"));
                    }
                });
            });
        }

        private static void ComposeServers(IContainer container, ProjectInfo project)
        {
            container.Column(column =>
            {
                column.Item().Text("Server Inventory").FontSize(16).Bold();
                column.Item().PaddingTop(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                    });
                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Server").Bold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("OS").Bold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(4).AlignRight().Text("CPU").Bold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(4).AlignRight().Text("Memory GB").Bold();
                    });

                    if (project.Servers == null || project.Servers.Count == 0)
                    {
                        table.Cell().ColumnSpan(4).Padding(8).Text("No servers in dataset.");
                        return;
                    }

                    foreach (var s in project.Servers.Take(200))
                    {
                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(s.ServerName);
                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(s.OS);
                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).AlignRight().Text(s.CPUCount.ToString());
                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).AlignRight().Text(s.MemoryGB.ToString("0.#"));
                    }
                });
            });
        }
    }
}

namespace OpenReportViewer.Reporting
{
    using Microsoft.Extensions.DependencyInjection;

    public static class PdfServiceCollectionExtensions
    {
        public static IServiceCollection AddOpenReportViewerPdf(this IServiceCollection services)
        {
            services.AddSingleton<IPdfReportGenerator, QuestPdfReportGenerator>();
            return services;
        }
    }
}

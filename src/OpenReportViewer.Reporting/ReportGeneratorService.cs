using System;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using OpenReportViewer.Core.Models;
using OpenReportViewer.Core.Interfaces;
using A = DocumentFormat.OpenXml.Drawing;
using P = DocumentFormat.OpenXml.Presentation;

namespace OpenReportViewer.Reporting
{
    public interface IPptxReportGenerator
    {
        void GeneratePresentation(ProjectInfo project, string outputPath);
    }

    public class ReportGeneratorService : IPptxReportGenerator, OpenReportViewer.Core.Interfaces.IReportGenerator
    {
        public string Format => "pptx";

        public void Generate(ProjectInfo project, string outputPath) => GeneratePresentation(project, outputPath);

        public void GeneratePresentation(ProjectInfo project, string outputPath)
        {
            ArgumentNullException.ThrowIfNull(project);
            if (string.IsNullOrWhiteSpace(outputPath))
                throw new ArgumentException("Output path cannot be null or empty", nameof(outputPath));

            // Create a presentation at the specified path
            using (PresentationDocument pres = PresentationDocument.Create(outputPath, PresentationDocumentType.Presentation))
            {
                PresentationPart presentationPart = pres.AddPresentationPart();
                presentationPart.Presentation = new Presentation();

                CreatePresentationParts(presentationPart);

                // Add Title Slide
                AddSlide(pres, "Live Optics Analysis Report", $"Project: {project.ProjectName}");

                // Add Summary Slide
                AddSlide(pres, "Executive Summary",
                    $"Analyzed {project.Servers?.Count ?? 0} servers.\n" +
                    $"Total CPU Cores: {GetTotalCpu(project)}\n" +
                    $"Total Memory: {GetTotalMem(project)} GB");
                    
                // Add AI Insights Slide (Placeholder)
                AddSlide(pres, "AI Insights", "Performance analysis suggests optimization opportunities in disk tiering.");
                
                pres.Save();
            }
        }

        private int GetTotalCpu(ProjectInfo p)
        {
            int total = 0;
            if (p.Servers != null)
            {
                foreach(var s in p.Servers) total += s.CPUCount;
            }
            return total;
        }

        private double GetTotalMem(ProjectInfo p)
        {
            double total = 0;
            if (p.Servers != null)
            {
                foreach(var s in p.Servers) total += s.MemoryGB;
            }
            return total;
        }

        private void CreatePresentationParts(PresentationPart presentationPart)
        {
            SlideMasterIdList slideMasterIdList = new SlideMasterIdList(new SlideMasterId() { Id = (UInt32Value)2147483648U, RelationshipId = "rId1" });
            SlideIdList slideIdList = new SlideIdList(new SlideId() { Id = (UInt32Value)256U, RelationshipId = "rId2" });
            SlideSize slideSize = new SlideSize() { Cx = 9144000, Cy = 6858000, Type = SlideSizeValues.Screen4x3 };
            NotesSize notesSize = new NotesSize() { Cx = 6858000, Cy = 9144000 };
            DefaultTextStyle defaultTextStyle = new DefaultTextStyle();

            presentationPart.Presentation.Append(slideMasterIdList, slideIdList, slideSize, notesSize, defaultTextStyle);

            SlideMasterPart slideMasterPart = presentationPart.AddNewPart<SlideMasterPart>("rId1");
            SlideMaster slideMaster = new SlideMaster(
                new CommonSlideData(new ShapeTree(
                    new P.NonVisualGroupShapeProperties(
                        new P.NonVisualDrawingProperties() { Id = (UInt32Value)1U, Name = "" },
                        new P.NonVisualGroupShapeDrawingProperties(),
                        new P.ApplicationNonVisualDrawingProperties()),
                    new P.GroupShapeProperties(new A.Transform2D(new A.Offset() { X = 0L, Y = 0L }, new A.Extents() { Cx = 0L, Cy = 0L }), new A.GroupShapeLocks() { NoGrouping = true }),
                    new P.Shape(
                        new P.NonVisualShapeProperties(
                            new P.NonVisualDrawingProperties() { Id = (UInt32Value)2U, Name = "Title Placeholder 1" },
                            new P.NonVisualShapeDrawingProperties(new A.ShapeLocks() { NoGrouping = true }),
                            new P.ApplicationNonVisualDrawingProperties(new PlaceholderShape() { Type = PlaceholderValues.Title })),
                        new P.ShapeProperties(),
                        new P.TextBody(new A.BodyProperties(), new A.ListStyle(), new A.Paragraph()))
                )),
                new P.ColorMap() { Background1 = A.ColorSchemeIndexValues.Light1, Text1 = A.ColorSchemeIndexValues.Dark1, Background2 = A.ColorSchemeIndexValues.Light2, Text2 = A.ColorSchemeIndexValues.Dark2, Accent1 = A.ColorSchemeIndexValues.Accent1, Accent2 = A.ColorSchemeIndexValues.Accent2, Accent3 = A.ColorSchemeIndexValues.Accent3, Accent4 = A.ColorSchemeIndexValues.Accent4, Accent5 = A.ColorSchemeIndexValues.Accent5, Accent6 = A.ColorSchemeIndexValues.Accent6, Hyperlink = A.ColorSchemeIndexValues.Hyperlink, FollowedHyperlink = A.ColorSchemeIndexValues.FollowedHyperlink }
            );
            slideMasterPart.SlideMaster = slideMaster;

            SlideLayoutPart slideLayoutPart = slideMasterPart.AddNewPart<SlideLayoutPart>("rId1");
            SlideLayout slideLayout = new SlideLayout(new CommonSlideData(new ShapeTree(
                new P.NonVisualGroupShapeProperties(
                    new P.NonVisualDrawingProperties() { Id = (UInt32Value)1U, Name = "" },
                    new P.NonVisualGroupShapeDrawingProperties(),
                    new P.ApplicationNonVisualDrawingProperties()),
                new P.GroupShapeProperties(new A.Transform2D(new A.Offset() { X = 0L, Y = 0L }, new A.Extents() { Cx = 0L, Cy = 0L }), new A.GroupShapeLocks() { NoGrouping = true }),
                new P.Shape(
                    new P.NonVisualShapeProperties(
                        new P.NonVisualDrawingProperties() { Id = (UInt32Value)2U, Name = "" },
                        new P.NonVisualShapeDrawingProperties(),
                        new P.ApplicationNonVisualDrawingProperties()),
                    new P.ShapeProperties(),
                    new P.TextBody(new A.BodyProperties(), new A.ListStyle(), new A.Paragraph()))
            )));
            slideLayoutPart.SlideLayout = slideLayout;

            slideMaster.SlideLayoutIdList = new SlideLayoutIdList(new SlideLayoutId() { Id = (UInt32Value)2147483649U, RelationshipId = "rId1" });
            
            ThemePart themePart = slideMasterPart.AddNewPart<ThemePart>("rId5");
            A.Theme theme = new A.Theme() { Name = "Office Theme" }; // Simplified Theme
            theme.ThemeElements = new A.ThemeElements(new A.ColorScheme() { Name = "Office" }, new A.FontScheme() { Name = "Office" }, new A.FormatScheme() { Name = "Office" }); // Minimal Elements
            themePart.Theme = theme;
        }

        private void AddSlide(PresentationDocument pres, string title, string text)
        {
            var presentationPart = pres.PresentationPart;
            if (presentationPart == null)
                throw new InvalidOperationException("PresentationPart is null");

            var slidePart = presentationPart.AddNewPart<SlidePart>();
            
            // Minimal Slide Structure
            slidePart.Slide = new Slide(
                new CommonSlideData(
                    new ShapeTree(
                        new P.NonVisualGroupShapeProperties(
                            new P.NonVisualDrawingProperties() { Id = (UInt32Value)1U, Name = "" },
                            new P.NonVisualGroupShapeDrawingProperties(),
                            new P.ApplicationNonVisualDrawingProperties()),
                        new P.GroupShapeProperties(new A.Transform2D(new A.Offset() { X = 0L, Y = 0L }, new A.Extents() { Cx = 0L, Cy = 0L }), new A.GroupShapeLocks() { NoGrouping = true }),
                        
                        // Title Shape
                        new P.Shape(
                             new P.NonVisualShapeProperties(
                                new P.NonVisualDrawingProperties() { Id = (UInt32Value)2U, Name = "Title 1" },
                                new P.NonVisualShapeDrawingProperties(new A.ShapeLocks() { NoGrouping = true }),
                                new P.ApplicationNonVisualDrawingProperties(new PlaceholderShape() { Type = PlaceholderValues.Title })),
                            new P.ShapeProperties(),
                            new P.TextBody(
                                new A.BodyProperties(), new A.ListStyle(),
                                new A.Paragraph(new A.Run(new A.Text(title)))))
                        ,
                        // Content Shape
                        new P.Shape(
                             new P.NonVisualShapeProperties(
                                new P.NonVisualDrawingProperties() { Id = (UInt32Value)3U, Name = "Content 1" },
                                new P.NonVisualShapeDrawingProperties(new A.ShapeLocks() { NoGrouping = true }),
                                new P.ApplicationNonVisualDrawingProperties(new PlaceholderShape() { Index = (UInt32Value)1U })),
                            new P.ShapeProperties(),
                            new P.TextBody(
                                new A.BodyProperties(), new A.ListStyle(),
                                new A.Paragraph(new A.Run(new A.Text(text)))))
                    )
                ),
                new ColorMapOverride(new A.MasterColorMapping())
            );

            // Link slide to layout part relationship
            var slideMasterPart = presentationPart.SlideMasterParts.FirstOrDefault();
            if (slideMasterPart != null && slideMasterPart.SlideLayoutParts.Any())
            {
                var slideLayoutPart = slideMasterPart.SlideLayoutParts.First();
                slidePart.AddPart(slideLayoutPart, "rId1");
            }
        }
    }
}

namespace OpenReportViewer.Reporting
{
    using Microsoft.Extensions.DependencyInjection;
    using OpenReportViewer.Core.Interfaces;

    public static class ReportingServiceCollectionExtensions
    {
        public static IServiceCollection AddOpenReportViewerReporting(this IServiceCollection services)
        {
            services.AddSingleton<ReportGeneratorService>();
            services.AddSingleton<IPptxReportGenerator>(sp => sp.GetRequiredService<ReportGeneratorService>());
            services.AddSingleton<IReportGenerator>(sp => sp.GetRequiredService<ReportGeneratorService>());
            return services;
        }
    }
}

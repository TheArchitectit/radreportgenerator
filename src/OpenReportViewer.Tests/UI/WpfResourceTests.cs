using System.Text.RegularExpressions;
using Xunit;

namespace OpenReportViewer.Tests.UI
{
    /// <summary>
    /// Smoke: converter resource is defined and referenced. Full interactive WPF launch
    /// is environment-dependent; this test validates the XAML contract that causes runtime failure.
    /// </summary>
    public class WpfResourceTests
    {
        private static string FindSrcFile(string relative)
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null)
            {
                var p = Path.Combine(dir.FullName, relative);
                if (File.Exists(p)) return p;
                dir = dir.Parent;
            }
            throw new FileNotFoundException(relative);
        }

        [Fact]
        public void AppXaml_DefinesBooleanToVisibilityConverter()
        {
            var path = FindSrcFile(Path.Combine("src", "OpenReportViewer.UI.Wpf", "App.xaml"));
            var xaml = File.ReadAllText(path);
            Assert.Contains("BooleanToVisibilityConverter", xaml);
            Assert.Contains("x:Key=\"BooleanToVisibilityConverter\"", xaml);
        }

        [Fact]
        public void MainWindowXaml_ReferencesConverterResource()
        {
            var path = FindSrcFile(Path.Combine("src", "OpenReportViewer.UI.Wpf", "MainWindow.xaml"));
            var xaml = File.ReadAllText(path);
            Assert.Contains("{StaticResource BooleanToVisibilityConverter}", xaml);
        }

        [Fact]
        public void MainWindowXaml_BindsBusyAndCharts()
        {
            var path = FindSrcFile(Path.Combine("src", "OpenReportViewer.UI.Wpf", "MainWindow.xaml"));
            var xaml = File.ReadAllText(path);
            Assert.Contains("IsBusy", xaml);
            Assert.Contains("ChartTopTitle", xaml);
            Assert.Contains("VmCount", xaml);
        }
    }
}

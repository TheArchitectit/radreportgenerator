namespace OpenReportViewer.Core.Diagnostics
{
    public interface IAppLog
    {
        void Info(string message);
        void Warn(string message);
        void Error(string message, Exception? ex = null);
        IReadOnlyList<string> Recent { get; }
    }

    public sealed class FileAppLog : IAppLog
    {
        private readonly string _path;
        private readonly object _gate = new();
        private readonly Queue<string> _recent = new();
        private const int MaxRecent = 200;

        public FileAppLog(string? directory = null)
        {
            var dir = directory ?? Path.Combine(AppContext.BaseDirectory, "logs");
            Directory.CreateDirectory(dir);
            _path = Path.Combine(dir, $"openreportviewer-{DateTime.Now:yyyyMMdd}.log");
        }

        public IReadOnlyList<string> Recent
        {
            get { lock (_gate) return _recent.ToArray(); }
        }

        public void Info(string message) => Write("INFO", message, null);
        public void Warn(string message) => Write("WARN", message, null);
        public void Error(string message, Exception? ex = null) => Write("ERROR", message, ex);

        private void Write(string level, string message, Exception? ex)
        {
            var line = $"{DateTime.Now:HH:mm:ss.fff} [{level}] {message}";
            if (ex != null) line += $" :: {ex.GetType().Name}: {ex.Message}";
            lock (_gate)
            {
                _recent.Enqueue(line);
                while (_recent.Count > MaxRecent) _recent.Dequeue();
                try { File.AppendAllText(_path, line + Environment.NewLine); }
                catch { /* logging must not throw */ }
            }
        }
    }
}

namespace OpenReportViewer.Core.Configuration
{
    using Microsoft.Extensions.DependencyInjection;
    using OpenReportViewer.Core.Diagnostics;

    public static class LoggingServiceCollectionExtensions
    {
        public static IServiceCollection AddOpenReportViewerLogging(this IServiceCollection services)
        {
            services.AddSingleton<IAppLog, FileAppLog>();
            return services;
        }
    }
}

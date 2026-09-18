namespace OpenReportViewer.Core.Diagnostics
{
    public sealed class OperationTrace
    {
        public string Name { get; init; } = "";
        public long DurationMs { get; init; }
        public string Detail { get; init; } = "";
        public bool Success { get; init; }
    }

    public interface IDebugService
    {
        OperationTrace Trace(string name, string detail);
        IReadOnlyList<OperationTrace> Recent { get; }
    }

    public sealed class DebugService : IDebugService
    {
        private readonly IAppLog _log;
        private readonly List<OperationTrace> _recent = new();
        private readonly object _gate = new();

        public DebugService(IAppLog log) => _log = log;

        public IReadOnlyList<OperationTrace> Recent
        {
            get { lock (_gate) return _recent.ToArray(); }
        }

        public OperationTrace Trace(string name, string detail)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            sw.Stop();
            var t = new OperationTrace { Name = name, DurationMs = sw.ElapsedMilliseconds, Detail = detail, Success = true };
            lock (_gate)
            {
                _recent.Add(t);
                if (_recent.Count > 100) _recent.RemoveAt(0);
            }
            _log.Info($"[debug] {name}: {detail}");
            return t;
        }
    }
}

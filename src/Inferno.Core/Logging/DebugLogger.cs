using System.Diagnostics;

namespace Inferno.Core.Logging
{
    public class DebugLogger : ILogger
    {
        public void Log(LogEntry entry)
            => Debug.WriteLine($"[{entry.Severity}] {(entry.Classifier != null ? $"{entry.Classifier}: " : null)}{entry.Message}{(entry.Exception != null ? $"\n{entry.Exception}\n{entry.Exception.Message}" : null)}");
    }
}

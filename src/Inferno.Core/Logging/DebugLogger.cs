using System.Diagnostics;

namespace Inferno.Core.Logging
{
    public class DebugLogger : ILogger
    {
        public void Log(LogEntry entry)
            => Debug.WriteLine($"Log.{entry.Severity}: {entry.Message}{(entry.Exception != null ? $"\n{entry.Exception}" : null)}");
    }
}

using System;

namespace Inferno.Core.Logging
{
    /// <summary>
    /// Immutable DTO that contains the log information.
    /// </summary>
    public class LogEntry
    {
        public readonly LoggingEventType Severity;
        public readonly string Message;
        public readonly Exception Exception;

        public LogEntry(LoggingEventType severity, string message, Exception exception = null)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (message == string.Empty) throw new ArgumentException(nameof(string.Empty), nameof(message));

            this.Severity = severity;
            this.Message = message;
            this.Exception = exception;
        }
    }
}

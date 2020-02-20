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
        public readonly string Classifier;
        public readonly Exception Exception;

        public LogEntry(LoggingEventType severity, string message, string classifier = null, Exception exception = null)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (message == string.Empty) throw new ArgumentException(nameof(string.Empty), nameof(message));

            Severity = severity;
            Message = message;
            Classifier = classifier;
            Exception = exception;
        }
    }
}

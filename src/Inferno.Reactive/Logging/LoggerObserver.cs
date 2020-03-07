using Inferno.Core.Logging;
using System;

namespace Inferno
{
    /// <summary>
    /// An observer that writes to an <see cref="ILogger"/> implementation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LoggerObserver<T> : IObserver<T>
    {
        private readonly ILogger _logger;
        private readonly string _classifier;
        private readonly LoggingEventType _logType;

        public LoggerObserver(ILogger logger, string classifier = null)
            : this(logger, LoggingEventType.Information, classifier) { }

        public LoggerObserver(ILogger logger, LoggingEventType logType, string classifier)
        {
            _logger = logger;
            _logType = logType;
            _classifier = classifier;
        }

        public void OnNext(T value)
        {
            _logger.Log(new LogEntry(_logType, $"OnNext({value})", _classifier));
        }
        public void OnError(Exception error)
        {
            _logger.Log(new LogEntry(LoggingEventType.Error, "OnError", _classifier));
        }
        public void OnCompleted()
        {
            _logger.Log(new LogEntry(_logType, "OnCompleted()", _classifier));
        }
    }
}

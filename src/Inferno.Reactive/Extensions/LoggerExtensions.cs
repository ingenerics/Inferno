using Inferno.Core.Logging;
using System;
using System.Reactive.Linq;

namespace Inferno
{
    /// <summary>
    /// Extension methods associated with the ILogger interface.
    /// </summary>
    public static class ReactiveLoggerExtensions
    {
        private static ILogger _logger;

        internal static void Initialize(ILogger logger)
        {
            _logger = logger;
        }

        public static IObservable<T> Log<T>(this IObservable<T> observable, string classifier = null)
        {
            return observable.Do(
                x => _logger.Log(new LogEntry(LoggingEventType.Debug, $"OnNext({x})", classifier)),
                ex => _logger.Log(new LogEntry(LoggingEventType.Error, "OnError", classifier)),
                () => _logger.Log(new LogEntry(LoggingEventType.Debug, "OnCompleted()", classifier)));
        }

        public static IObservable<T> Log<T>(this IObservable<T> observable, string classifier, LoggingEventType eventType)
        {
            return observable.Do(
                x => _logger.Log(new LogEntry(eventType, $"OnNext({x})", classifier)),
                ex => _logger.Log(new LogEntry(LoggingEventType.Error, "OnError", classifier)),
                () => _logger.Log(new LogEntry(eventType, "OnCompleted()", classifier)));
        }
    }
}

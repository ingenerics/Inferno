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

        public static IObservable<T> Log<T>(this IObservable<T> observable, string classifier, LoggingEventType logType)
        {
            return observable.Do(
                x => _logger.Log(new LogEntry(logType, $"OnNext({x})", classifier)),
                ex => _logger.Log(new LogEntry(LoggingEventType.Error, "OnError", classifier)),
                () => _logger.Log(new LogEntry(logType, "OnCompleted()", classifier)));
        }

        public static IDisposable SubscribeLogger<T>(this IObservable<T> observable, string classifier = null)
        {
            return observable.Subscribe(new LoggerObserver<T>(_logger, classifier));
        }

        public static IDisposable SubscribeLogger<T>(this IObservable<T> observable, LoggingEventType logType, string classifier = null)
        {
            return observable.Subscribe(new LoggerObserver<T>(_logger, logType, classifier));
        }
    }
}

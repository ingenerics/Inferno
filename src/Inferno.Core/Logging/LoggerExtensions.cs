using System;

namespace Inferno.Core.Logging
{
    public static class LoggerExtensions
    {
        public static void LogDebug(this ILogger logger, object classifier, string message)
        {
            logger.Log(new LogEntry(LoggingEventType.Debug, message, classifier.GetType().Name));
        }

        public static void LogDebug(this ILogger logger, string message)
        {
            logger.Log(new LogEntry(LoggingEventType.Debug, message));
        }

        public static void LogInformation(this ILogger logger, object classifier, string message)
        {
            logger.Log(new LogEntry(LoggingEventType.Information, message, classifier.GetType().Name));
        }

        public static void LogInformation(this ILogger logger, string message)
        {
            logger.Log(new LogEntry(LoggingEventType.Information, message));
        }

        public static void LogWarning(this ILogger logger, object classifier, string message)
        {
            logger.Log(new LogEntry(LoggingEventType.Warning, message, classifier.GetType().Name));
        }

        public static void LogWarning(this ILogger logger, string message)
        {
            logger.Log(new LogEntry(LoggingEventType.Warning, message));
        }

        public static void LogError(this ILogger logger, object classifier, Exception exception)
        {
            logger.Log(new LogEntry(LoggingEventType.Error, exception.Message, classifier.GetType().Name, exception));
        }

        public static void LogError(this ILogger logger, Exception exception)
        {
            logger.Log(new LogEntry(LoggingEventType.Error, exception.Message, exception: exception));
        }

        public static void LogFatal(this ILogger logger, object classifier, Exception exception)
        {
            logger.Log(new LogEntry(LoggingEventType.Fatal, exception.Message, classifier.GetType().Name, exception));
        }

        public static void LogFatal(this ILogger logger, Exception exception)
        {
            logger.Log(new LogEntry(LoggingEventType.Fatal, exception.Message, exception: exception));
        }
    }
}

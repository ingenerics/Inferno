using Inferno.Core.Logging;
using System;

namespace Inferno
{
    /// <summary>
    /// Extension methods associated with the <see cref="IEnableLogger"/> interface.
    /// </summary>
    public static class IEnableLoggerExtensions
    {
        private static ILogger _logger;

        internal static void Initialize(ILogger logger)
        {
            _logger = logger;
        }

        public static void LogDebug(this IEnableLogger _, object classifier, string message)
            => _logger.LogDebug(classifier, message);

        public static void LogDebug(this IEnableLogger _, string message)
            => _logger.LogDebug(message);

        public static void LogInformation(this IEnableLogger _, object classifier, string message)
            => _logger.LogInformation(classifier, message);

        public static void LogInformation(this IEnableLogger _, string message)
            => _logger.LogInformation(message);

        public static void LogWarning(this IEnableLogger _, object classifier, string message)
            => _logger.LogWarning(classifier, message);

        public static void LogWarning(this IEnableLogger _, string message)
            => _logger.LogWarning(message);

        public static void LogError(this IEnableLogger _, object classifier, Exception exception, string message)
            => _logger.LogError(classifier, exception, message);

        public static void LogError(this IEnableLogger _, object classifier, Exception exception)
            => _logger.LogError(classifier, exception);

        public static void LogError(this IEnableLogger _, Exception exception)
            => _logger.LogError(exception);

        public static void LogFatal(this IEnableLogger _, object classifier, Exception exception, string message)
            => _logger.LogFatal(classifier, exception, message);

        public static void LogFatal(this IEnableLogger _, object classifier, Exception exception)
            => _logger.LogFatal(classifier, exception);

        public static void LogFatal(this IEnableLogger _, Exception exception)
            => _logger.LogFatal(exception);
    }
}

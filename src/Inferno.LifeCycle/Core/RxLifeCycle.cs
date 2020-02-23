using Inferno.Core;
using Inferno.Core.Logging;

namespace Inferno
{
    /// <summary>
    /// The main orchestrator for lifecycle events in a Inferno application.
    /// </summary>
    public static class RxLifeCycle
    {
        private static ILogger _logger;
        private static IDependencyResolver _dependencyResolver;

        public static void Initialize(IDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
            _logger = _dependencyResolver.GetInstance<ILogger>();

            #region Configuration of static classes and shared state

            Screen.Logger = _logger;

            var sinkForViewFetchers = _dependencyResolver.GetAllInstances<ISinkForViewFetcher>();
            ViewAwareExtensions.Initialize(sinkForViewFetchers);

            #endregion Configuration of static classes and shared state
        }
    }
}

using Inferno.Core;
using Inferno.Core.Logging;

namespace Inferno
{
    /// <summary>
    /// The main orchestrator that initializes the Inferno lifecycle.
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

            var loadedForViewFetchers = _dependencyResolver.GetAllInstances<ILoadedForViewFetcher>();
            ViewAwareExtensions.Initialize(_logger, loadedForViewFetchers);

            #endregion Configuration of static classes and shared state
        }
    }
}

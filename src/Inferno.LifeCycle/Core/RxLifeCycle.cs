using Inferno.Core;

namespace Inferno
{
    /// <summary>
    /// The main orchestrator for lifecycle events in a Inferno application.
    /// </summary>
    public static class RxLifeCycle
    {
        private static IDependencyResolver _dependencyResolver;

        public static void Initialize(IDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;

            #region Configuration of static classes and shared state

            var sinkForViewFetchers = _dependencyResolver.GetAllInstances<ISinkForViewFetcher>();
            ViewAwareExtensions.Initialize(sinkForViewFetchers);

            #endregion Configuration of static classes and shared state
        }
    }
}

using System;
using System.Reactive.Subjects;

namespace Inferno.LifeCycle.Tests
{
    /// <summary>
    /// FakeSinkForLoadedViewFetcher is used to simulate view loading / unloading during tests.
    /// </summary>
    public class FakeSinkForLoadedViewFetcher : ISinkForViewFetcher
    {
        private readonly Subject<bool> _viewLoaded;

        public FakeSinkForLoadedViewFetcher()
        {
            _viewLoaded = new Subject<bool>();
        }

        public int GetAffinityForView(Type view) => 0; // FakeSinkForLoadedViewFetcher will be decorated

        public IObservable<bool> GetSinkForView(object view) => _viewLoaded;

        public void LoadView() => _viewLoaded.OnNext(true);
        public void UnloadView() => _viewLoaded.OnNext(false);

        public void Dispose()
        {
            _viewLoaded?.Dispose();
        }
    }
}

using System;
using System.Reactive.Linq;

namespace Inferno.LifeCycle.Tests
{
    public class TestLoadedForViewFetcher : ILoadedForViewFetcher
    {
        public int GetAffinityForView(Type view)
        {
            return typeof(ITestView).IsAssignableFrom(view) ? 100 : 0;
        }

        public IObservable<bool> GetLoadedForView(object view)
        {
            var tv = (ITestView)view;
            return tv.Loaded.Select(_ => true).Merge(tv.Unloaded.Select(_ => false));
        }
    }
}

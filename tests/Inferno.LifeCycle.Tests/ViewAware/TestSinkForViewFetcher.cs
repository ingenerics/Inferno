using System;
using System.Reactive.Linq;

namespace Inferno.LifeCycle.Tests
{
    public class TestSinkForViewFetcher : ISinkForViewFetcher
    {
        public int GetAffinityForView(Type view)
        {
            return view == typeof(LoadingView) ? 100 : 0;
        }

        public IObservable<bool> GetSinkForView(object view)
        {
            var lv = view as LoadingView;
            return lv.Loaded.Select(_ => true).Merge(lv.Unloaded.Select(_ => false));
        }
    }
}

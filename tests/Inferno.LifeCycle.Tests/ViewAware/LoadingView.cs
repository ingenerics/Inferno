using System.Reactive.Disposables;

namespace Inferno.LifeCycle.Tests
{
    public sealed class LoadingView : TestViewBase<ViewAwareViewModel>
    {
        public LoadingView()
        {
            this.WhenLoaded(d =>
            {
                IsLoadedCount++;
                d(Disposable.Create(() => IsLoadedCount--));
            });
        }

        public int IsLoadedCount { get; set; }
    }
}

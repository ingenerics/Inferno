using System.Reactive.Disposables;

namespace Inferno.LifeCycle.Tests
{
    public class ViewAwareViewModel : ReactiveObject, IViewAware
    {
        public ViewAwareViewModel()
        {
            View = new ViewSink();

            this.WhenLoaded(d =>
            {
                IsLoadedCount++;
                d(Disposable.Create(() => IsLoadedCount--));
            });
        }

        public ViewSink View { get; protected set; }

        public int IsLoadedCount { get; protected set; }
    }
}

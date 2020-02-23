using System.Reactive.Disposables;

namespace Inferno.LifeCycle.Tests
{
    public class DerivedViewAwareViewModel : ViewAwareViewModel
    {
        public DerivedViewAwareViewModel()
        {
            this.WhenLoaded(d =>
            {
                IsLoadedCountAlso++;
                d(Disposable.Create(() => IsLoadedCountAlso--));
            });
        }

        public int IsLoadedCountAlso { get; protected set; }
    }
}

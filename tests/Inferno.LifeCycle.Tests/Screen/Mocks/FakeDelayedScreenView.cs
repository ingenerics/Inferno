using System.Reactive.Disposables;

namespace Inferno.LifeCycle.Tests
{
    internal class FakeDelayedScreenView : TestViewBase<DelayedScreen>
    {
        public FakeDelayedScreenView(DelayedScreen screen)
        {
            ViewModel = screen;

            this.WhenLoaded(disposables =>
            {
                IsActivated = true;

                Disposable.Create(() => IsActivated = false).DisposeWith(disposables);
            });
        }

        public bool IsActivated { get; private set; }
    }
}
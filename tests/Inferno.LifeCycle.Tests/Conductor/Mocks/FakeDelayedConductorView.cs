using System.Reactive.Disposables;

namespace Inferno.LifeCycle.Tests
{
    internal class FakeDelayedConductorView : TestViewBase<DelayedConductor>
    {
        public FakeDelayedConductorView(DelayedConductor conductor)
        {
            ViewModel = conductor;

            this.WhenLoaded(disposables =>
            {
                IsActivated = true;

                Disposable.Create(() => IsActivated = false).DisposeWith(disposables);
            });
        }

        public bool IsActivated { get; private set; }
    }
}
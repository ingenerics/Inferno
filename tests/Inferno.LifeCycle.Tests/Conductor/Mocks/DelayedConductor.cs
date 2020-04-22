using Microsoft.Reactive.Testing;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;

namespace Inferno.LifeCycle.Tests
{
    internal class DelayedConductor : Conductor<DelayedScreen>
    {
        private readonly TestScheduler _scheduler;

        public long WhenInitializedScopeStartTime { get; private set; }

        // "Simulate" Async work that needs to be completed before activating screen.
        protected override Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            _scheduler.AdvanceBy(200);

            return base.OnInitializeAsync(cancellationToken);
        }

        // "Simulate" Async work that needs to be completed before executing WhenXXX scopes.
        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            _scheduler.AdvanceBy(100);

            return base.OnActivateAsync(cancellationToken);
        }

        // Ctor positioned here to show scheduler flow (scopes) chronologically
        public DelayedConductor(TestScheduler scheduler)
        {
            _scheduler = scheduler;

            this.WhenInitialized(disposables =>
            {
                WhenInitializedScopeStartTime = _scheduler.Clock;

                _scheduler.AdvanceBy(5);
                disposables.Add(Disposable.Empty);
            });

            this.WhenActivated(disposables =>
            {
                _scheduler.AdvanceBy(5);
                disposables.Add(Disposable.Empty);
            });
        }
    }
}

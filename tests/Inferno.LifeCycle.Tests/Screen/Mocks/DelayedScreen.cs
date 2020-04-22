using Microsoft.Reactive.Testing;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;

namespace Inferno.LifeCycle.Tests
{
    internal class DelayedScreen : Screen
    {
        private readonly TestScheduler _scheduler;

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
        public DelayedScreen(TestScheduler scheduler)
        {
            _scheduler = scheduler;

            // These scopes (WhenInitialized and WhenActivated) are typically used to hook everything up,
            // ie. the internal plumbing of the view model. Eg which command listens to which observable, etc.
            // The important thing in the scope of screen lifecycle is that these scopes don't block,
            // ie. in contrast to when using await in OnInitializeAsync or OnActivateAsync, after the hookups are done
            // these scopes will give back control to the runtime without waiting for the pipelines to do any processing or complete.
            this.WhenInitialized(disposables =>
            {
                _scheduler.AdvanceBy(5);
                disposables.Add(Disposable.Empty);
            });

            this.WhenActivated(disposables =>
            {
                _scheduler.AdvanceBy(5);
                IsFullyActivated = true;
                disposables.Add(Disposable.Empty);
            });
        }

        private bool _isFullyActivated;
        public bool IsFullyActivated
        {
            get => _isFullyActivated;
            set => this.RaiseAndSetIfChanged(ref _isFullyActivated, value);
        }
    }
}

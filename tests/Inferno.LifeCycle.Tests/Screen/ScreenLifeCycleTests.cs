using Inferno.Core;
using Microsoft.Reactive.Testing;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Xunit;

namespace Inferno.LifeCycle.Tests
{
    public class ScreenLifeCycleTests
    {
        private class DelayedScreen : Screen
        {
            private readonly TestScheduler _scheduler;

            // Simulate Async work that needs to be completed before activating screen.
            protected override Task OnInitializeAsync(CancellationToken cancellationToken)
            {
                _scheduler.AdvanceBy(200);

                return base.OnInitializeAsync(cancellationToken);
            }

            // Simulate Async work that needs to be completed before executing WhenXXX scopes.
            protected override Task OnActivateAsync(CancellationToken cancellationToken)
            {
                _scheduler.AdvanceBy(100);

                return base.OnActivateAsync(cancellationToken);
            }

            // Ctor positioned here to show scheduler flow chronologically
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
                    disposables.Add(Disposable.Empty);
                });
            }
        }

        private class FakeDelayedView : FrameworkElement, IViewFor
        {
            public FakeDelayedView(DelayedScreen screen)
            {
                ViewModel = screen;
            }

            public object ViewModel { get; set; }
        }

        [Fact]
        public async Task ActivationCompletesWhenAllPartsHaveCompleted()
        {
            var scheduler = new TestScheduler();
            var screen = new DelayedScreen(scheduler);

            await ((IActivate)screen).ActivateAsync(CancellationToken.None);

            Assert.Equal(310, scheduler.Clock);
        }

        [StaFact]
        public async Task ViewIsOnlyActivatedWhenViewModelActivationHasCompleted()
        {
            var scheduler = new TestScheduler();
            var screen = new DelayedScreen(scheduler);
            var view = new FakeDelayedView(screen);

            var fakeSinkForLoadedViewFetcher = new FakeSinkForLoadedViewFetcher();
            var sinkForViewFetcher = new SinkForActivatedViewFetcher(fakeSinkForLoadedViewFetcher);
            var isViewActivated = false;

            var subscription =
                sinkForViewFetcher
                    .GetSinkForView(view)
                    .Do(isActive => isViewActivated = isActive)
                    .Subscribe();

            Assert.Equal(false, isViewActivated);

            // WPF raises the ViewLoaded event on FrameworkElement
            fakeSinkForLoadedViewFetcher.LoadView();

            Assert.Equal(false, isViewActivated);

            // The Screen's Conductor triggers ActivateAsync
            await ((IActivate)screen).ActivateAsync(CancellationToken.None);

            Assert.Equal(true, isViewActivated);

            subscription.Dispose();
            fakeSinkForLoadedViewFetcher.Dispose();
        }

        [StaFact]
        public async Task ViewIsOnlyActivatedWhenViewIsAddedToVisualTree()
        {
            var scheduler = new TestScheduler();
            var screen = new DelayedScreen(scheduler);
            var view = new FakeDelayedView(screen);

            var fakeSinkForLoadedViewFetcher = new FakeSinkForLoadedViewFetcher();
            var sinkForViewFetcher = new SinkForActivatedViewFetcher(fakeSinkForLoadedViewFetcher);
            var isViewActivated = false;

            var subscription =
                sinkForViewFetcher
                    .GetSinkForView(view)
                    .Do(isActive => isViewActivated = isActive)
                    .Subscribe();

            Assert.Equal(false, isViewActivated);

            // The Screen's Conductor triggers ActivateAsync
            await ((IActivate)screen).ActivateAsync(CancellationToken.None);

            Assert.Equal(false, isViewActivated);

            // WPF raises the ViewLoaded event on FrameworkElement
            fakeSinkForLoadedViewFetcher.LoadView();

            Assert.Equal(true, isViewActivated);

            subscription.Dispose();
            fakeSinkForLoadedViewFetcher.Dispose();
        }
    }
}

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
    public class ConductorLifeCycleTests
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

        private class FakeDelayedScreenView : FrameworkElement, IViewFor
        {
            public FakeDelayedScreenView(DelayedScreen screen)
            {
                ViewModel = screen;
            }

            public object ViewModel { get; set; }
        }

        private class DelayedConductor : Conductor<DelayedScreen>
        {
            private readonly TestScheduler _scheduler;

            public long WhenInitializedScopeStartTime { get; private set; }

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
            public DelayedConductor(TestScheduler scheduler)
            {
                _scheduler = scheduler;

                // These scopes (WhenInitialized and WhenActivated) are typically used to hook everything up,
                // ie. the internal plumbing of the view model. Eg which command listens to which observable, etc.
                // The important thing in the scope of screen lifecycle is that these scopes don't block,
                // ie. in contrast to when using await in OnInitializeAsync or OnActivateAsync, after the hookups are done
                // these scopes will give back control to the runtime without waiting for the pipelines to do any processing or complete.
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

        private class FakeDelayedConductorView : FrameworkElement, IViewFor
        {
            public FakeDelayedConductorView(DelayedConductor conductor)
            {
                ViewModel = conductor;
            }

            public object ViewModel { get; set; }
        }

        [Fact]
        public async Task ActivationCompletesWhenAllPartsHaveCompleted()
        {
            var scheduler = new TestScheduler();
            var screen = new DelayedScreen(scheduler);
            var conductor = new DelayedConductor(scheduler);

            conductor.ActiveItem = screen;
            await ((IActivate)conductor).ActivateAsync(CancellationToken.None);

            Assert.Equal(620, scheduler.Clock);
        }

        [StaFact]
        public async Task ViewIsOnlyActivatedWhenViewModelActivationHasCompleted()
        {
            var scheduler = new TestScheduler();
            var screen = new DelayedScreen(scheduler);
            var screenView = new FakeDelayedScreenView(screen);
            var conductor = new DelayedConductor(scheduler);
            var conductorView = new FakeDelayedConductorView(conductor);

            conductor.ActiveItem = screen;

            var fakeSinkForLoadedViewFetcher = new FakeSinkForLoadedViewFetcher();
            var sinkForViewFetcher = new SinkForActivatedViewFetcher(fakeSinkForLoadedViewFetcher);
            var isScreenViewActivated = false;
            var isConductorViewActivated = false;

            var subscriptions = new CompositeDisposable();

            subscriptions.Add(
                sinkForViewFetcher
                    .GetSinkForView(screenView)
                    .Do(isActive => isScreenViewActivated = isActive)
                    .Subscribe());

            subscriptions.Add(
                sinkForViewFetcher
                    .GetSinkForView(conductorView)
                    .Do(isActive => isConductorViewActivated = isActive)
                    .Subscribe());

            Assert.Equal(false, isScreenViewActivated);
            Assert.Equal(false, isConductorViewActivated);

            // WPF raises the ViewLoaded event.
            // Note this event will be raised on root and will then be raised successively on all children.
            // Ref MSDN https://docs.microsoft.com/en-us/dotnet/framework/wpf/advanced/object-lifetime-events
            // In other words, no need to split this up for the Conductor and the Screen.
            fakeSinkForLoadedViewFetcher.LoadView();

            Assert.Equal(false, isScreenViewActivated);
            Assert.Equal(false, isConductorViewActivated);

            // The Conductor's Conductor triggers ActivateAsync
            await ((IActivate)conductor).ActivateAsync(CancellationToken.None);

            Assert.Equal(true, isScreenViewActivated);
            Assert.Equal(true, isConductorViewActivated);

            subscriptions.Dispose();
            fakeSinkForLoadedViewFetcher.Dispose();
        }

        [StaFact]
        public async Task ViewIsOnlyActivatedWhenViewIsAddedToVisualTree()
        {
            var scheduler = new TestScheduler();
            var screen = new DelayedScreen(scheduler);
            var screenView = new FakeDelayedScreenView(screen);
            var conductor = new DelayedConductor(scheduler);
            var conductorView = new FakeDelayedConductorView(conductor);

            conductor.ActiveItem = screen;

            var fakeSinkForLoadedViewFetcher = new FakeSinkForLoadedViewFetcher();
            var sinkForViewFetcher = new SinkForActivatedViewFetcher(fakeSinkForLoadedViewFetcher);
            var isScreenViewActivated = false;
            var isConductorViewActivated = false;

            var subscriptions = new CompositeDisposable();

            subscriptions.Add(
                sinkForViewFetcher
                    .GetSinkForView(screenView)
                    .Do(isActive => isScreenViewActivated = isActive)
                    .Subscribe());

            subscriptions.Add(
                sinkForViewFetcher
                    .GetSinkForView(conductorView)
                    .Do(isActive => isConductorViewActivated = isActive)
                    .Subscribe());

            Assert.Equal(false, isScreenViewActivated);
            Assert.Equal(false, isConductorViewActivated);

            // The Conductor's Conductor triggers ActivateAsync
            await ((IActivate)conductor).ActivateAsync(CancellationToken.None);

            Assert.Equal(false, isScreenViewActivated);
            Assert.Equal(false, isConductorViewActivated);

            // WPF raises the ViewLoaded event.
            // Note this event will be raised on root and will then be raised successively on all children.
            // Ref MSDN https://docs.microsoft.com/en-us/dotnet/framework/wpf/advanced/object-lifetime-events
            // In other words, no need to split this up for the Conductor and the Screen.
            fakeSinkForLoadedViewFetcher.LoadView();

            Assert.Equal(true, isScreenViewActivated);
            Assert.Equal(true, isConductorViewActivated);

            subscriptions.Dispose();
            fakeSinkForLoadedViewFetcher.Dispose();
        }

        /*
         * Consider this vertical timeline representing the first activation of the conductor and its only item.
         *
         *      Conductor                           Screen
         *
         *   |    OnInitializeAsync
         *   |
         *   |    OnActivateAsync    ---------->      OnInitializeAsync
         *   |
         *   |                                        OnActivateAsync
         *   |
         *   |                                        WhenInitialized
         *   |
         *   |    WhenInitialized    <----------      WhenActivated
         *   |
         *   |    WhenActivated
         *  \|/
         *
         * The main take away is that the Conductor's WhenInitialized is only executed once the Screen has been fully activated.
         * This ensures child properties will have been properly initialized when referencing them in parent scopes.
         * Furthermore, as a conducted Screen will be deactivated when it's conductor deactivates, we can safely assume the
         * View's WhenLoaded scope will only be executed after all View Model activation is done. This is an important feature,
         * as it enables us to offload a lot of work from the UIThread.
         * To be sure, if we set up our bindings in the View's WhenLoaded scope, we can save our view from re-rendering due to
         * building up viewmodel state (eg filling up collections, initializing/activating children, etc.) after we've already
         * bound them. Of course, observables set up in the WhenXXX scopes might still cause re-rendering, which is desired.
         */

        [StaFact]
        public async Task ConductorScopesAreOnlyExecutedAfterActiveItemIsFullyActivated()
        {
            var scheduler = new TestScheduler();
            var screen = new DelayedScreen(scheduler);
            var screenView = new FakeDelayedScreenView(screen);
            var conductor = new DelayedConductor(scheduler)
            {
                ActiveItem = screen
            };

            var fakeSinkForLoadedViewFetcher = new FakeSinkForLoadedViewFetcher();
            var sinkForViewFetcher = new SinkForActivatedViewFetcher(fakeSinkForLoadedViewFetcher);

            long screenActivationTime = 0;

            var subscription = 
                sinkForViewFetcher
                .GetSinkForView(screenView)
                .Do(_ => screenActivationTime = scheduler.Clock)
                .Subscribe();

            // WPF raises the ViewLoaded event.
            fakeSinkForLoadedViewFetcher.LoadView();
            // The Conductor's Conductor triggers ActivateAsync
            await ((IActivate)conductor).ActivateAsync(CancellationToken.None);

            Assert.Equal(610, screenActivationTime);
            Assert.Equal(610, conductor.WhenInitializedScopeStartTime);

            subscription.Dispose();
            fakeSinkForLoadedViewFetcher.Dispose();
        }
    }
}

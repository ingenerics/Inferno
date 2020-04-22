using Microsoft.Reactive.Testing;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Inferno.LifeCycle.Tests
{
    public class ConductorLifeCycleTests
    {
        public ConductorLifeCycleTests()
        {
            var dependencyResolver = new FakeDependencyResolverLifeCycle();
            dependencyResolver.ReplaceSingletons<ILoadedForViewFetcher>(new TestLoadedForViewFetcher());
            RxApp.Initialize(dependencyResolver);
            RxLifeCycle.Initialize(dependencyResolver);
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

        [Fact]
        public async Task ActivationCompletesWhenAllPartsHaveCompleted()
        {
            var scheduler = new TestScheduler();
            var screen = new DelayedScreen(scheduler);
            var conductor = new DelayedConductor(scheduler);

            conductor.ActiveItem = screen;
            await ((IActivate)conductor).ActivateAsync(CancellationToken.None);

            // scheduler is passed as a stopwatch
            Assert.Equal(610, conductor.WhenInitializedScopeStartTime);
            // Diff is conductor's scopes (2 * 5)
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

            Assert.Equal(false, screenView.IsActivated);
            Assert.Equal(false, conductorView.IsActivated);

            // WPF raises the ViewLoaded event.
            // Note this event will be raised on root and will then be raised successively on all children.
            // Ref MSDN https://docs.microsoft.com/en-us/dotnet/framework/wpf/advanced/object-lifetime-events
            conductorView.Loaded.OnNext(Unit.Default);
            screenView.Loaded.OnNext(Unit.Default);

            Assert.Equal(false, screenView.IsActivated);
            Assert.Equal(false, conductorView.IsActivated);

            // The Conductor's Conductor triggers ActivateAsync
            await ((IActivate)conductor).ActivateAsync(CancellationToken.None);

            Assert.Equal(true, screenView.IsActivated);
            Assert.Equal(true, conductorView.IsActivated);

            screenView.Unloaded.OnNext(Unit.Default);
            conductorView.Unloaded.OnNext(Unit.Default);
            screenView.Dispose();
            conductorView.Dispose();
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

            Assert.Equal(false, screenView.IsActivated);
            Assert.Equal(false, conductorView.IsActivated);

            // The Conductor's Conductor triggers ActivateAsync
            await ((IActivate)conductor).ActivateAsync(CancellationToken.None);

            Assert.Equal(false, screenView.IsActivated);
            Assert.Equal(false, conductorView.IsActivated);

            // WPF raises the ViewLoaded event.
            // Note this event will be raised on root and will then be raised successively on all children.
            // Ref MSDN https://docs.microsoft.com/en-us/dotnet/framework/wpf/advanced/object-lifetime-events
            conductorView.Loaded.OnNext(Unit.Default);
            screenView.Loaded.OnNext(Unit.Default);

            Assert.Equal(true, screenView.IsActivated);
            Assert.Equal(true, conductorView.IsActivated);

            screenView.Unloaded.OnNext(Unit.Default);
            conductorView.Unloaded.OnNext(Unit.Default);
            screenView.Dispose();
            conductorView.Dispose();
        }

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

            long screenFullyActivatedTime = 0; // Incl scopes
            long conductorActivationTime = 0; // Only async

            var disposables = new CompositeDisposable();
            screen
                .WhenAnyValue(x => x.IsFullyActivated)
                .Do(_ => screenFullyActivatedTime = scheduler.Clock)
                .Subscribe()
                .DisposeWith(disposables);
            conductor
                .WhenAnyValue(x => x.IsActive)
                .Do(_ => conductorActivationTime = scheduler.Clock)
                .Subscribe()
                .DisposeWith(disposables);

            // WPF raises the ViewLoaded event.
            screenView.Loaded.OnNext(Unit.Default);
            // The Conductor's Conductor triggers ActivateAsync
            await ((IActivate)conductor).ActivateAsync(CancellationToken.None);

            Assert.Equal(610, screenFullyActivatedTime);
            Assert.Equal(610, conductorActivationTime);
            Assert.Equal(610, conductor.WhenInitializedScopeStartTime);

            disposables.Dispose();
            screenView.Unloaded.OnNext(Unit.Default);
            screenView.Dispose();
        }

        #region TryClose

        [Fact]
        public async Task ScreenTryCloseAsyncWillAskConductorToCloseIt()
        {
            // conductor conducts conducted
            // conducted.TryCloseAsync() will ask conductor to close it

            var conductor = new Conductor<IScreen>();
            var conducted = new StateScreen { IsClosable = true };

            conductor.ActiveItem = conducted;

            await ((IActivate)conductor).ActivateAsync(CancellationToken.None);
            await conducted.TryCloseAsync();

            Assert.True(conductor.IsActive);
            Assert.False(conducted.IsActive);
            Assert.True(conducted.IsClosed);
        }

        [Fact]
        public async Task ConductorTryCloseAsyncWillAskItsConductorToCloseIt()
        {
            // nothing conducts conductor
            // conductor.TryCloseAsync() will do nothing

            var conductor = new Conductor<IScreen>();
            var conducted = new StateScreen { IsClosable = true };

            conductor.ActiveItem = conducted;

            await ((IActivate)conductor).ActivateAsync(CancellationToken.None);
            await conductor.TryCloseAsync();

            Assert.True(conductor.IsActive);
            Assert.True(conducted.IsActive);
            Assert.False(conducted.IsClosed);
        }

        [Fact]
        public async Task ConductorTryCloseAsyncWillAskItsConductorToCloseItAndItsChildren()
        {
            // conductor1 conducts conductor2
            // conductor2.TryCloseAsync() will ask conductor1 to close it

            var conductor1 = new Conductor<IScreen>();
            var conductor2 = new Conductor<IScreen>();
            var conducted = new StateScreen { IsClosable = true };

            conductor1.ActiveItem = conductor2;
            conductor2.ActiveItem = conducted;

            await ((IActivate)conductor1).ActivateAsync(CancellationToken.None);
            await conductor2.TryCloseAsync();

            Assert.True(conductor1.IsActive);
            Assert.False(conductor2.IsActive);

            Assert.False(conducted.IsActive);
            Assert.True(conducted.IsClosed);
        }

        #endregion TryClose
    }
}

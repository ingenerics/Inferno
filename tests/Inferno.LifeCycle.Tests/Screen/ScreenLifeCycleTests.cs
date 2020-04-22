using System.Reactive;
using Microsoft.Reactive.Testing;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Inferno.LifeCycle.Tests
{
    public class ScreenLifeCycleTests
    {
        public ScreenLifeCycleTests()
        {
            var dependencyResolver = new FakeDependencyResolverLifeCycle();
            dependencyResolver.ReplaceSingletons<ILoadedForViewFetcher>(new TestLoadedForViewFetcher());
            RxApp.Initialize(dependencyResolver);
            RxLifeCycle.Initialize(dependencyResolver);
        }

        [Fact]
        public async Task ActivationCompletesWhenAllPartsHaveCompleted()
        {
            var scheduler = new TestScheduler();
            var screen = new DelayedScreen(scheduler);

            await ((IActivate)screen).ActivateAsync(CancellationToken.None);

            // scheduler is passed as a stopwatch
            Assert.Equal(310, scheduler.Clock);
        }

        [StaFact]
        public async Task ViewIsOnlyActivatedWhenViewModelActivationHasCompleted()
        {
            var scheduler = new TestScheduler();
            var screen = new DelayedScreen(scheduler);
            var view = new FakeDelayedScreenView(screen);

            Assert.Equal(false, view.IsActivated);

            // WPF raises the ViewLoaded event on FrameworkElement
            view.Loaded.OnNext(Unit.Default);

            Assert.Equal(false, view.IsActivated);

            // The Screen's Conductor triggers ActivateAsync
            await ((IActivate)screen).ActivateAsync(CancellationToken.None);

            Assert.Equal(true, view.IsActivated);

            view.Unloaded.OnNext(Unit.Default);
            view.Dispose();
        }

        [StaFact]
        public async Task ViewIsOnlyActivatedWhenViewIsAddedToVisualTree()
        {
            var scheduler = new TestScheduler();
            var screen = new DelayedScreen(scheduler);
            var view = new FakeDelayedScreenView(screen);

            Assert.Equal(false, view.IsActivated);

            // The Screen's Conductor triggers ActivateAsync
            await ((IActivate)screen).ActivateAsync(CancellationToken.None);

            Assert.Equal(false, view.IsActivated);

            // WPF raises the ViewLoaded event on FrameworkElement
            view.Loaded.OnNext(Unit.Default);

            Assert.Equal(true, view.IsActivated);

            view.Unloaded.OnNext(Unit.Default);
            view.Dispose();
        }

        [StaFact]
        public async Task ViewIsDeactivatedWhenViewModelIsDeactivated()
        {
            var scheduler = new TestScheduler();
            var screen = new DelayedScreen(scheduler);
            var view = new FakeDelayedScreenView(screen);

            Assert.Equal(false, view.IsActivated);

            // WPF raises the ViewLoaded event on FrameworkElement
            view.Loaded.OnNext(Unit.Default);
            // The Screen's Conductor triggers ActivateAsync
            await ((IActivate)screen).ActivateAsync(CancellationToken.None);

            Assert.Equal(true, view.IsActivated);

            // The Screen's Conductor triggers DeactivateAsync, without closing
            await ((IActivate)screen).DeactivateAsync(false, CancellationToken.None);

            Assert.Equal(false, view.IsActivated);

            view.Unloaded.OnNext(Unit.Default);
            view.Dispose();
        }

        [StaFact]
        public async Task ViewIsDeactivatedWhenViewIsRemovedFromVisualTree()
        {
            var scheduler = new TestScheduler();
            var screen = new DelayedScreen(scheduler);
            var view = new FakeDelayedScreenView(screen);

            Assert.Equal(false, view.IsActivated);

            // WPF raises the ViewLoaded event on FrameworkElement
            view.Loaded.OnNext(Unit.Default);
            // The Screen's Conductor triggers ActivateAsync
            await ((IActivate)screen).ActivateAsync(CancellationToken.None);

            Assert.Equal(true, view.IsActivated);

            // WPF raises the ViewUnloaded event on FrameworkElement
            view.Unloaded.OnNext(Unit.Default);

            Assert.Equal(false, view.IsActivated);

            view.Dispose();
        }
    }
}

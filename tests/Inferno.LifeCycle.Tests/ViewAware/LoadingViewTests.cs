using System.Reactive;
using Xunit;

namespace Inferno.LifeCycle.Tests
{
    public class LoadingViewTests
    {
        public LoadingViewTests()
        {
            var dependencyResolver = new FakeDependencyResolverLifeCycle();
            dependencyResolver.ReplaceSingletons<ILoadedForViewFetcher>(new TestLoadedForViewFetcher());
            RxApp.Initialize(dependencyResolver);
            RxLifeCycle.Initialize(dependencyResolver);
        }

        [StaFact]
        public void LoadingViewSmokeTest()
        {
            var vm = new ViewAwareViewModel();
            var fixture = new LoadingView();

            fixture.ViewModel = vm;
            Assert.Equal(0, vm.IsLoadedCount);
            Assert.Equal(0, fixture.IsLoadedCount);

            fixture.Loaded.OnNext(Unit.Default);
            Assert.Equal(1, vm.IsLoadedCount);
            Assert.Equal(1, fixture.IsLoadedCount);

            fixture.Unloaded.OnNext(Unit.Default);
            Assert.Equal(0, vm.IsLoadedCount);
            Assert.Equal(0, fixture.IsLoadedCount);

            fixture.Dispose();
        }

        [StaFact]
        public void NullingViewModelShouldUnloadIt()
        {
            var vm = new ViewAwareViewModel();
            var fixture = new LoadingView();

            fixture.ViewModel = vm;
            Assert.Equal(0, vm.IsLoadedCount);
            Assert.Equal(0, fixture.IsLoadedCount);

            fixture.Loaded.OnNext(Unit.Default);
            Assert.Equal(1, vm.IsLoadedCount);
            Assert.Equal(1, fixture.IsLoadedCount);

            fixture.ViewModel = null;
            Assert.Equal(0, vm.IsLoadedCount);

            fixture.Dispose();
        }

        [StaFact]
        public void SwitchingViewModelShouldUnloadIt()
        {
            var vm = new ViewAwareViewModel();
            var fixture = new LoadingView();

            fixture.ViewModel = vm;
            Assert.Equal(0, vm.IsLoadedCount);
            Assert.Equal(0, fixture.IsLoadedCount);

            fixture.Loaded.OnNext(Unit.Default);
            Assert.Equal(1, vm.IsLoadedCount);
            Assert.Equal(1, fixture.IsLoadedCount);

            var newVm = new ViewAwareViewModel();
            Assert.Equal(0, newVm.IsLoadedCount);

            fixture.ViewModel = newVm;
            Assert.Equal(0, vm.IsLoadedCount);
            Assert.Equal(1, newVm.IsLoadedCount);

            fixture.Dispose();
        }

        [StaFact]
        public void SettingViewModelAfterLoadedShouldLoadIt()
        {
            var vm = new ViewAwareViewModel();
            var fixture = new LoadingView();

            Assert.Equal(0, vm.IsLoadedCount);
            Assert.Equal(0, fixture.IsLoadedCount);

            fixture.Loaded.OnNext(Unit.Default);
            Assert.Equal(1, fixture.IsLoadedCount);

            fixture.ViewModel = vm;
            Assert.Equal(1, fixture.IsLoadedCount);
            Assert.Equal(1, vm.IsLoadedCount);

            fixture.Unloaded.OnNext(Unit.Default);
            Assert.Equal(0, fixture.IsLoadedCount);
            Assert.Equal(0, vm.IsLoadedCount);

            fixture.Dispose();
        }

        [StaFact]
        public void CanUnloadAndLoadViewAgain()
        {
            var vm = new ViewAwareViewModel();
            var fixture = new LoadingView();

            fixture.ViewModel = vm;
            Assert.Equal(0, vm.IsLoadedCount);
            Assert.Equal(0, fixture.IsLoadedCount);

            fixture.Loaded.OnNext(Unit.Default);
            Assert.Equal(1, vm.IsLoadedCount);
            Assert.Equal(1, fixture.IsLoadedCount);

            fixture.Unloaded.OnNext(Unit.Default);
            Assert.Equal(0, vm.IsLoadedCount);
            Assert.Equal(0, fixture.IsLoadedCount);

            fixture.Loaded.OnNext(Unit.Default);
            Assert.Equal(1, vm.IsLoadedCount);
            Assert.Equal(1, fixture.IsLoadedCount);

            fixture.Dispose();
        }
    }
}

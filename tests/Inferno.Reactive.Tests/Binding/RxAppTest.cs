using System.Diagnostics;
using System.Reactive.Concurrency;
using Xunit;

namespace Inferno.Reactive.Tests
{
    public class RxAppTest
    {
        public RxAppTest()
        {
            RxApp.Initialize(new FakeDependencyResolverReactive());
        }

        [Fact]
        public void SchedulerShouldBeCurrentThreadInTestRunner()
        {
            Debug.WriteLine(RxApp.MainThreadScheduler.GetType().FullName);
            Assert.Equal(CurrentThreadScheduler.Instance, RxApp.MainThreadScheduler);
        }
    }
}

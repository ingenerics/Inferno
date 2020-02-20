using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Inferno.Reactive.Tests
{
    public class AwaiterTest
    {
        public AwaiterTest()
        {
            RxApp.Initialize(new FakeDependencyResolverReactive());
        }

        [Fact]
        public void AwaiterSmokeTest()
        {
            var fixture = AwaitAnObservable();
            fixture.Wait();

            Assert.Equal(42, fixture.Result);
        }

        private async Task<int> AwaitAnObservable()
        {
            var o = Observable.Start(
                () =>
                {
                    Thread.Sleep(1000);
                    return 42;
                },
                RxApp.TaskpoolScheduler);

            var ret = await o;
            return ret;
        }
    }
}

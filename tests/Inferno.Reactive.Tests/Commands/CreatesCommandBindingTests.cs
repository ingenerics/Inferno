using Inferno.Testing;
using System.ComponentModel;
using System.Reactive.Linq;
using Xunit;

namespace Inferno.Reactive.Tests
{
    public class CreatesCommandBindingTests
    {
        public CreatesCommandBindingTests()
        {
            RxApp.Initialize(new FakeDependencyResolverReactive());
        }

        [Fact]
        public void EventBinderBindsToExplicitEvent()
        {
            var input = new TestFixture();
            var fixture = new CreatesCommandBindingViaEvent();
            var wasCalled = false;
            var cmd = ReactiveCommand.Create<int>(x => wasCalled = true);

            Assert.True(fixture.GetAffinityForObject(input.GetType(), true) > 0);
            Assert.False(fixture.GetAffinityForObject(input.GetType(), false) > 0);

            var disp = fixture.BindCommandToObject<PropertyChangedEventArgs>(cmd, input, Observable.Return((object)5), "PropertyChanged");
            input.IsNotNullString = "Foo";
            Assert.True(wasCalled);

            wasCalled = false;
            disp.Dispose();
            input.IsNotNullString = "Bar";
            Assert.False(wasCalled);
        }
    }
}
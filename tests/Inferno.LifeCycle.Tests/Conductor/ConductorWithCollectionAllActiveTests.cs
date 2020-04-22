using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Inferno.LifeCycle.Tests
{
    public class ConductorWithCollectionAllActiveTests
    {
        public ConductorWithCollectionAllActiveTests()
        {
            var dependencyResolver = new FakeDependencyResolverLifeCycle();
            RxApp.Initialize(dependencyResolver);
            RxLifeCycle.Initialize(dependencyResolver);
        }

        [Fact]
        public void AddedItemAppearsInChildren()
        {
            var conductor = new Conductor<IScreen>.Collection.AllActive();
            var conducted = new Screen();
            conductor.Items.Add(conducted);
            Assert.Contains(conducted, conductor.GetChildren());
        }

        [Fact]
        public async Task CanCloseIsTrueWhenAllItemsAreClosable()
        {
            var conductor = new Conductor<IScreen>.Collection.AllActive
            {
                CloseStrategy = new OneByOneCloseStrategy<IScreen>()
            };
            var conducted = new StateScreen
            {
                IsClosable = true
            };
            conductor.Items.Add(conducted);
            await ((IActivate)conductor).ActivateAsync(CancellationToken.None);
            var canClose = await conductor.CanCloseAsync(CancellationToken.None);

            Assert.True(canClose);
        }

        [Fact]
        public async Task CanCloseIsFalseWhenSomeItemsAreNotClosable()
        {
            var conductor = new Conductor<IScreen>.Collection.AllActive
            {
                CloseStrategy = new OneByOneCloseStrategy<IScreen>()
            };
            var conducted = new StateScreen
            {
                IsClosable = false
            };
            conductor.Items.Add(conducted);
            await ((IActivate)conductor).ActivateAsync(CancellationToken.None);
            var canClose = await conductor.CanCloseAsync(CancellationToken.None);

            Assert.False(canClose);
        }

        [Fact]
        public async Task OneByOneClosingStrategyClosesClosableItemsWhenCanCloseIsFalse()
        {
            // OneByOneClosingStrategy expected behavior:
            // - Close all items that can be closed, starting with the last item and moving back.
            // - Break on the first item that can't be closed and activate it (makes user aware).

            var conductor = new Conductor<IScreen>.Collection.AllActive
            {
                CloseStrategy = new OneByOneCloseStrategy<IScreen>()
            };

            var notClosable = new StateScreen
            {
                IsClosable = false
            };
            var closable = new StateScreen
            {
                IsClosable = true
            };

            // Last item is closable, the one before that is not, so only last item will be closed.
            conductor.Items.Add(notClosable);
            conductor.Items.Add(closable);

            await ((IActivate)conductor).ActivateAsync(CancellationToken.None);
            var canClose = await conductor.CanCloseAsync(CancellationToken.None);

            Assert.False(canClose);
            Assert.True(closable.IsClosed);
            Assert.False(closable.IsActive);
            Assert.False(notClosable.IsClosed);
            Assert.True(notClosable.IsActive);
        }

        [Fact]
        public async Task AllOrNoneClosingStrategyDoesNotCloseClosableItemsWhenCanCloseIsFalse()
        {
            // AllOrNoneCloseStrategy expected behavior:
            // - Close all items if all can be closed and none otherwise.
            // - ActiveItem is not changed when canClose is false.

            var conductor = new Conductor<IScreen>.Collection.AllActive
            {
                CloseStrategy = new AllOrNoneCloseStrategy<IScreen>()
            };

            var notClosable = new StateScreen
            {
                IsClosable = false
            };
            var closable = new StateScreen
            {
                IsClosable = true
            };

            // Last item is closable, the one before that is not, so no items will be closed.
            conductor.Items.Add(notClosable);
            conductor.Items.Add(closable);

            await ((IActivate)conductor).ActivateAsync(CancellationToken.None);
            var canClose = await conductor.CanCloseAsync(CancellationToken.None);

            Assert.False(canClose);
            Assert.False(closable.IsClosed);
            Assert.True(closable.IsActive);
            Assert.False(notClosable.IsClosed);
            Assert.True(notClosable.IsActive);
        }

        [Fact]
        public async Task ChildrenAreActivatedWhenConductorIsActivated()
        {
            var conductor = new Conductor<IScreen>.Collection.AllActive();
            var conducted = new Screen();
            conductor.Items.Add(conducted);
            await ((IActivate)conductor).ActivateAsync(CancellationToken.None);
            Assert.True(conducted.IsActive);
        }

        [Fact]
        public async Task ChildrenCanOnlyBeActiveWhenConductorIsActivated()
        {
            var conductor = new Conductor<IScreen>.Collection.AllActive();
            var conducted = new Screen();
            conductor.Items.Add(conducted);
            await conductor.ActivateItemAsync(conducted, CancellationToken.None);
            Assert.False(conducted.IsActive);
        }

        [Fact]
        public void ParentItemIsSetWhenAddingConductedItem()
        {
            var conductor = new Conductor<IScreen>.Collection.AllActive();
            var conducted = new Screen();
            conductor.Items.Add(conducted);
            Assert.Equal(conductor, conducted.Parent);
        }

        [Fact]
        public void ParentItemIsSetWhenReplacingConductedItem()
        {
            var conductor = new Conductor<IScreen>.Collection.AllActive();
            var originalConducted = new Screen();
            conductor.Items.Add(originalConducted);
            var newConducted = new Screen();
            conductor.Items[0] = newConducted;
            Assert.Equal(conductor, newConducted.Parent);
        }

        [Fact(Skip = "This is not possible as we don't get the removed items in the event handler.")]
        public void ParentItemIsUnsetOnClear()
        {
            var conductor = new Conductor<IScreen>.Collection.AllActive();
            var conducted = new Screen();
            conductor.Items.Add(conducted);
            conductor.Items.Clear();
            Assert.NotEqual(conductor, conducted.Parent);
        }

        [Fact]
        public void ParentItemIsUnsetWhenRemovingConductedItem()
        {
            var conductor = new Conductor<IScreen>.Collection.AllActive();
            var conducted = new Screen();
            conductor.Items.Add(conducted);
            conductor.Items.RemoveAt(0);
            Assert.NotEqual(conductor, conducted.Parent);
        }

        [Fact]
        public void ParentItemIsUnsetWhenReplacingConductedItem()
        {
            var conductor = new Conductor<IScreen>.Collection.AllActive();
            var conducted = new Screen();
            conductor.Items.Add(conducted);
            var conducted2 = new Screen();
            conductor.Items[0] = conducted2;
            Assert.NotEqual(conductor, conducted.Parent);
            Assert.Equal(conductor, conducted2.Parent);
        }

        #region TryClose

        [Fact]
        public async Task ScreenTryCloseAsyncWillAskConductorToCloseIt()
        {
            // conductor conducts conducted
            // conducted.TryCloseAsync() will ask conductor to close it

            var conductor = new Conductor<IScreen>.Collection.AllActive();
            var conducted1 = new StateScreen { IsClosable = true };
            var conducted2 = new StateScreen { IsClosable = true };

            // All items are closable
            conductor.Items.Add(conducted1);
            conductor.Items.Add(conducted2);

            await ((IActivate)conductor).ActivateAsync(CancellationToken.None);
            await conducted1.TryCloseAsync();

            Assert.True(conductor.IsActive);

            Assert.False(conducted1.IsActive);
            Assert.True(conducted1.IsClosed);

            Assert.True(conducted2.IsActive);
            Assert.False(conducted2.IsClosed);
        }

        [Fact]
        public async Task ConductorTryCloseAsyncWillAskItsConductorToCloseIt()
        {
            // nothing conducts conductor
            // conductor.TryCloseAsync() will do nothing

            var conductor = new Conductor<IScreen>.Collection.AllActive();
            var conducted1 = new StateScreen { IsClosable = true };
            var conducted2 = new StateScreen { IsClosable = true };

            // All items are closable
            conductor.Items.Add(conducted1);
            conductor.Items.Add(conducted2);

            await ((IActivate)conductor).ActivateAsync(CancellationToken.None);
            await conductor.TryCloseAsync();

            Assert.True(conductor.IsActive);
            Assert.True(conducted1.IsActive);
            Assert.False(conducted1.IsClosed);
        }

        [Fact]
        public async Task ConductorTryCloseAsyncWillAskItsConductorToCloseItAndItsChildren()
        {
            // conductor1 conducts conductor2
            // conductor2.TryCloseAsync() will ask conductor1 to close it

            var conductor1 = new Conductor<IScreen>();
            var conductor2 = new Conductor<IScreen>.Collection.AllActive();

            var conducted1 = new StateScreen { IsClosable = true };
            var conducted2 = new StateScreen { IsClosable = true };

            conductor1.ActiveItem = conductor2;
            // All items are closable
            conductor2.Items.Add(conducted1);
            conductor2.Items.Add(conducted2);

            await ((IActivate)conductor1).ActivateAsync(CancellationToken.None);
            await conductor2.TryCloseAsync();

            Assert.True(conductor1.IsActive);
            Assert.False(conductor2.IsActive);

            Assert.False(conducted1.IsActive);
            Assert.True(conducted1.IsClosed);
            Assert.False(conducted2.IsActive);
            Assert.True(conducted2.IsClosed);
        }

        #endregion TryClose
    }
}

using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Inferno.LifeCycle.Tests
{
    public class ConductorWithCollectionOneActiveTests
    {
        private class StateScreen : Screen
        {
            public bool IsClosed { get; private set; }
            public bool IsClosable { get; set; }

            public override Task<bool> CanCloseAsync(CancellationToken cancellationToken)
            {
                return Task.FromResult(IsClosable);
            }

            protected override async Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
            {
                await base.OnDeactivateAsync(close, cancellationToken);
                IsClosed = close;
            }
        }

        [Fact]
        public void AddedItemAppearsInChildren()
        {
            var conductor = new Conductor<IScreen>.Collection.OneActive();
            var conducted = new Screen();
            conductor.Items.Add(conducted);
            Assert.Contains(conducted, conductor.GetChildren());
        }

        [Fact]
        public async Task CanCloseIsTrueWhenAllItemsAreClosable()
        {
            var conductor = new Conductor<IScreen>.Collection.OneActive
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
            var conductor = new Conductor<IScreen>.Collection.OneActive
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

            var conductor = new Conductor<IScreen>.Collection.OneActive
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

            // ConductWith will allow conductor to activate (and initialize) the items as they are added.
            notClosable.ConductWith(conductor);
            closable.ConductWith(conductor);
            // Last item is closable, some other item is not, so last item will be closed.
            conductor.Items.Add(notClosable);
            conductor.Items.Add(closable);
            // Set ActiveItem so the conductor can change it.
            conductor.ActiveItem = closable;

            await ((IActivate)conductor).ActivateAsync(CancellationToken.None);
            var canClose = await conductor.CanCloseAsync(CancellationToken.None);

            Assert.False(canClose);
            Assert.True(closable.IsClosed);
            Assert.False(closable.IsActive);
            Assert.False(notClosable.IsClosed);
            Assert.True(notClosable.IsActive);

            // The first item that can't be closed is activated.
            Assert.Equal(notClosable, conductor.ActiveItem);
        }

        [Fact]
        public async Task AllOrNoneClosingStrategyDoesNotCloseClosableItemsWhenCanCloseIsFalse()
        {
            // AllOrNoneCloseStrategy expected behavior:
            // - Close all items if all can be closed and none otherwise.
            // - ActiveItem is not changed when canClose is false.

            var conductor = new Conductor<IScreen>.Collection.OneActive
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

            // ConductWith will allow conductor to activate (and initialize) the items as they are added.
            notClosable.ConductWith(conductor);
            closable.ConductWith(conductor);
            // Last item is closable, some other item is not, so last item will be closed.
            conductor.Items.Add(notClosable);
            conductor.Items.Add(closable);
            // Set ActiveItem so the conductor can change it.
            conductor.ActiveItem = closable;

            await ((IActivate)conductor).ActivateAsync(CancellationToken.None);
            var canClose = await conductor.CanCloseAsync(CancellationToken.None);

            Assert.False(canClose);
            Assert.False(closable.IsClosed);
            Assert.True(closable.IsActive);
            Assert.False(notClosable.IsClosed);
            Assert.True(notClosable.IsActive);

            // The last item remains activated.
            Assert.Equal(closable, conductor.ActiveItem);
        }

        [Fact]
        public async Task ChildrenAreNotActivatedJustBecauseConductorIsActivated()
        {
            var conductor = new Conductor<IScreen>.Collection.OneActive();
            var conducted = new Screen();
            conductor.Items.Add(conducted);
            await ((IActivate)conductor).ActivateAsync(CancellationToken.None);
            Assert.False(conducted.IsActive);
            Assert.NotEqual(conducted, conductor.ActiveItem);
            await conductor.ActivateItemAsync(conducted, CancellationToken.None);
            Assert.True(conducted.IsActive);
            Assert.Equal(conducted, conductor.ActiveItem);
        }

        [Fact]
        public async Task ChildrenCanOnlyBeActiveWhenConductorIsActive()
        {
            var conductor = new Conductor<IScreen>.Collection.OneActive();
            var conducted = new Screen();
            conductor.Items.Add(conducted);
            await conductor.ActivateItemAsync(conducted, CancellationToken.None);
            Assert.False(conducted.IsActive);
            Assert.Equal(conducted, conductor.ActiveItem);
        }

        [Fact]
        public void ParentItemIsSetOnAddedConductedItem()
        {
            var conductor = new Conductor<IScreen>.Collection.OneActive();
            var conducted = new Screen();
            conductor.Items.Add(conducted);
            Assert.Equal(conductor, conducted.Parent);
        }

        [Fact]
        public void ParentItemIsSetOnReplacedConductedItem()
        {
            var conductor = new Conductor<IScreen>.Collection.OneActive();
            var originalConducted = new Screen();
            conductor.Items.Add(originalConducted);
            var newConducted = new Screen();
            conductor.Items[0] = newConducted;
            Assert.Equal(conductor, newConducted.Parent);
        }

        [Fact(Skip = "This is not possible as we don't get the removed items in the event handler.")]
        public void ParentItemIsUnsetOnClear()
        {
            var conductor = new Conductor<IScreen>.Collection.OneActive();
            var conducted = new Screen();
            conductor.Items.Add(conducted);
            conductor.Items.Clear();
            Assert.NotEqual(conductor, conducted.Parent);
        }

        [Fact]
        public void ParentItemIsUnsetOnRemovedConductedItem()
        {
            var conductor = new Conductor<IScreen>.Collection.OneActive();
            var conducted = new Screen();
            conductor.Items.Add(conducted);
            conductor.Items.RemoveAt(0);
            Assert.NotEqual(conductor, conducted.Parent);
        }

        [Fact]
        public void ParentItemIsUnsetOnReplaceConductedItem()
        {
            var conductor = new Conductor<IScreen>.Collection.OneActive();
            var conducted = new Screen();
            conductor.Items.Add(conducted);
            var conducted2 = new Screen();
            conductor.Items[0] = conducted2;
            Assert.NotEqual(conductor, conducted.Parent);
            Assert.Equal(conductor, conducted2.Parent);
        }
    }
}

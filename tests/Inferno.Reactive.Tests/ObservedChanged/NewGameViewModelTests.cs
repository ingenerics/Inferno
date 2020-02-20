using System;
using System.Linq;
using Xunit;

namespace Inferno.Reactive.Tests
{
    public class NewGameViewModelTests
    {
        private readonly NewGameViewModel _viewmodel;

        public NewGameViewModelTests()
        {
            RxApp.Initialize(new FakeDependencyResolverReactive());

            _viewmodel = new NewGameViewModel();
        }

        [Fact]
        public void CanAddUpToSevenPlayers()
        {
            foreach (var i in Enumerable.Range(1, 7))
            {
                _viewmodel.NewPlayerName = "Player" + i;
                _viewmodel.AddPlayer.Execute().Subscribe();
                Assert.Equal(i, _viewmodel.Players.Count);
            }
        }
    }
}

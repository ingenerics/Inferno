using System.Reactive;

namespace Inferno.Reactive.Tests
{
    public class ReactiveCommandHolder : ReactiveObject
    {
        private ReactiveCommand<int, Unit> _theCommand;

        public ReactiveCommand<int, Unit> TheCommand
        {
            get => _theCommand;
            set => this.RaiseAndSetIfChanged(ref _theCommand, value);
        }
    }
}

using System.Windows.Input;

namespace Inferno.Reactive.Tests
{
    public class ICommandHolder : ReactiveObject
    {
        private ICommand _theCommand;

        public ICommand TheCommand
        {
            get => _theCommand;
            set => this.RaiseAndSetIfChanged(ref _theCommand, value);
        }
    }
}
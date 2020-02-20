using System.Reactive;
using System.Reactive.Concurrency;

namespace Inferno.Reactive.Tests
{
    public class CommandBindingViewModel : ReactiveObject
    {
        private ReactiveCommand<int, Unit> _Command1;
        private ReactiveCommand<Unit, Unit> _Command2;

        private int _value;

        public CommandBindingViewModel()
        {
            Command1 = ReactiveCommand.Create<int, Unit>(_ => Unit.Default, outputScheduler: ImmediateScheduler.Instance);
            Command2 = ReactiveCommand.Create(() => { }, outputScheduler: ImmediateScheduler.Instance);
        }

        public ReactiveCommand<int, Unit> Command1
        {
            get => _Command1;
            set => this.RaiseAndSetIfChanged(ref _Command1, value);
        }

        public ReactiveCommand<Unit, Unit> Command2
        {
            get => _Command2;
            set => this.RaiseAndSetIfChanged(ref _Command2, value);
        }

        public FakeNestedViewModel NestedViewModel { get; set; }

        public int Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }
    }
}
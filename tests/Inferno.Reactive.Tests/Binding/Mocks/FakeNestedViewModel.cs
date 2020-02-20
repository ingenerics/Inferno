using System.Reactive;

namespace Inferno.Reactive.Tests
{
    public class FakeNestedViewModel : ReactiveObject
    {
        public FakeNestedViewModel()
        {
            NestedCommand = ReactiveCommand.Create(() => { });
        }

        public ReactiveCommand<Unit, Unit> NestedCommand { get; protected set; }
    }
}

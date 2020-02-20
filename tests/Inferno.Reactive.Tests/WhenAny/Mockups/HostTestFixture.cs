using Inferno.Testing;

namespace Inferno.Reactive.Tests
{
    public class HostTestFixture : ReactiveObject
    {
        private TestFixture _Child;

        private NonObservableTestFixture _PocoChild;

        private int _SomeOtherParam;

        public TestFixture Child
        {
            get => _Child;
            set => this.RaiseAndSetIfChanged(ref _Child, value);
        }

        public NonObservableTestFixture PocoChild
        {
            get => _PocoChild;
            set => this.RaiseAndSetIfChanged(ref _PocoChild, value);
        }

        public int SomeOtherParam
        {
            get => _SomeOtherParam;
            set => this.RaiseAndSetIfChanged(ref _SomeOtherParam, value);
        }
    }
}

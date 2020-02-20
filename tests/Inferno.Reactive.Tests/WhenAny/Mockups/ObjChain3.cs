namespace Inferno.Reactive.Tests
{
    public class ObjChain3 : ReactiveObject
    {
        private HostTestFixture _model = new HostTestFixture();

        public HostTestFixture Model
        {
            get => _model;
            set => this.RaiseAndSetIfChanged(ref _model, value);
        }
    }
}

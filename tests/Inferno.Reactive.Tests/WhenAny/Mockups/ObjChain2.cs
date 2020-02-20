namespace Inferno.Reactive.Tests
{
    public class ObjChain2 : ReactiveObject
    {
        private ObjChain3 _model = new ObjChain3();

        public ObjChain3 Model
        {
            get => _model;
            set => this.RaiseAndSetIfChanged(ref _model, value);
        }
    }
}

namespace Inferno.Reactive.Tests
{
    public class ObjChain1 : ReactiveObject
    {
        private ObjChain2 _model = new ObjChain2();

        public ObjChain2 Model
        {
            get => _model;
            set => this.RaiseAndSetIfChanged(ref _model, value);
        }
    }
}

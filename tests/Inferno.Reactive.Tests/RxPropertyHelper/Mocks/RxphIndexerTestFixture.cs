namespace Inferno.Reactive.Tests
{
    internal class RxphIndexerTestFixture : ReactiveObject
    {
        private string _text;

        public RxphIndexerTestFixture()
        {
            var temp = this.WhenAnyValue(f => f.Text)
                           .ToProperty(this, f => f["Whatever"])
                           .Value;
        }

        public string Text
        {
            get => _text;
            set => this.RaiseAndSetIfChanged(ref _text, value);
        }

        public string this[string propertyName] => string.Empty;
    }
}

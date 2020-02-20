using System.ComponentModel;
using Inferno.Testing;

namespace Inferno.Reactive.Tests
{
    public class NonReactiveINPCObject : INotifyPropertyChanged
    {
        private TestFixture _inpcProperty;

        public event PropertyChangedEventHandler PropertyChanged;

        public TestFixture InpcProperty
        {
            get => _inpcProperty;
            set
            {
                if (_inpcProperty == value)
                {
                    return;
                }

                _inpcProperty = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InpcProperty)));
            }
        }
    }
}
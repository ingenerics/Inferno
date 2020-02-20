using System;
using System.Windows.Input;

namespace Inferno.Reactive.Tests
{
    public class FakeCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public object CanExecuteParameter { get; private set; }

        public object ExecuteParameter { get; private set; }

        public bool CanExecute(object parameter)
        {
            CanExecuteParameter = parameter;
            return true;
        }

        public void Execute(object parameter)
        {
            ExecuteParameter = parameter;
        }

        protected virtual void NotifyCanExecuteChanged(EventArgs e)
        {
            CanExecuteChanged?.Invoke(this, e);
        }
    }
}

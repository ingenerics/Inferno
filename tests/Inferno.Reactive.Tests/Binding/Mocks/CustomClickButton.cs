using System;
using System.Windows.Controls;

namespace Inferno.Reactive.Tests
{
    public class CustomClickButton : Button
    {
        public event EventHandler<EventArgs> CustomClick;

        public void RaiseCustomClick() =>
            CustomClick?.Invoke(this, EventArgs.Empty);
    }
}

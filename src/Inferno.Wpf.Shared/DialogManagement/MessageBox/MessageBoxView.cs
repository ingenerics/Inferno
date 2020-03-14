using System.Windows;
using Inferno.Core;
using System.Windows.Controls;
using System.Windows.Data;

namespace Inferno.DialogManagement.MessageBox
{
    // Wrapper to let view resolution map MessageBoxViewModel to MessageBoxView
    public class MessageBoxView : Label
    {
        public MessageBoxView()
        {
            SetBinding(Label.ContentProperty, new Binding(nameof(IHaveDisplayName.DisplayName)));
            SetValue(Label.VerticalAlignmentProperty, VerticalAlignment.Center);
        }
    }
}

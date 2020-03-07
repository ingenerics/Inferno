using System.Windows;

namespace Inferno
{
    public static class WindowCloser
    {
        public static readonly DependencyProperty RequestCloseProperty =
            DependencyProperty.RegisterAttached(
                nameof(Window.DialogResult),
                typeof(bool),
                typeof(WindowCloser),
                new PropertyMetadata(RequestCloseChanged));

        private static void RequestCloseChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is Window window && (bool)e.NewValue)
            {
                window.Close();
            }
        }

        public static void SetRequestClose(Window target, bool value)
        {
            target.SetValue(RequestCloseProperty, value);
        }
    }
}

using System.Windows;
using System.Windows.Controls;

namespace Inferno
{
    public class CompositionControl : ContentControl
    {
        public object ViewModel
        {
            get { return (object)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(object), typeof(CompositionControl), new PropertyMetadata(null, OnViewModelChanged));

        private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var content = d as ContentControl;
            var viewModel = e.NewValue;
            if (content == null || viewModel == null) return;
            View.SetModel(content, viewModel);
            if (viewModel is IScreen screen && content.DataContext is IConductor conductor)
                screen.ConductWith(conductor as IActivate);
        }
    }
}

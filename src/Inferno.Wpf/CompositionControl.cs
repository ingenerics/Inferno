using System.Windows;
using System.Windows.Controls;

namespace Inferno
{
    public class CompositionControl : ContentControl
    {
        #region ViewModel

        public object ViewModel
        {
            get { return (object)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(object), typeof(CompositionControl), new PropertyMetadata(null, OnViewModelChanged));

        private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = e.NewValue;
            if (d is ContentControl content)
            {
                View.SetModel(content, viewModel);
            }
        }

        #endregion ViewModel

        #region Context

        public object Context
        {
            get { return (object)GetValue(ContextProperty); }
            set { SetValue(ContextProperty, value); }
        }

        public static readonly DependencyProperty ContextProperty =
            DependencyProperty.Register("Context", typeof(object), typeof(CompositionControl), new PropertyMetadata(null, OnContextChanged));

        private static void OnContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var context = e.NewValue;
            if (d is ContentControl content)
            {
                View.SetContext(content, context);
            }
        }

        #endregion Context
    }
}
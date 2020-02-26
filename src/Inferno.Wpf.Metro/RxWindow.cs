using Inferno.Core;
using System.Windows;
using MahApps.Metro.Controls;

namespace Inferno
{
    public class RxWindow<TViewModel> :
        MetroWindow, IViewFor<TViewModel>
        where TViewModel : class
    {
        /// <summary>
        /// The view model dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(
                nameof(ViewModel),
                typeof(TViewModel),
                typeof(RxWindow<TViewModel>),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets the binding root view model.
        /// </summary>
        public TViewModel BindingRoot => ViewModel;

        public TViewModel ViewModel
        {
            get => (TViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TViewModel)value;
        }
    }
}

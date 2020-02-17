using System.Windows;

namespace Inferno.Core
{
    public interface IViewModelBinder
    {
        void Bind(object viewModel, DependencyObject view);
    }
}

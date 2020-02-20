using System.Windows;

namespace Inferno
{
    public interface IViewModelBinder
    {
        void Bind(object viewModel, DependencyObject view);
    }
}

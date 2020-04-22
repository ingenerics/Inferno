using Inferno.Core;
using Inferno.Core.Logging;
using System.Windows;

namespace Inferno
{
    public class ViewModelBinder : IViewModelBinder
    {
        protected readonly ILogger Logger;

        public ViewModelBinder(ILogger logger)
        {
            Logger = logger;
        }

        /// <summary>
        /// Binds the specified viewModel to the view.
        /// </summary>
        public void Bind(object viewModel, DependencyObject view)
        {
            Logger.LogInformation(this, $"Setting {viewModel} as DataContext of {view}");

            ((FrameworkElement)view).DataContext = viewModel;

            if (view is IViewFor rxView)
            {
                rxView.ViewModel = viewModel;
            }
        }
    }
}

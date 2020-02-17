using System.Windows;
using Inferno.Core.Logging;

namespace Inferno.Core
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
            Logger.LogInformation($"Binding {view} and {viewModel}.");

            ((FrameworkElement)view).DataContext = viewModel;

            if (view is IViewFor rxView)
            {
                rxView.ViewModel = viewModel;
            }
        }
    }
}

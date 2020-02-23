using System;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;
using Inferno.Core;

namespace Inferno
{
    /*
     * The Loaded event is raised as a coordinated effort throughout the entire element tree (specifically, the logical tree).
     * When all elements in the tree are in a state where they are considered loaded, the Loaded event is first raised on the root element.
     * The Loaded event is then raised successively on each child element.
     * Ref MSDN https://docs.microsoft.com/en-us/dotnet/framework/wpf/advanced/object-lifetime-events
     */

    /// <summary>
    /// SinkForActivatedViewFetcher is how Inferno determines which lifecycle state a view, backed by an IActivate view model, is in.
    /// We typically want to defer binding until view model activation is complete AND the view is loaded (ie when it was added to the visual tree).
    /// If we bind before view model activation is complete, the activation would trigger notifications and work on the UIThread that could otherwise be avoided.
    /// </summary>
    public class SinkForActivatedViewFetcher : ISinkForViewFetcher
    {
        private readonly ISinkForViewFetcher _sinkForFrameworkElementFetcher;

        public SinkForActivatedViewFetcher(ISinkForViewFetcher sinkForFrameworkElementFetcher)
        {
            _sinkForFrameworkElementFetcher = sinkForFrameworkElementFetcher;
        }

        /// <inheritdoc/>
        public int GetAffinityForView(Type view)
        {
            return typeof(IViewFor).GetTypeInfo().IsAssignableFrom(view.GetTypeInfo()) ? 20 : 0;
        }

        /// <inheritdoc/>
        public IObservable<bool> GetSinkForView(object view)
        {
            var frameworkElement = view as FrameworkElement;
            var activatable = (view as IViewFor)?.ViewModel as IActivate;

            if (frameworkElement != null && activatable != null)
            {
                return
                    Observable
                        .CombineLatest(
                            activatable.Activator.IsActive,
                            _sinkForFrameworkElementFetcher.GetSinkForView(view),
                            (isActive, isLoaded) => isActive && isLoaded)
                        .DistinctUntilChanged();
            }
            else if (frameworkElement == null && activatable == null)
            {
                return Observable<bool>.Empty;
            }
            else if (frameworkElement == null)
            {
                return activatable.Activator.IsActive;
            }
            else
            {
                return _sinkForFrameworkElementFetcher.GetSinkForView(view);
            }
        }
    }
}

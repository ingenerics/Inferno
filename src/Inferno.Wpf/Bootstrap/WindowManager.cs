using Inferno.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Inferno
{
    /// <summary>
    /// A service that manages windows.
    /// </summary>
    public class WindowManager : IWindowManager
    {
        protected readonly Application Application;
        protected readonly IViewLocator ViewLocator;
        protected readonly IViewModelBinder ViewModelBinder;

        public WindowManager(Application application, IViewLocator viewLocator, IViewModelBinder viewModelBinder)
        {
            Application = application;
            ViewLocator = viewLocator;
            ViewModelBinder = viewModelBinder;
        }

        /// <summary>
        /// Shows a modal dialog for the specified model.
        /// </summary>
        /// <param name="rootModel">The root model.</param>
        /// <param name="context">The context.</param>
        /// <param name="settings">The dialog popup settings.</param>
        /// <returns>The dialog result.</returns>
        public virtual async Task<bool?> ShowDialogAsync(object rootModel, object context = null, IDictionary<string, object> settings = null)
        {
            var window = await CreateWindowAsync(rootModel, true, context, settings);

            return window.ShowDialog();
        }

        /// <summary>
        /// Shows a window for the specified model.
        /// </summary>
        /// <param name="rootModel">The root model.</param>
        /// <param name="context">The context.</param>
        /// <param name="settings">The optional window settings.</param>
        public virtual async Task ShowWindowAsync(object rootModel, object context = null, IDictionary<string, object> settings = null)
        {
            var window = await CreateWindowAsync(rootModel, false, context, settings);

            window.Show();
        }

        /// <summary>
        /// Creates a window.
        /// </summary>
        /// <param name="rootModel">The view model.</param>
        /// <param name="isDialog">Whethor or not the window is being shown as a dialog.</param>
        /// <param name="context">The view context.</param>
        /// <param name="settings">The optional popup settings.</param>
        /// <returns>The window.</returns>
        protected virtual async Task<Window> CreateWindowAsync(object rootModel, bool isDialog, object context, IDictionary<string, object> settings)
        {
            var view = EnsureWindow(rootModel, ViewLocator.LocateForModel(rootModel, context), isDialog);
            ViewModelBinder.Bind(rootModel, view);

            if (string.IsNullOrEmpty(view.Title) && rootModel is IHaveDisplayName)
            {
                var binding = new Binding(nameof(IHaveDisplayName.DisplayName)) { Mode = BindingMode.TwoWay };
                view.SetBinding(Window.TitleProperty, binding);
            }

            ApplySettings(view, settings);

            var conductor = new WindowConductor(rootModel, view);

            await conductor.InitializeAsync();

            return view;
        }

        /// <summary>
        /// Makes sure the view is a window or is wrapped by one.
        /// </summary>
        /// <param name="model">The view model.</param>
        /// <param name="view">The view.</param>
        /// <param name="isDialog">Whethor or not the window is being shown as a dialog.</param>
        /// <returns>The window.</returns>
        protected virtual Window EnsureWindow(object model, object view, bool isDialog)
        {
            if (view is Window window)
            {
                var owner = InferOwnerOf(window);
                if (owner != null && isDialog)
                {
                    window.Owner = owner;
                }
            }
            else
            {
                window = new Window
                {
                    Content = view,
                    SizeToContent = SizeToContent.WidthAndHeight
                };

                var owner = InferOwnerOf(window);
                if (owner != null)
                {
                    window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    window.Owner = owner;
                }
                else
                {
                    window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                }
            }

            return window;
        }

        /// <summary>
        /// Infers the owner of the window.
        /// </summary>
        /// <param name="window">The window to whose owner needs to be determined.</param>
        /// <returns>The owner.</returns>
        protected virtual Window InferOwnerOf(Window window)
        {
            if (Application == null)
            {
                return null;
            }

            var active = Application.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive);
            active = active ?? (PresentationSource.FromVisual(Application.MainWindow) == null ? null : Application.MainWindow);
            return active == window ? null : active;
        }

        private bool ApplySettings(object target, IEnumerable<KeyValuePair<string, object>> settings)
        {
            if (settings != null)
            {
                var type = target.GetType();

                foreach (var pair in settings)
                {
                    var propertyInfo = type.GetProperty(pair.Key);

                    if (propertyInfo != null)
                    {
                        propertyInfo.SetValue(target, pair.Value, null);
                    }
                }

                return true;
            }

            return false;
        }

        private class WindowConductor
        {
            private readonly Window _view;
            private readonly object _model;

            private bool _deactivatingFromView;
            private bool _deactivateFromViewModel;
            private bool _actuallyClosing;

            public WindowConductor(object model, Window view)
            {
                this._model = model;
                this._view = view;
            }

            public async Task InitializeAsync()
            {
                if (_model is IActivate activatable)
                {
                    await activatable.ActivateAsync(CancellationToken.None);
                    _view.Closed += Closed;
                    activatable.Activator.AddDisposable(
                        activatable.Activator.Deactivated
                        .Select(Deactivated)
                        .Subscribe());
                }

                if (_model is IGuardClose guard)
                {
                    _view.Closing += Closing;
                }
            }

            private async void Closed(object sender, EventArgs e)
            {
                _view.Closed -= Closed;
                _view.Closing -= Closing;

                if (_deactivateFromViewModel)
                {
                    return;
                }

                var activatable = (IActivate)_model;

                _deactivatingFromView = true;
                await activatable.DeactivateAsync(true, CancellationToken.None);
                _deactivatingFromView = false;
            }

            private Unit Deactivated(bool wasClosed)
            {
                if (!wasClosed)
                {
                    return Unit.Default;
                }

                if (_deactivatingFromView)
                {
                    return Unit.Default;
                }

                _deactivateFromViewModel = true;
                _actuallyClosing = true;
                _view.Close();
                _actuallyClosing = false;
                _deactivateFromViewModel = false;

                return Unit.Default;
            }

            private async void Closing(object sender, CancelEventArgs e)
            {
                if (e.Cancel)
                {
                    return;
                }

                var guard = (IGuardClose)_model;

                if (_actuallyClosing)
                {
                    _actuallyClosing = false;
                    return;
                }

                var cachedDialogResult = _view.DialogResult;

                e.Cancel = true;

                await Task.Yield();

                var canClose = await guard.CanCloseAsync(CancellationToken.None);

                if (!canClose)
                    return;

                _actuallyClosing = true;

                if (cachedDialogResult == null)
                {
                    _view.Close();
                }
                else if (_view.DialogResult != cachedDialogResult)
                {
                    _view.DialogResult = cachedDialogResult;
                }
            }
        }
    }
}
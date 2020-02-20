using Inferno.Core;
using Inferno.Core.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Inferno
{
    /// <summary>
    /// Inherit from this class in order to customize the configuration of the framework.
    /// </summary>
    public abstract class BootstrapperBase
    {
        private bool _isInitialized;

        protected readonly Application _application;

        /// <summary>
        /// Creates an instance of the bootstrapper.
        /// </summary>
        protected BootstrapperBase(Application application)
        {
            _application = application;
        }

        protected IDependencyResolver DependencyResolver { get; private set; }

        /// <summary>
        /// Initialize the framework.
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            _isInitialized = true;

            StartRuntime();
        }

        /// <summary>
        /// Called by the bootstrapper's constructor at runtime to start the framework.
        /// </summary>
        protected virtual void StartRuntime()
        {
            PrepareApplication();

            DependencyResolver = Configure();

            AttachProperties(DependencyResolver);
        }

        /// <summary>
        /// Provides an opportunity to hook into the application object.
        /// </summary>
        protected virtual void PrepareApplication()
        {
            _application.Startup += OnStartup;

            _application.DispatcherUnhandledException += OnUnhandledException;

            _application.Exit += OnExit;
        }

        /// <summary>
        /// Override to configure the framework and setup your IoC container.
        /// </summary>
        protected virtual IDependencyResolver Configure()
            => new DefaultDependencyResolver();

        /// <summary>
        /// Hydrate Attached Property helpers.
        /// </summary>
        /// <param name="dependencyResolver"></param>
        protected virtual void AttachProperties(IDependencyResolver dependencyResolver)
        {
            View.Logger = (ILogger)dependencyResolver.GetInstance<ILogger>();
            View.ViewLocator = (IViewLocator)dependencyResolver.GetInstance<IViewLocator>();
            View.ViewModelBinder = (IViewModelBinder)dependencyResolver.GetInstance<IViewModelBinder>();
        }

        /// <summary>
        /// Override to tell the framework where to find assemblies to inspect for views, etc.
        /// </summary>
        /// <returns>A list of assemblies to inspect.</returns>
        protected virtual AssemblySource SelectAssemblies()
        {
            return new AssemblySource(GetType().Assembly);
        }

        /// <summary>
        /// Override this to add custom behavior to execute after the application starts.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The args.</param>
        protected virtual void OnStartup(object sender, StartupEventArgs e)
        {
        }

        /// <summary>
        /// Override this to add custom behavior on exit.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        protected virtual void OnExit(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Override this to add custom behavior for unhandled exceptions.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        protected virtual void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
        }

        /// <summary>
        /// Locates the view model, locates the associate view, binds them and shows it as the root view.
        /// </summary>
        /// <param name="viewModelType">The view model type.</param>
        /// <param name="settings">The optional window settings.</param>
        protected async Task DisplayRootViewForAsync(Type viewModelType, IDictionary<string, object> settings = null)
        {
            var windowManager = DependencyResolver.GetInstance<IWindowManager>();
            await windowManager.ShowWindowAsync(DependencyResolver.GetInstance(viewModelType), null, settings);
        }

        /// <summary>
        /// Locates the view model, locates the associate view, binds them and shows it as the root view.
        /// </summary>
        /// <typeparam name="TViewModel">The view model type.</typeparam>
        /// <param name="settings">The optional window settings.</param>
        protected Task DisplayRootViewFor<TViewModel>(IDictionary<string, object> settings = null)
        {
            return DisplayRootViewForAsync(typeof(TViewModel), settings);
        }
    }
}
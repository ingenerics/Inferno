﻿using SimpleInjector;
using System.Windows;
using Inferno.Core;
using Inferno.Core.Logging;

namespace ScoreBoard.Bootstrap
{
    public class AppBootstrapper : BootstrapperBase
    {
        public AppBootstrapper(Application application) : base(application)
        {
            Initialize();
        }

        protected override IDependencyResolver Configure()
        {
            // 1. Create a new Simple Injector container
            var container = new Container();

            // 2. A wrapper that adheres to Inferno's internal contract (SimpleInjector adapter).
            // -> There are no direct references to SimpleInjector classes outside of this method.
            var dependencyResolver = new SimpleInjectorDependencyResolver(container);

            // 3. Configure the container
            //    Register app components
            container.RegisterInstance(SelectAssemblies());
            container.Register<ILogger, DebugLogger>(Lifestyle.Singleton);
            container.Register<IViewLocator, ViewLocator>(Lifestyle.Singleton);
            container.Register<IViewModelBinder, ViewModelBinder>(Lifestyle.Singleton);
            container.RegisterInstance<IDependencyResolver>(dependencyResolver); // Used by IViewLocator to resolve views
            //    Components that are not covered by unit tests
            container.RegisterInstance(_application);
            container.Register<IWindowManager, WindowManager>(Lifestyle.Singleton);

            // 4. Initialize reactive components

            // 5. Verify your configuration (optional)
            container.Verify();

            return dependencyResolver;
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }

        /// <summary>
        /// Override to tell the framework where to find assemblies to inspect for views.
        /// </summary>
        /// <returns>A list of assemblies to inspect.</returns>
        protected override AssemblySource SelectAssemblies()
        {
            return new AssemblySource(typeof(ShellView).Assembly);
        }
    }
}
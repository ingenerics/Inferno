using Inferno;
using Inferno.Core;
using Inferno.Core.Logging;
using SimpleInjector;
using System.Reactive.Concurrency;
using System.Windows;
using WorldCup.Repo;

namespace WorldCup.Bootstrap
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
            var logger = new ItemsControlLogger();

            container.RegisterInstance(SelectAssemblies());
            container.RegisterInstance<ILogger>(logger);
            container.RegisterInstance<IItemsControlLogger>(logger);
            container.Register<IViewLocator, ViewLocator>(Lifestyle.Singleton);
            container.Register<IViewModelBinder, ViewModelBinder>(Lifestyle.Singleton);
            container.Register<ICommandBinderImplementation, CommandBinderImplementation>(Lifestyle.Singleton);
            container.Collection.Register<ICreatesObservableForProperty>(new INPCObservableForProperty(), new IROObservableForProperty(), new POCOObservableForProperty(logger), new DependencyObjectObservableForProperty(logger));
            container.Collection.Register<ICreatesCommandBinding>(new CreatesCommandBindingViaEvent(), new CreatesCommandBindingViaCommandParameter());
            container.Collection.Register<IBindingTypeConverter>(new EqualityTypeConverter(logger), new StringConverter(), new ComponentModelTypeConverter(), new BooleanToVisibilityTypeConverter());
            container.Collection.Register<ISetMethodBindingConverter>(new NullSetMethodBindingConverter());
            container.Collection.Register<IPropertyBindingHook>(new NullObjectBindingHook());
            container.Collection.Register<ILoadedForViewFetcher>(new LoadedForViewFetcher());
            container.RegisterInstance<IDependencyResolver>(dependencyResolver); // Used by IViewLocator to resolve views
            //    Components that are not covered by unit tests
            container.RegisterInstance(_application);
            container.Register<IWindowManager, WindowManager>(Lifestyle.Singleton);
            container.Register<IDialogManager, DialogManager>(Lifestyle.Singleton);
            container.Register<IWorldCupRepo, WorldCupRepo>(Lifestyle.Singleton);

            // 4. Verify your configuration (optional)
            container.Verify();

            return dependencyResolver;
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            RxApp.Initialize(DependencyResolver, DispatcherScheduler.Current);
            RxLifeCycle.Initialize(DependencyResolver);

            DisplayRootViewFor<ShellViewModel>();
        }

        /// <summary>
        /// Override to tell the framework where to find assemblies to inspect for views.
        /// </summary>
        /// <returns>A list of assemblies to inspect.</returns>
        protected override AssemblySource SelectAssemblies()
        {
            return new AssemblySource(typeof(IDialogManager).Assembly, typeof(ShellView).Assembly);
        }
    }
}

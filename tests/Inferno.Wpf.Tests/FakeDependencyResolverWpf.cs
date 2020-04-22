using Inferno.Core;
using Inferno.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Inferno.Wpf.Tests
{
    public class FakeDependencyResolverWpf : IDependencyResolver
    {
        private readonly ILogger _logger;
        private readonly Dictionary<Type, object> _container;

        public FakeDependencyResolverWpf()
        {
            _logger = new DebugLogger();
            _container = new Dictionary<Type, object>();

            var callingAssembly = Assembly.GetCallingAssembly();
            var assemblySource = new AssemblySource(callingAssembly);
            _container.Add(typeof(ILogger), _logger);
            _container.Add(typeof(IViewLocator), new ViewLocator(this, assemblySource, _logger));
            _container.Add(typeof(IViewModelBinder), new ViewModelBinder(_logger));

            InitializeReactiveComponents();
        }

        private void InitializeReactiveComponents()
        {
            RegisterSingleton<ICommandBinderImplementation>(new CommandBinderImplementation());
            RegisterSingletons<ICreatesObservableForProperty>(new INPCObservableForProperty(), new IROObservableForProperty(), new POCOObservableForProperty(_logger), new DependencyObjectObservableForProperty(_logger));
            RegisterSingletons<ICreatesCommandBinding>(new CreatesCommandBindingViaEvent(), new CreatesCommandBindingViaCommandParameter());
            RegisterSingletons<IBindingTypeConverter>(new EqualityTypeConverter(_logger), new StringConverter(), new ComponentModelTypeConverter(), new BooleanToVisibilityTypeConverter());
            RegisterSingletons<ISetMethodBindingConverter>(new NullSetMethodBindingConverter());
            RegisterSingletons<IPropertyBindingHook>(new NullObjectBindingHook());
            RegisterSingletons<ILoadedForViewFetcher>(new LoadedForViewFetcher());
        }

        public void RegisterSingleton<TService>(TService instance)
            => _container.Add(typeof(TService), instance);

        public void RegisterSingletons<TService>(params TService[] instances)
            => _container.Add(typeof(TService), instances.Cast<object>());

        public void RegisterTransient<TService>(Func<TService> instanceFactory)
            => _container.Add(typeof(TService), instanceFactory);

        public void ReplaceSingleton<TService>(TService instance)
        {
            if (_container.ContainsKey(typeof(TService)))
            {
                _container.Remove(typeof(TService));
                RegisterSingleton(instance);
            }
            else
            {
                throw new Exception($"Type {typeof(TService)} is not registered in the fake dependency resolver");
            }
        }

        public void ReplaceSingletons<TService>(params TService[] instances)
        {
            if (_container.ContainsKey(typeof(TService)))
            {
                _container.Remove(typeof(TService));
                RegisterSingletons(instances);
            }
            else
            {
                throw new Exception($"Type {typeof(TService)} is either not registered in the fake dependency resolver, or the implementation is not IEnumerable");
            }
        }

        public TService GetInstance<TService>() where TService : class
        {
            if (_container.TryGetValue(typeof(TService), out var obj))
            {
                return obj is Func<TService> f ? f() : (TService)obj;
            }
            else
            {
                throw new Exception($"Type {typeof(TService)} is not registered in the fake dependency resolver");
            }
        }

        public object GetInstance(Type service)
        {
            MethodInfo method =
                typeof(FakeDependencyResolverWpf)
                .GetMethod(nameof(GetInstance), new Type[0])
                .MakeGenericMethod(service);

            return method.Invoke(this, null);
        }

        public bool TryGetInstance(Type serviceType, out object instance)
            => _container.TryGetValue(serviceType, out instance);

        public bool TryGetInstance<TService>(out TService instance) where TService : class
        {
            var success = TryGetInstance(typeof(TService), out var temp);
            instance = temp as TService;

            return success;
        }

        // For the time being, no transient collections are allowed/used.
        // => Generic method forwards to method with type passed as argument.

        public IEnumerable<TService> GetAllInstances<TService>() where TService : class
            => GetAllInstances(typeof(TService)).Cast<TService>();

        public IEnumerable<object> GetAllInstances(Type service)
        {
            if (_container.TryGetValue(service, out var obj) && obj is IEnumerable<object> instances)
            {
                return instances;
            }
            else
            {
                throw new Exception($"Type {service} is either not registered in the fake dependency resolver, or the implementation is not IEnumerable");
            }
        }

        // NOTE UNUSED BECAUSE OF REFLECTION PERFORMANCE PENALTY
        // For future ref when transient collections are needed
        // => Method with type passed as argument forwards to generic method.

        //public IEnumerable<TService> GetAllInstances<TService>() where TService : class
        //{
        //    _container.TryGetValue(typeof(TService), out var obj);

        //    if (obj is Func<IEnumerable<TService>> f)
        //    {
        //        return f();
        //    }
        //    else if (obj is IEnumerable<TService> instances)
        //    {
        //        return instances;
        //    }
        //    else
        //    {
        //        throw new Exception($"Type {typeof(TService)} is either not registered in the fake dependecy resolver, or the implementation is not IEnumerable");
        //    }
        //}

        //public IEnumerable<object> GetAllInstances(Type service)
        //{
        //    MethodInfo method =
        //        typeof(FakeDependencyResolver)
        //        .GetMethod(nameof(GetAllInstances), new Type[0])
        //        .MakeGenericMethod(service);

        //    var obj = method.Invoke(this, null);

        //    return ((System.Collections.IEnumerable)obj).Cast<object>();
        //}
    }
}

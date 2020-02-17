using Inferno.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Inferno.Core.Tests
{
    public class FakeDependencyResolverCore : IDependencyResolver
    {
        private readonly Dictionary<Type, object> _container;

        public FakeDependencyResolverCore()
        {
            _container = new Dictionary<Type, object>();

            var logger = new DebugLogger();
            var callingAssembly = Assembly.GetCallingAssembly();
            var assemblySource = new AssemblySource(callingAssembly);
            _container.Add(typeof(ILogger), logger);
            _container.Add(typeof(IViewLocator), new ViewLocator(this, assemblySource, logger));
            _container.Add(typeof(IViewModelBinder), new ViewModelBinder(logger));
        }

        public void RegisterTransient<TService>(Func<TService> instanceFactory)
            => _container.Add(typeof(TService), instanceFactory);

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
                typeof(FakeDependencyResolverCore)
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
    }
}

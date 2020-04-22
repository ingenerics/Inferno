using Inferno.Core;
using SimpleInjector;
using System;
using System.Collections.Generic;

namespace WorldCup.Bootstrap
{
    public class SimpleInjectorDependencyResolver : IDependencyResolver
    {
        private readonly Container _container;
        private readonly IServiceProvider _provider;

        public SimpleInjectorDependencyResolver(Container container)
        {
            _container = container;
            _provider = container;
        }

        public object GetInstance(Type serviceType)
            => _container.GetInstance(serviceType);

        public TService GetInstance<TService>() where TService : class
            => _container.GetInstance<TService>();

        public IEnumerable<object> GetAllInstances(Type serviceType)
            => _container.GetAllInstances(serviceType);

        public IEnumerable<TService> GetAllInstances<TService>() where TService : class
            => _container.GetAllInstances<TService>();

        public bool TryGetInstance(Type serviceType, out object instance)
        {
            instance = _provider.GetService(serviceType);
            return instance != null;
        }

        public bool TryGetInstance<TService>(out TService instance)
            where TService : class
        {
            instance = (TService)_provider.GetService(typeof(TService));
            return instance != null;
        }
    }
}
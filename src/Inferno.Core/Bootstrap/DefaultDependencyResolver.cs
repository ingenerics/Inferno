using System;
using System.Collections.Generic;

namespace Inferno.Core
{
    /// <summary>
    /// Serves as a null object, should always be replaced by a real dependency resolver.
    /// </summary>
    public sealed class DefaultDependencyResolver : IDependencyResolver
    {
        public object GetInstance(Type service)
        {
            return Activator.CreateInstance(service);
        }

        public IEnumerable<object> GetAllInstances(Type service)
        {
            return new[] { Activator.CreateInstance(service) };
        }

        public TService GetInstance<TService>() where TService : class
        {
            return (TService)GetInstance(typeof(TService));
        }

        public IEnumerable<TService> GetAllInstances<TService>() where TService : class
        {
            return (IEnumerable<TService>)GetAllInstances(typeof(TService));
        }

        public bool TryGetInstance(Type serviceType, out object instance)
        {
            instance = GetInstance(serviceType);

            return true;
        }

        public bool TryGetInstance<TService>(out TService instance) where TService : class
        {
            instance = GetInstance<TService>();

            return true;
        }
    }
}
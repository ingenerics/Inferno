using System;
using System.Collections.Generic;

namespace Inferno.Core
{
    public interface IDependencyResolver
    {
        object GetInstance(Type service);
        TService GetInstance<TService>() where TService : class;
        bool TryGetInstance(Type serviceType, out object instance);
        bool TryGetInstance<TService>(out TService instance) where TService : class;

        IEnumerable<object> GetAllInstances(Type service);
        IEnumerable<TService> GetAllInstances<TService>() where TService : class;
    }
}
using Inferno.Core.Logging;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace Inferno.Core.Tests
{
    public class DependencyResolverTests
    {
        [Fact]
        public void DependencyResolver_Instance_Singleton_Generic_Test()
        {
            // Arrange
            var dependencyResolver = new FakeDependencyResolverCore();
            // Act
            var collection = dependencyResolver.GetInstance<ILogger>();
            // Assert
            Assert.NotNull(collection);
        }

        [Fact]
        public void DependencyResolver_Instance_Singleton_Type_Test()
        {
            // Arrange
            var dependencyResolver = new FakeDependencyResolverCore();
            // Act
            var collection = dependencyResolver.GetInstance(typeof(ILogger));
            // Assert
            Assert.NotNull(collection);
        }

        [Fact]
        public void DependencyResolver_Instance_Transient_Generic_Test()
        {
            // Arrange
            var dependencyResolver = new FakeDependencyResolverCore();
            dependencyResolver.RegisterTransient<IList>(() => new List<int>());
            // Act
            var list = dependencyResolver.GetInstance<IList>();
            // Assert
            Assert.NotNull(list);
        }

        [Fact]
        public void DependencyResolver_Instance_Transient_Type_Test()
        {
            // Arrange
            var dependencyResolver = new FakeDependencyResolverCore();
            dependencyResolver.RegisterTransient<IList>(() => new List<int>());
            // Act
            var list = dependencyResolver.GetInstance(typeof(IList));
            // Assert
            Assert.NotNull(list);
        }
    }
}

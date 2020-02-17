using Inferno.Core.Logging;
using Inferno.Core.Tests.Views;
using System.Windows;
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

        [StaFact]
        public void DependencyResolver_Instance_Transient_Generic_Test()
        {
            // Arrange
            var dependencyResolver = new FakeDependencyResolverCore();
            dependencyResolver.RegisterTransient<UIElement>(() => new ShellView());
            // Act
            var view = dependencyResolver.GetInstance<UIElement>();
            // Assert
            Assert.NotNull(view);
            Assert.IsType<ShellView>(view);
        }

        [StaFact]
        public void DependencyResolver_Instance_Transient_Type_Test()
        {
            // Arrange
            var dependencyResolver = new FakeDependencyResolverCore();
            dependencyResolver.RegisterTransient<UIElement>(() => new ShellView());
            // Act
            var view = dependencyResolver.GetInstance(typeof(UIElement));
            // Assert
            Assert.NotNull(view);
            Assert.IsType<ShellView>(view);
        }
    }
}

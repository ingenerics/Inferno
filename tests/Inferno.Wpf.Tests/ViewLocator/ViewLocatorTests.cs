using Inferno.Core;
using Inferno.Wpf.Tests.ViewModels;
using Inferno.Wpf.Tests.Views;
using Inferno.Wpf.Tests.Views.Shared;
using Xunit;

namespace Inferno.Wpf.Tests
{
    public class ViewLocatorTests
    {
        private readonly ViewLocator _viewLocator;

        public ViewLocatorTests()
        {
            var dependencyResolver = new FakeDependencyResolverWpf();
            _viewLocator = (ViewLocator)dependencyResolver.GetInstance<IViewLocator>();
        }

        [StaFact]
        public void NamespaceConventions_ViewModel_OneToOne_View_Test()
        {
            // Arrange
            var viewModel = new ShellViewModel();
            // Act
            var view = _viewLocator.LocateForModel(viewModel, null, false);
            // Assert
            Assert.NotNull(view);
            Assert.IsType<ShellView>(view);
        }

        [StaFact]
        public void NamespaceConventions_ViewModel_OneToMany_View_Test()
        {
            // Arrange
            var viewModel = new SharedViewModel();
            // Act
            var view = _viewLocator.LocateForModel(viewModel, "ViewA", false);
            // Assert
            // Assert
            Assert.NotNull(view);
            Assert.IsType<ViewA>(view);
        }

        [StaFact]
        public void UseViewForAttribute_ViewModel_ManyToOne_View_Test()
        {
            // Arrange
            var viewModel = new AttributedViewModel();
            // Act
            var view = _viewLocator.LocateForModel(viewModel, null, false);
            // Assert
            Assert.NotNull(view);
            Assert.IsType<ShellView>(view);
        }
    }
}

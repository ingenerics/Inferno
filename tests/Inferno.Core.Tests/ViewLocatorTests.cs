using Inferno.Core.Tests.ViewModels;
using Inferno.Core.Tests.Views;
using Inferno.Core.Tests.Views.Shared;
using Xunit;

namespace Inferno.Core.Tests
{
    public class ViewLocatorTests
    {
        private readonly ViewLocator _viewLocator;

        public ViewLocatorTests()
        {
            var dependencyResolver = new FakeDependencyResolverCore();
            _viewLocator = (ViewLocator)dependencyResolver.GetInstance<IViewLocator>();
        }

        [StaFact]
        public void NamespaceConventions_ViewModel_OneToOne_View_Test()
        {
            // Arrange
            var viewModel = new ShellViewModel();
            // Act
            var view = _viewLocator.LocateForModel(viewModel, null);
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
            var view = _viewLocator.LocateForModel(viewModel, "ViewA");
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
            var view = _viewLocator.LocateForModel(viewModel, null);
            // Assert
            Assert.NotNull(view);
            Assert.IsType<ShellView>(view);
        }
    }
}

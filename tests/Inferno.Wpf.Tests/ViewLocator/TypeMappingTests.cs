using Inferno.Core.Conventions;
using System;
using Xunit;

namespace Inferno.Wpf.Tests
{
    /*
     * Tests will fail in debug mode, because of exceptions being thrown.
     */
    public class TypeMappingTests
    {
        private readonly ViewLocator _viewLocator;

        public TypeMappingTests()
        {
            var dependencyResolver = new FakeDependencyResolverWpf();
            _viewLocator = (ViewLocator)dependencyResolver.GetInstance<IViewLocator>();
        }

        [Fact]
        public void ConfigureTypeMappingsShouldThrowWhenDefaultSubNamespaceForViewModelsIsEmpty()
        {
            var config = new TypeMappingConfiguration
            {
                DefaultSubNamespaceForViews = "not empty",
                DefaultSubNamespaceForViewModels = string.Empty,
                NameFormat = "not Empty"
            };

            Assert.Throws<ArgumentException>(() => _viewLocator.ConfigureTypeMappings(config));
        }

        [Fact]
        public void ConfigureTypeMappingsShouldThrowWhenDefaultSubNamespaceForViewModelsIsNull()
        {
            var config = new TypeMappingConfiguration
            {
                DefaultSubNamespaceForViews = "not null",
                DefaultSubNamespaceForViewModels = null,
                NameFormat = "not null"
            };

            Assert.Throws<ArgumentException>(() => _viewLocator.ConfigureTypeMappings(config));
        }

        [Fact]
        public void ConfigureTypeMappingsShouldThrowWhenDefaultSubNamespaceForViewsIsEmpty()
        {
            var config = new TypeMappingConfiguration
            {
                DefaultSubNamespaceForViews = string.Empty,
                DefaultSubNamespaceForViewModels = "not Empty",
                NameFormat = "not Empty"
            };

            Assert.Throws<ArgumentException>(() => _viewLocator.ConfigureTypeMappings(config));
        }

        [Fact]
        public void ConfigureTypeMappingsShouldThrowWhenDefaultSubNamespaceForViewsIsNull()
        {
            var config = new TypeMappingConfiguration
            {
                DefaultSubNamespaceForViews = null,
                DefaultSubNamespaceForViewModels = "not null",
                NameFormat = "not null"
            };

            Assert.Throws<ArgumentException>(() => _viewLocator.ConfigureTypeMappings(config));
        }

        [Fact]
        public void ConfigureTypeMappingsShouldThrowWhenNameFormatIsEmpty()
        {
            var config = new TypeMappingConfiguration
            {
                DefaultSubNamespaceForViews = "not Empty",
                DefaultSubNamespaceForViewModels = "not Empty",
                NameFormat = string.Empty
            };

            Assert.Throws<ArgumentException>(() => _viewLocator.ConfigureTypeMappings(config));
        }

        [Fact]
        public void ConfigureTypeMappingsShouldThrowWhenNameFormatIsNull()
        {
            var config = new TypeMappingConfiguration
            {
                DefaultSubNamespaceForViews = "not null",
                DefaultSubNamespaceForViewModels = "not null",
                NameFormat = null
            };

            Assert.Throws<ArgumentException>(() => _viewLocator.ConfigureTypeMappings(config));
        }

        [Fact]
        public void ConfigureTypeMappingsWithDefaultValuesShouldNotThrow()
        {
            var typeMappingConfiguration = new TypeMappingConfiguration();

            _viewLocator.ConfigureTypeMappings(typeMappingConfiguration);
        }
    }
}

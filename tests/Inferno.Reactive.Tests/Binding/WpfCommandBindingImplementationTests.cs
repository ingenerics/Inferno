using System;
using System.Windows;
using System.Windows.Input;
using Xunit;

namespace Inferno.Reactive.Tests
{
    public class WpfCommandBindingImplementationTests
    {
        public WpfCommandBindingImplementationTests()
        {
            RxApp.Initialize(new FakeDependencyResolverReactive());
        }

        [StaFact]
        public void CommandBindToExplicitEventWireup()
        {
            var vm = new CommandBindingViewModel();
            var view = new CommandBindingView { ViewModel = vm };

            var invokeCount = 0;
            vm.Command2.Subscribe(_ => invokeCount++);

            var disp = view.BindCommand(vm, x => x.Command2, x => x.Command2, "MouseUp");

            view.Command2.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });

            disp.Dispose();

            view.Command2.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });
            Assert.Equal(1, invokeCount);
        }
    }
}